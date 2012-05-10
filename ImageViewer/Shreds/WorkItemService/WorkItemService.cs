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
        private bool _disabled;

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
            CheckDisabled();          
          
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
            CheckDisabled();
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
                            response.Item = WorkItemHelper.FromWorkItem(workItem);
                            return response;
                        }
                    }
                }
                
                var item = new WorkItem
                               {
                                   Request = request.Request,
                                   Type = request.Request.WorkItemType,
                                   Priority = request.Request.Priority,
                                   InsertTime = now,
                                   ScheduledTime = now,
                                   DeleteTime = now.AddMinutes(WorkItemServiceSettings.Instance.DeleteDelayMinutes),
                                   ExpirationTime = now.AddSeconds(WorkItemServiceSettings.Instance.ExpireDelaySeconds),
                                   Status = WorkItemStatusEnum.Pending
                               };

                var studyRequest = request.Request as WorkItemStudyRequest;
                if (studyRequest != null)
                {
                    item.StudyInstanceUid = studyRequest.Study.StudyInstanceUid;

                    if (request.Request.WorkItemType.Equals(DeleteStudyRequest.WorkItemTypeString))
                    {
                        // Mark studies to delete as "deleted" in the database.
                        var studyBroker = context.GetStudyBroker();
                        var study = studyBroker.GetStudy(studyRequest.Study.StudyInstanceUid);
                        study.Deleted = true;
                    }
                }


                broker.AddWorkItem(item);
                
                context.Commit();

                response.Item = WorkItemHelper.FromWorkItem(item);
            }

			WorkItemPublishSubscribeHelper.PublishWorkItemChanged(response.Item);
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
                            workItem.DeleteTime = Platform.Time.AddMinutes(WorkItemServiceSettings.Instance.DeleteDelayMinutes);              
                    }
                    if (request.ScheduledTime.HasValue)
                        workItem.ScheduledTime = request.ScheduledTime.Value;

                    if (request.Cancel.HasValue && request.Cancel.Value)
                    {
                        if (workItem.Progress == null || workItem.Progress.IsCancelable)
                        {
                            if (workItem.Status.Equals(WorkItemStatusEnum.Idle)
                                || workItem.Status.Equals(WorkItemStatusEnum.Pending))
                                workItem.Status = WorkItemStatusEnum.Canceled;
                            else if (workItem.Status.Equals(WorkItemStatusEnum.InProgress))
                            {
                                // Abort the WorkItem
                                WorkItemProcessor.Instance.Cancel(workItem.Oid);
                            }
                        }
                    }
                }

                context.Commit();

                response.Item = WorkItemHelper.FromWorkItem(workItem);
            } 

			WorkItemPublishSubscribeHelper.PublishWorkItemChanged(response.Item);

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
                    results.Add(WorkItemHelper.FromWorkItem(dbItem));
                }

                response.Items = results.ToArray();
            }
            return response;
        }

        #endregion

        #region Private Methods

        private void CheckDisabled()
        {
            if (_disabled)
                throw new Exception(SR.ExceptionServiceHasBeenDisabled);
        }

        #endregion
    }
}
