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
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor
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
        /// Simple routine for failing a <see cref="WorkItem"/> and save a reason.
        /// </summary>
        /// <param name="reason">A non-localized reason for the failure.</param>
        /// <param name="failureType">The type of failure.</param>
        /// <param name="scheduledTime">The time to reschedule the WorkItem if it isn't a fatal error. </param>
        public void Fail(string reason, WorkItemFailureType failureType, DateTime scheduledTime)
        {
            Progress.StatusDetails = reason;
            Fail(failureType, scheduledTime);
        }

        /// <summary>
        /// SImple routine for failing a <see cref="WorkItem"/>
        /// </summary>
        /// <param name="failureType"></param>
        public void Fail(WorkItemFailureType failureType)
        {
            Fail(failureType, Platform.Time.AddSeconds(WorkItemServiceSettings.Default.PostponeSeconds));
        }

        /// <summary>
        /// Simple routine for failing a <see cref="WorkItem"/> and rescheduling it at a specified time.
        /// </summary>
        /// <param name="failureType"></param>
        /// <param name="scheduledTime">The time to reschedule the WorkItem if it isn't a fatal error. </param>
        public void Fail(WorkItemFailureType failureType, DateTime scheduledTime)
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var workItemBroker = context.GetWorkItemBroker();

                Item = workItemBroker.GetWorkItem(Item.Oid);
                DateTime now = Platform.Time;

                Item.Progress = Progress;
                Item.FailureCount = Item.FailureCount + 1;
                Item.DeleteTime = now.AddMinutes(WorkItemServiceSettings.Default.DeleteDelayMinutes);
                if (Item.FailureCount >= WorkItemServiceSettings.Default.RetryCount
                    || failureType == WorkItemFailureType.Fatal )
                {
                    Item.Status = WorkItemStatusEnum.Failed;
                    Item.ExpirationTime = now;
                    Item.ScheduledTime = now;
                }
                else
                {
                    Item.ScheduledTime = scheduledTime;
                    if (Item.ExpirationTime < Item.ScheduledTime)
                        Item.ExpirationTime = Item.ScheduledTime;
                    Item.Status = WorkItemStatusEnum.Pending;
                }

                context.Commit();
            }

            Publish(false);
            Platform.Log(LogLevel, "Failing {0} WorkItem for OID {1}", Item.Type, Item.Oid);
        }

        /// <summary>
        /// Postpone a <see cref="WorkItem"/>
        /// </summary>
        /// <param name="scheduledTime">The time to postpone the entry to.</param>
        public void Postpone(DateTime scheduledTime)
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var workItemBroker = context.GetWorkItemBroker();

                Item = workItemBroker.GetWorkItem(Item.Oid);
                Item.Progress = Progress;
                Item.ScheduledTime = scheduledTime;
                if (Item.ScheduledTime > Item.ExpirationTime)
                    Item.ExpirationTime = Item.ScheduledTime;
                Item.Status = WorkItemStatusEnum.Pending;
                context.Commit();
            }

            Publish(false);
            Platform.Log(LogLevel, "Postponing {0} WorkItem for OID {1} until {2}, expires {3}", Item.Type, Item.Oid, Item.ScheduledTime.ToLongTimeString(), Item.ExpirationTime.ToLongTimeString());
        }

        /// <summary>
        /// Postpone a <see cref="WorkItem"/>
        /// </summary>
        public void Postpone()
        {
            DateTime now = Platform.Time;
            DateTime newScheduledTime = now.AddSeconds(WorkItemServiceSettings.Default.PostponeSeconds);
            Postpone(newScheduledTime);
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
                Item.DeleteTime = now.AddMinutes(WorkItemServiceSettings.Default.DeleteDelayMinutes);
                Item.Status = WorkItemStatusEnum.Complete;

                var uidBroker = context.GetWorkItemUidBroker();
                foreach (var entity in Item.WorkItemUids)
                {
                    uidBroker.Delete(entity);
                }

                context.Commit();
            }

            Publish(false);
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
                Item.ScheduledTime = now.AddSeconds(WorkItemServiceSettings.Default.PostponeSeconds);
                if (Item.ScheduledTime > Item.ExpirationTime)
                    Item.ScheduledTime = Item.ExpirationTime;
                Item.Status = WorkItemStatusEnum.Idle;

                context.Commit();
            }

            Publish(false);
            Platform.Log(LogLevel, "Idling {0} WorkItem for OID {1} until {2}, expires {3}", Item.Type, Item.Oid, Item.ScheduledTime.ToLongTimeString(), Item.ExpirationTime.ToLongTimeString());
        }

        /// <summary>
        /// Mark <see cref="WorkItem"/> as being in the process of canceling
        /// </summary>
        public void Canceling()
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var workItemBroker = context.GetWorkItemBroker();

                Item = workItemBroker.GetWorkItem(Item.Oid);
                Item.Progress = Progress;
                Item.Status = WorkItemStatusEnum.Canceling;
                context.Commit();
            }

            Publish(false);
            Platform.Log(LogLevel, "Canceling {0} WorkItem for OID {1}", Item.Type, Item.Oid);
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
                Item.DeleteTime = now.AddMinutes(WorkItemServiceSettings.Default.DeleteDelayMinutes);
                Item.Status = WorkItemStatusEnum.Canceled;
                Item.Progress = Progress;
                
                context.Commit();
            }

            Publish(false);
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

            Publish(false);
            Platform.Log(LogLevel, "Deleting {0} WorkItem for OID {1}", Item.Type, Item.Oid);
        }

        /// <summary>
        /// Update the progress for a <see cref="WorkItem"/>.  Progress will be published.
        /// </summary>
        public void UpdateProgress()
        {
            // We were originally committing to the database here, but decided to only commit when done processing the WorkItem.
            // This could lead to some misleading progress if a Refresh is done.
             Publish(false);
        }

        /// <summary>
        /// Update the progress for a <see cref="WorkItem"/>.  Progress will be published.
        /// </summary>
        public void UpdateProgress(bool updateDatabase)
        {
            // We were originally committing to the database here, but decided to only commit when done processing the WorkItem.
            // This could lead to some misleading progress if a Refresh is done.
            Publish(updateDatabase);
        }
        #endregion

        #region Private Methods

        private void Publish(bool saveToDatabase)
        {
            if (saveToDatabase)
            {
                using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
                {
                    var broker = context.GetWorkItemBroker();

                    Item = broker.GetWorkItem(Item.Oid);

                    Item.Progress = Progress;

                    context.Commit();
                }
            }
            else
                Item.Progress = Progress;

			WorkItemPublishSubscribeHelper.PublishWorkItemChanged(WorkItemDataHelper.FromWorkItem(Item));
        }

        #endregion
    }
}
