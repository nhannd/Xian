#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
    /// <summary>
    /// Implementation of <see cref="IWorkItemService"/> for processing requests to manipulate WorkItems.
    /// </summary>
    public class WorkItemService : IWorkItemService
    {
        #region Private Members

        private static WorkItemService _instance;

        #endregion

        #region Properties

        public static WorkItemService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WorkItemService();
                }

                return _instance;
            }
        }

        #endregion

        #region Public Methods

        public void Start()
        {         
            try
            {
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e, "Failed to start WorkItemService.");
            }
        }

        public void Stop()
        {
        }

        public WorkItemInsertResponse Insert(WorkItemInsertRequest request)
        {
            var response = new WorkItemInsertResponse();

            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                DateTime now = Platform.Time;
                var broker = context.GetWorkItemBroker();

                if (request.Request.WorkItemType.Equals(ReindexRequest.WorkItemTypeString))
                {
                    var list = broker.GetWorkItems(request.Request.WorkItemType, null, null);
                    foreach (var workItem in list)
                    {
                        if (workItem.Status == WorkItemStatusEnum.Pending
                            || workItem.Status == WorkItemStatusEnum.InProgress)
                        {
                            response.Item = WorkItemDataHelper.FromWorkItem(workItem);
                            return response;
                        }
                    }
                }

                var studyRequest = request.Request as WorkItemStudyRequest;
                if (studyRequest != null)
                {
                    var list = broker.GetWorkItems(request.Request.WorkItemType, null, studyRequest.Study.StudyInstanceUid);
                    foreach (var workItem in list)
                    {
                        if (workItem.Status == WorkItemStatusEnum.Pending
                            || workItem.Status == WorkItemStatusEnum.InProgress)
                        {
                            // Mark studies to delete as "deleted" in the database.
                            var studyBroker = context.GetStudyBroker();
                            var study = studyBroker.GetStudy(studyRequest.Study.StudyInstanceUid);
                            if (study != null)
                            {
                                study.Deleted = true;
                                context.Commit();
                            }

                            response.Item = WorkItemDataHelper.FromWorkItem(workItem);
                            return response;
                        }
                    }
                }


                var item = new WorkItem
                               {
                                   Request = request.Request,
                                   Progress = request.Progress,
                                   Type = request.Request.WorkItemType,
                                   Priority = request.Request.Priority,
                                   ScheduledTime = now.AddSeconds(WorkItemServiceSettings.Default.InsertDelaySeconds),
                                   ProcessTime = now.AddSeconds(WorkItemServiceSettings.Default.InsertDelaySeconds),
                                   DeleteTime = now.AddMinutes(WorkItemServiceSettings.Default.DeleteDelayMinutes),
                                   ExpirationTime = now.AddSeconds(WorkItemServiceSettings.Default.ExpireDelaySeconds),
                                   RequestedTime = now,
                                   Status = WorkItemStatusEnum.Pending
                               };

                if (studyRequest != null)
                {
                    item.StudyInstanceUid = studyRequest.Study.StudyInstanceUid;

                    if (request.Request.WorkItemType.Equals(DeleteStudyRequest.WorkItemTypeString))
                    {
                        // Mark studies to delete as "deleted" in the database.
                        var studyBroker = context.GetStudyBroker();
                        var study = studyBroker.GetStudy(studyRequest.Study.StudyInstanceUid);
                        if (study != null)
                            study.Deleted = true;
                    }
                }

                broker.AddWorkItem(item);
                
                context.Commit();

                response.Item = WorkItemDataHelper.FromWorkItem(item);
            }

            // Cache the UserIdentityContext for later use by the shred
            if (request.Request.WorkItemType.Equals(ImportFilesRequest.WorkItemTypeString))
                UserIdentityCache.Put(response.Item.Identifier,UserIdentityContext.CreateFromCurrentThreadPrincipal());

			WorkItemPublishSubscribeHelper.PublishWorkItemChanged(WorkItemsChangedEventType.Update, response.Item);
            if (WorkItemProcessor.Instance != null)
                WorkItemProcessor.Instance.SignalThread();

            return response;
        }

        public WorkItemUpdateResponse Update(WorkItemUpdateRequest request)
        {
            var response = new WorkItemUpdateResponse();
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();
                var workItem = broker.GetWorkItem(request.Identifier);
                if (workItem == null)
                {
                    response.Item = null;
                    return response;
                }

                bool deleted = false;

                if (request.Delete.HasValue && request.Delete.Value)
                {
                    if (workItem.Status != WorkItemStatusEnum.InProgress)
                    {
                        workItem.Status = WorkItemStatusEnum.Deleted;
                        deleted = true;            
                    }
                }
                if (!deleted)
                {
                    if (request.ExpirationTime.HasValue)
                        workItem.ExpirationTime = request.ExpirationTime.Value;
                    if (request.Priority.HasValue)
                        workItem.Priority = request.Priority.Value;
                    if (request.Status.HasValue && workItem.Status != WorkItemStatusEnum.InProgress)
                    {
                        workItem.Status = request.Status.Value;
                        if (request.Status.Value == WorkItemStatusEnum.Canceled)
                            workItem.DeleteTime = Platform.Time.AddMinutes(WorkItemServiceSettings.Default.DeleteDelayMinutes);
                        else if (request.Status.Value == WorkItemStatusEnum.Pending)
                        {
                            workItem.ScheduledTime = Platform.Time;
                            workItem.FailureCount = 0;
                        }

                        // Cache the UserIdentityContext for later use by the shred
                        if (workItem.Request.WorkItemType.Equals(ImportFilesRequest.WorkItemTypeString) && request.Status.Value == WorkItemStatusEnum.Pending)
                            UserIdentityCache.Put(workItem.Oid, UserIdentityContext.CreateFromCurrentThreadPrincipal());

                    }
                    if (request.ProcessTime.HasValue)
                        workItem.ProcessTime = request.ProcessTime.Value;

                    if (request.Cancel.HasValue && request.Cancel.Value)
                    {
                        if (workItem.Progress == null || workItem.Progress.IsCancelable)
                        {
                            if (workItem.Status.Equals(WorkItemStatusEnum.Idle)
                                || workItem.Status.Equals(WorkItemStatusEnum.Pending))
                            {
                                workItem.Status = WorkItemStatusEnum.Canceled;

                                // Force the study to be visible again if its a DeleteStudyRequest we're canceling
                                if (workItem.Type.Equals(DeleteStudyRequest.WorkItemTypeString))
                                {
                                    var studyBroker = context.GetStudyBroker();
                                    var study = studyBroker.GetStudy(workItem.StudyInstanceUid);
                                    if (study != null)
                                    {
                                        study.Deleted = false;
                                    }
                                }
                            }
                            else if (workItem.Status.Equals(WorkItemStatusEnum.InProgress))
                            {
                                // Abort the WorkItem
                                WorkItemProcessor.Instance.Cancel(workItem.Oid);
                            }
                        }
                    }
                }

                context.Commit();

                response.Item = WorkItemDataHelper.FromWorkItem(workItem);
            }

			WorkItemPublishSubscribeHelper.PublishWorkItemChanged(WorkItemsChangedEventType.Update, response.Item);

            return response;
        }

        public WorkItemQueryResponse Query(WorkItemQueryRequest request)
        {
            var response = new WorkItemQueryResponse();
            using (var context = new DataAccessContext())
            {
                var broker = context.GetWorkItemBroker();
 
                var dbList = broker.GetWorkItems(request.Type, request.Status, request.StudyInstanceUid);

                var results = new List<WorkItemData>();

                foreach (var dbItem in dbList)
                {
                    results.Add(WorkItemDataHelper.FromWorkItem(dbItem));
                }

                response.Items = results.ToArray();
            }
            return response;
        }

        #endregion
    }
}
