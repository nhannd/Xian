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
    /// Proxy class for updating the status and progress of a <see cref="WorkItem"/>.
    /// </summary>
    public class WorkItemStatusProxy
    {
        #region Public Properties

        // Hard-coded log level for proxy
        public LogLevel LogLevel = LogLevel.Info;

        public WorkItem Item { get; private set; }
        public WorkItemProgress Progress { get; set; }
        public WorkItemRequest Request { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="item">The WorkItem to create a proxy for.</param>
        public WorkItemStatusProxy(WorkItem item)
        {
            Item = item;
            Progress = item.Progress;
            Request = item.Request;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Simple routine for failing a <see cref="WorkItem"/> and save a reason.
        /// </summary>
        /// <param name="reason">A non-localized reason for the failure.</param>
        /// <param name="failureType">The type of failure.</param>
        public void Fail(string reason, WorkItemFailureType failureType)
        {
            Progress.StatusDetails = reason;
            Fail(failureType);
        }

        /// <summary>
        /// Simple routine for failing a <see cref="WorkItem"/>.
        /// </summary>
        /// <param name="failureType"></param>
        public void Fail(WorkItemFailureType failureType)
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var workItemBroker = context.GetWorkItemBroker();

                Item = workItemBroker.GetWorkItem(Item.Oid);
                DateTime now = Platform.Time;

                Item.Progress = Progress;
                Item.FailureCount = Item.FailureCount + 1;
                Item.DeleteTime = now.AddMinutes(WorkItemServiceSettings.Instance.DeleteDelayMinutes);
                if (Item.FailureCount >= WorkItemServiceSettings.Instance.RetryCount
                    || failureType == WorkItemFailureType.Fatal )
                {
                    Item.Status = WorkItemStatusEnum.Failed;
                    Item.ExpirationTime = now;
                    Item.ScheduledTime = now;
                }
                else
                {
                    Item.ScheduledTime = now.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);
                    if (Item.ExpirationTime < Item.ScheduledTime)
                        Item.ExpirationTime = Item.ScheduledTime;
                    Item.Status = WorkItemStatusEnum.Pending;
                }

                context.Commit();
            }

            Publish();
            Platform.Log(LogLevel, "Failing {0} WorkItem for OID {1}", Item.Type, Item.Oid);
        }

        /// <summary>
        /// Postpone a <see cref="WorkItem"/>
        /// </summary>
        public void Postpone()
        {
            DateTime newScheduledTime = Platform.Time.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var workItemBroker = context.GetWorkItemBroker();

                Item = workItemBroker.GetWorkItem(Item.Oid);
                Item.Progress = Progress;
                Item.ScheduledTime = newScheduledTime;
                if (Item.ScheduledTime > Item.ExpirationTime)
                    Item.ScheduledTime = Item.ExpirationTime;
                Item.Status = WorkItemStatusEnum.Pending;
                context.Commit();
            }

            Publish();
            Platform.Log(LogLevel, "Postponing {0} WorkItem for OID {1}", Item.Type, Item.Oid);
        }

        /// <summary>
        /// Complete a <see cref="WorkItem"/>.
        /// </summary>
        public void Complete()
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();
           
                Item = broker.GetWorkItem(Item.Oid);

                DateTime now = Platform.Time;

                Item.Progress = Progress;
                Item.ScheduledTime = now;
                Item.ExpirationTime = now;
                Item.DeleteTime = now.AddMinutes(WorkItemServiceSettings.Instance.DeleteDelayMinutes);
                Item.Status = WorkItemStatusEnum.Complete;

                var uidBroker = context.GetWorkItemUidBroker();
                foreach (var entity in Item.WorkItemUids)
                {
                    uidBroker.Delete(entity);
                }

                context.Commit();
            }

            Publish();
            Platform.Log(LogLevel, "Completing {0} WorkItem for OID {1}", Item.Type, Item.Oid);
        }

        /// <summary>
        /// Make a <see cref="WorkItem"/> Idle.
        /// </summary>
        public void Idle()
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();
              
                Item = broker.GetWorkItem(Item.Oid);

                DateTime now = Platform.Time;

                Item.Progress = Progress;
                Item.ScheduledTime = now.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);
                if (Item.ScheduledTime > Item.ExpirationTime)
                    Item.ScheduledTime = Item.ExpirationTime;
                Item.Status = WorkItemStatusEnum.Idle;

                context.Commit();
            }

            Publish();
            Platform.Log(LogLevel, "Idling {0} WorkItem for OID {1}", Item.Type, Item.Oid);
        }

        /// <summary>
        /// Cancel a <see cref="WorkItem"/>
        /// </summary>
        public void Cancel()
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();
                
                Item = broker.GetWorkItem(Item.Oid);
                
                DateTime now = Platform.Time;

                Item.ScheduledTime = now;
                Item.ExpirationTime = now;
                Item.DeleteTime = now.AddMinutes(WorkItemServiceSettings.Instance.DeleteDelayMinutes);
                Item.Status = WorkItemStatusEnum.Canceled;
                Item.Progress = Progress;
                
                context.Commit();
            }

            Publish();
            Platform.Log(LogLevel, "Canceling {0} WorkItem for OID {1}", Item.Type, Item.Oid);
        }

        /// <summary>
        /// Delete a <see cref="WorkItem"/>.
        /// </summary>
        public void Delete()
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();

                Item = broker.GetWorkItem(Item.Oid);
                Item.Status = WorkItemStatusEnum.Deleted;
                broker.Delete(Item);

                context.Commit();
            }

            Publish();
            Platform.Log(LogLevel, "Deleting {0} WorkItem for OID {1}", Item.Type, Item.Oid);
        }

        /// <summary>
        /// Update the progress for a <see cref="WorkItem"/>.  Progress will be published.
        /// </summary>
        public void UpdateProgress()
        {
            // We were originally committing to the database here, but decided to only commit when done processing the WorkItem.
             Publish();
        }

        #endregion

        #region Private Methods

        private void Publish()
        {
            Item.Progress = Progress;
			WorkItemActivityPublisher.WorkItemChanged(WorkItemHelper.FromWorkItem(Item));
        }

        #endregion
    }
}
