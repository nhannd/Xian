#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
    /// <summary>
    /// Enum telling if a work queue entry had a fatal or nonfatal error.
    /// </summary>
    public enum WorkItemFailureType
    {
        Fatal,
        NonFatal
    }

    /// <summary>
    /// Enum for telling when processing is complete for a WorkQueue item.
    /// </summary>
    public enum WorkItemProcessStatus
    {
        Complete,
        Pending,
        Idle,
        IdleNoDelete,
        CompleteDelayDelete
    }

    public class WorkItemStatusProxy
    {
        public WorkItem Item { get; private set; }
        public WorkItemProgress Progress { get; set; }
        public WorkItemRequest Request { get; set; }
        public WorkItemStatusProxy(WorkItem item)
        {
            Item = item;
            Progress = item.Progress;
            Request = item.Request;
        }

        /// <summary>
        /// Simple routine for failing a work queue item.
        /// </summary>
        /// <param name="failureDescription">The reason for the failure.</param>
        /// <param name="failureType"></param>
        public void Fail(string failureDescription, WorkItemFailureType failureType)
        {
            using (var context = new DataAccessContext())
            {
                var workItemBroker = context.GetWorkItemBroker();

                Item = workItemBroker.GetWorkItem(Item.Oid);
                DateTime now = Platform.Time;

                Item.Progress = Progress;
                Item.FailureCount = Item.FailureCount + 1;
                Item.ScheduledTime = now;
                Item.ExpirationTime = now.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);
                if (Item.FailureCount >= WorkItemServiceSettings.Instance.RetryCount
                    || failureType == WorkItemFailureType.Fatal )
                {
                    Item.Status = WorkItemStatusEnum.Failed;
                    Item.ExpirationTime = now;
                }
                else
                {
                    Item.ExpirationTime = Platform.Time.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);
                    Item.Status = WorkItemStatusEnum.Pending;
                }

                context.Commit();
            }

            Publish();
        }

        public void Postpone()
        {
            DateTime newScheduledTime = Platform.Time.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);
            DateTime expireTime = newScheduledTime.Add(TimeSpan.FromSeconds(WorkItemServiceSettings.Instance.ExpireDelaySeconds));
            using (var context = new DataAccessContext())
            {
                var workItemBroker = context.GetWorkItemBroker();

                Item = workItemBroker.GetWorkItem(Item.Oid);
                Item.Progress = Progress;
                Item.ScheduledTime = newScheduledTime;
                Item.ExpirationTime = expireTime;
                Item.Status = WorkItemStatusEnum.Pending;
                context.Commit();
            }

            Publish();
        }

        public void Complete()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetWorkItemBroker();
           
                Item = broker.GetWorkItem(Item.Oid);

                DateTime now = Platform.Time;

                Item.Progress = Progress;
                Item.ScheduledTime = now;
                Item.ExpirationTime = now;
                Item.Status = WorkItemStatusEnum.Complete;

                var uidBroker = context.GetWorkItemUidBroker();
                foreach (var entity in Item.WorkItemUids)
                {
                    uidBroker.Delete(entity);
                }

                context.Commit();
            }

            Publish();
        }

        public void Idle()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetWorkItemBroker();
              
                Item = broker.GetWorkItem(Item.Oid);

                DateTime now = Platform.Time;

                Item.Progress = Progress;
                Item.ScheduledTime = now.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);
                Item.Status = WorkItemStatusEnum.Idle;

                context.Commit();
            }

            Publish();
        }

        public void Cancel()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetWorkItemBroker();
                
                Item = broker.GetWorkItem(Item.Oid);
                
                DateTime now = Platform.Time;

                Item.ScheduledTime = now;
                Item.ExpirationTime = now;
                Item.Status = WorkItemStatusEnum.Canceled;
                Item.Progress = Progress;
                
                context.Commit();
            }

            Publish();
        }

        public void Delete()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetWorkItemBroker();

                Item = broker.GetWorkItem(Item.Oid);
                broker.Delete(Item);

                context.Commit();
            }

            Publish();
        }

        public void UpdateProgress()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetWorkItemBroker();
                
                Item = broker.GetWorkItem(Item.Oid);

                Item.Progress = Progress;

                context.Commit();
            }

            Publish();
        }

        private void Publish()
        {
            try
            {
                PublishManager<IWorkItemActivityCallback>.Publish("WorkItemChanged", WorkItemHelper.FromWorkItem(Item));
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Warn, e, "Unexpected error attempting to publish WorkItem status");
            }            
        }
    }
}
