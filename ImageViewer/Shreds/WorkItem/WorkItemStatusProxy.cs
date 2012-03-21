using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Shreds.WorkItem
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
        public StudyManagement.Storage.WorkItem Item { get; private set; }

        public WorkItemStatusProxy(StudyManagement.Storage.WorkItem item)
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
                DateTime now = Platform.Time;

                //item.FailureCount = item.FailureCount + 1;
                Item.ScheduledTime = now;
                Item.ExpirationTime = now.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);
                //if ((item.FailureCount + 1) > prop.MaxFailureCount)
                if (failureType == WorkItemFailureType.Fatal)
                {
                    Item.Status = WorkItemStatusEnum.Failed;
                    Item.ExpirationTime = now;
                }
                else
                {
                    Item.ExpirationTime = Platform.Time.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);
                }

                workItemBroker.Update(Item);

                context.Commit();
            }
        }

        public void Postpone(string reasonText)
        {
            DateTime newScheduledTime = Platform.Time.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);
            DateTime expireTime = newScheduledTime.Add(TimeSpan.FromSeconds(WorkItemServiceSettings.Instance.ExpireDelaySeconds));
            using (var context = new DataAccessContext())
            {
                Item.ScheduledTime = newScheduledTime;
                Item.ExpirationTime = expireTime;

                var broker = context.GetWorkItemBroker();

                broker.Update(Item);
                context.Commit();
            }
        }

        /// <summary>
        /// Set a status of <see cref="WorkItem"/> item after batch processing has been completed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This routine will set the status of the <paramref name="item"/> to one of the followings
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
            bool completed = status == WorkItemProcessStatus.Complete
                        || (status == WorkItemProcessStatus.Idle && Item.ExpirationTime < Platform.Time);

            using (var dataContext = new DataAccessContext())
            {
                Item.ScheduledTime = Item.ScheduledTime.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);

                DateTime scheduledTime;
                var now = Platform.Time;

                if (Item.ScheduledTime > Item.ExpirationTime)
                    Item.ScheduledTime = Item.ExpirationTime;

                if (status == WorkItemProcessStatus.CompleteDelayDelete)
                {
                    Item.Status = WorkItemStatusEnum.Idle;
                    item.FailureDescription = string.Empty;
                    Item.ScheduledTime =
                        Item.ExpirationTime = now.AddSeconds(WorkQueueProperties.DeleteDelaySeconds);
                }
                else if (status == WorkItemProcessStatus.Complete
                         || (status == WorkItemProcessStatus.Idle && Item.ExpirationTime < now))
                {
                    Item.Status = WorkItemStatusEnum.Complete;
                    Item.FailureCount = Item.FailureCount;
                    Item.ScheduledTime = scheduledTime;

                    completed = true;
                }
                else if (status == WorkItemProcessStatus.Idle
                         || status == WorkItemProcessStatus.IdleNoDelete)
                {
                    scheduledTime = now.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);
                    if (scheduledTime > Item.ExpirationTime)
                        scheduledTime = Item.ExpirationTime;

                    Item.Status = WorkItemStatusEnum.Idle;
                    Item.ScheduledTime = scheduledTime;
                }
                else
                {
                    Item.Status = WorkItemStatusEnum.Pending;

                    Item.ExpirationTime = scheduledTime.AddSeconds(WorkItemServiceSettings.Instance.ExpireDelaySeconds);
                    Item.ScheduledTime = scheduledTime;
                }


                dataContext.Commit();
            }



        }


        public void Complete()
        {
            using (var context = new DataAccessContext())
            {
                DateTime now = Platform.Time;

                if (now < Item.ExpirationTime)
                {
                    Item.ScheduledTime = now.AddSeconds(WorkItemServiceSettings.Instance.PostponeSeconds);
                    Item.Status = WorkItemStatusEnum.Idle;
                }
                else
                {
                    Item.ScheduledTime = now;
                    Item.ExpirationTime = now;
                    Item.Status = WorkItemStatusEnum.Complete;
                }

                var broker = context.GetWorkItemBroker();

                broker.Update(Item);
                context.Commit();
            }
        }
    }
}
