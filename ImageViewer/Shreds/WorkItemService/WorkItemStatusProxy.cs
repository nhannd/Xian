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

        public WorkItemStatusProxy(WorkItem item)
        {
            Item = item;
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

                Item.FailureCount = Item.FailureCount + 1;
                Item.ScheduledTime = now;
                Item.ExpirationTime = now.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);
                if (Item.FailureCount >= WorkItemServiceSettings.Instance.RetryCount)
                if (failureType == WorkItemFailureType.Fatal)
                {
                    Item.Status = WorkItemStatusEnum.Failed;
                    Item.ExpirationTime = now;
                }
                else
                {
                    Item.ExpirationTime = Platform.Time.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);
                }

                context.Commit();
            }
        }

        public void Postpone()
        {
            DateTime newScheduledTime = Platform.Time.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);
            DateTime expireTime = newScheduledTime.Add(TimeSpan.FromSeconds(WorkItemServiceSettings.Instance.ExpireDelaySeconds));
            using (var context = new DataAccessContext())
            {
                var workItemBroker = context.GetWorkItemBroker();

                Item = workItemBroker.GetWorkItem(Item.Oid);
                Item.ScheduledTime = newScheduledTime;
                Item.ExpirationTime = expireTime;

                context.Commit();
            }
        }

        /// <summary>
        /// Set a status of <see cref="WorkItem"/> item after batch processing has been completed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This routine will set the status of the <see cref="WorkItem"/> to one of the followings
        /// <list type="bullet">
        /// <item>Failed: if the current process failed and number of retries has been reached.</item>
        /// <item>Pending: if the current batch has been processed successfully</item>
        /// <item>Idle : if current batch size = 0.</item>
        /// <item>Completed: if batch size =0 (idle) and the item has expired.</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="status">Indicates if complete.</param>
        protected virtual void PostProcessing(WorkItemProcessStatus status)
        {            
            using (var dataContext = new DataAccessContext())
            {
                var workItemBroker = dataContext.GetWorkItemBroker();

                Item = workItemBroker.GetWorkItem(Item.Oid);
                
                Item.ScheduledTime = Item.ScheduledTime.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);

                var now = Platform.Time;

                if (Item.ScheduledTime > Item.ExpirationTime)
                    Item.ScheduledTime = Item.ExpirationTime;

                if (status == WorkItemProcessStatus.CompleteDelayDelete)
                {
                    Item.Status = WorkItemStatusEnum.Idle;
                    Item.ScheduledTime =
                        Item.ExpirationTime = now.AddSeconds(WorkItemServiceSettings.Instance.ExpireDelaySeconds);
                }
                else if (status == WorkItemProcessStatus.Complete
                         || (status == WorkItemProcessStatus.Idle && Item.ExpirationTime < now))
                {
                    Item.Status = WorkItemStatusEnum.Complete;
                    Item.FailureCount = Item.FailureCount;
                    Item.ScheduledTime = now;
                }
                else if (status == WorkItemProcessStatus.Idle
                         || status == WorkItemProcessStatus.IdleNoDelete)
                {
                    DateTime scheduledTime = now.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);
                    if (scheduledTime > Item.ExpirationTime)
                        scheduledTime = Item.ExpirationTime;

                    Item.Status = WorkItemStatusEnum.Idle;
                    Item.ScheduledTime = scheduledTime;
                }
                else
                {
                    Item.Status = WorkItemStatusEnum.Pending;

                    Item.ExpirationTime = now.AddSeconds(WorkItemServiceSettings.Instance.ExpireDelaySeconds);
                    Item.ScheduledTime = now.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);
                }


                dataContext.Commit();
            }
        }

        public void Complete()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetWorkItemBroker();
                Item = broker.GetWorkItem(Item.Oid);

                DateTime now = Platform.Time;

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
        }

        public void Idle()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetWorkItemBroker();
                Item = broker.GetWorkItem(Item.Oid);

                DateTime now = Platform.Time;

                Item.ScheduledTime = now.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);
                Item.Status = WorkItemStatusEnum.Idle;

                context.Commit();
            }
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

                context.Commit();
            }
        }
    }
}
