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
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
    public class WorkItemService : IWorkItemService
    {
        private static WorkItemService _instance;
        private bool _disabled;

        public static WorkItemService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WorkItemService();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        private void Initialize()
        {
            try
            {
             
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                _disabled = true;
            }
        }

        public void Start()
        {
            CheckDisabled();          
          
            try
            {
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e, "Failed to start purge timer; old items will never be purged.");
            }
        }

        public void Stop()
        {
            CheckDisabled();
   
        }

        private void CheckDisabled()
        {
            if (_disabled)
                throw new Exception(SR.ExceptionServiceHasBeenDisabled);
        }

        public WorkItemInsertResponse Insert(WorkItemInsertRequest request)
        {
            var response = new WorkItemInsertResponse();

            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                DateTime now = Platform.Time;
                var broker = context.GetWorkItemBroker();

                if (request.Request.Type == WorkItemTypeEnum.ReIndex)
                {
                    var list = broker.GetWorkItems(request.Request.Type, null, null);
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
                                   Type = request.Request.Type,
                                   Priority = request.Request.Priority,
                                   InsertTime = now,
                                   ScheduledTime = now.AddSeconds(5),
                                   DeleteTime = now.AddMinutes(WorkItemServiceSettings.Instance.DeleteDelayMinutes),
                                   ExpirationTime = now.AddSeconds(WorkItemServiceSettings.Instance.ExpireDelaySeconds),
                                   Status = WorkItemStatusEnum.Pending
                               };

                broker.AddWorkItem(item);
                
                context.Commit();

                response.Item = WorkItemHelper.FromWorkItem(item);
            }

            try
            {
                PublishManager<IWorkItemActivityCallback>.Publish("WorkItemChanged", response.Item);
                WorkItemProcessor.Instance.SignalThread();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e, "Unexpected error attempting to publish WorkItem status");
            }            

            return response;
        }

        public WorkItemUpdateResponse Update(WorkItemUpdateRequest request)
        {
            var response = new WorkItemUpdateResponse();
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();
                var workItem = broker.GetWorkItem(request.Identifier);

                if (request.ExpirationTime.HasValue)
                    workItem.ExpirationTime = request.ExpirationTime.Value;
                if (request.Priority.HasValue)
                    workItem.Priority = request.Priority.Value;
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
                context.Commit();
            }
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


        public WorkItemPublishResponse Publish(WorkItemPublishRequest request)
        {
            try
            {
                PublishManager<IWorkItemActivityCallback>.Publish("WorkItemChanged", request.Item);
                return new WorkItemPublishResponse();
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                var message = SR.ExceptionErrorProcessingUnsubscribe;
                var exceptionMessage = String.Format("{0}\nDetail:{1}", message, e.Message);
                throw new WorkItemServiceException(exceptionMessage);
            }
        }
    }
}
