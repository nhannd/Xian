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
    internal class WorkItemQuery : IDisposable
    {
        #region Private Members

        private readonly DataAccessContext _context;
      
        #endregion

        #region Contructors

        private WorkItemQuery()
        {
            _context = new DataAccessContext(DataAccessContext.WorkItemMutex);
        }

        #endregion


        #region Public Static Methods

        public static List<WorkItem> GetWorkItems(int count, WorkItemPriorityEnum priority)
        {
            using (var query = new WorkItemQuery())
            {
                return query.InternalGetWorkItems(count, priority);
            }
        }

#if UNIT_TESTS
        public static bool UnitTestCanStart(WorkItem item, out string reason)
        {
            using (var query = new WorkItemQuery())
            {
                return query.CanStart(item, out reason);
            }
        }
#endif


        #endregion

        #region Public Methods

        public void Dispose()
        {
            _context.Dispose();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Method for getting next <see cref="WorkItem"/> entry.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="priority">Search for stat items.</param>
        /// <remarks>
        /// </remarks>
        /// <returns>
        /// A <see cref="WorkItem"/> entry if found, or else null;
        /// </returns>
        private List<WorkItem> InternalGetWorkItems(int count, WorkItemPriorityEnum priority)
        {
            var returnedItems = new List<WorkItem>();
            var itemsToPublish = new List<WorkItemData>();
            try
            {
                var workItemBroker = _context.GetWorkItemBroker();

                List<WorkItem> workItems;
                if (priority == WorkItemPriorityEnum.Stat)
                    workItems = workItemBroker.GetWorkItemsForProcessing(count*4, priority);
                else if (priority == WorkItemPriorityEnum.High)
                    workItems = workItemBroker.GetWorkItemsForProcessing(count * 4, priority);
                else
                    workItems = workItemBroker.GetWorkItemsForProcessing(count*4);

                foreach (var item in workItems)
                {
                    string reason;
                    if (CanStart(item, out reason))
                    {
                        // TODO (CR Jul 2012): Do subsequent queries see this item as "In Progress"?
                        item.Status = WorkItemStatusEnum.InProgress;
                        returnedItems.Add(item);
                    }
                    else
                    {
                        Postpone(item);
                        WorkItemProgress progress = item.Progress;
                        if (progress != null)
                        {
                            progress.StatusDetails = reason;
                            item.Progress = progress;
                            itemsToPublish.Add(WorkItemDataHelper.FromWorkItem(item));
                        }
                    }

                    if (returnedItems.Count >= count)
                        break;
                }

                _context.Commit();

                return returnedItems;
            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Warn, x, "Unexpected error querying for {0} {1} priority WorkItems", count,
                             priority.GetDescription());
                return null;
            }
            finally
            {
                if (itemsToPublish.Count > 0)
                {
                    WorkItemPublishSubscribeHelper.PublishWorkItemsChanged(WorkItemsChangedEventType.Update,
                                                                           itemsToPublish);
                }
            }
        }

        private bool CanStart(WorkItem item, out string reason)
        {
            if (item.Request.ConcurrencyType == WorkItemConcurrency.NonExclusive)
                return CanStartNonExclusive(item, out reason);

            if (item.Request.ConcurrencyType == WorkItemConcurrency.StudyUpdate)
                return CanStartStudyUpdate(item, out reason);

            if (item.Request.ConcurrencyType == WorkItemConcurrency.StudyDelete)
                return CanStartStudyDelete(item, out reason);

            if (item.Request.ConcurrencyType == WorkItemConcurrency.StudyRead)
                return CanStartStudyRead(item, out reason);

            return CanStartExclusive(item, out reason);
        }

        private bool CanStartNonExclusive(WorkItem workItem, out string reason)
        {
            if (ExclusiveInProgressWorkItem(out reason))
                return false;

            var competingList = GetCompetingWorkItems(workItem);
            if (competingList != null)
            {
                foreach (var competingWorkItem in competingList)
                {
                    // Block for Exclusive concurrency types scheduled ahead of us
                    if (competingWorkItem.Request.ConcurrencyType == WorkItemConcurrency.Exclusive)
                    {
                        reason = string.Format("Waiting for: {0}", competingWorkItem.Request.ActivityDescription);
                        return false;
                    }
                }
            }

            return true;
        }

        private bool CanStartStudyUpdate(WorkItem workItem, out string reason)
        {
            // TODO (CR Jul 2012): Only in progress, or anything not pending or terminated?
            var relatedList = GetInProgressWorkItemsForStudy(workItem);
            if (relatedList != null)
            {
                foreach (var relatedWorkItem in relatedList)
                {
                    // Don't block on StudyReads, so we can process a study at the same time its coming over the network.
                    if (relatedWorkItem.Request.ConcurrencyType == WorkItemConcurrency.StudyRead)
                        continue;

                    reason = string.Format("Waiting for: {0}", relatedWorkItem.Request.ActivityDescription);
                    return false;
                }
            }
            
            var competingList = GetCompetingWorkItems(workItem);
            if (competingList != null)
            {
                foreach (var competingWorkItem in competingList)
                {
                    // Study Updates/Reads should only block if "In Progress", otherwise they can run at the same time.
                    if (!competingWorkItem.Status.IsInProgress() && competingWorkItem.Request.ConcurrencyType == WorkItemConcurrency.StudyUpdate)
                        continue;
                    // Don't block on StudyReads, so we can process a study at the same time its coming over the network.
                    if (competingWorkItem.Request.ConcurrencyType == WorkItemConcurrency.StudyRead)
                        continue;

                    reason = string.Format("Waiting for: {0}", competingWorkItem.Request.ActivityDescription);
                    return false;
                }
            }

            // TODO (CR Jul 2012): Only in progress, or anything not pending or terminated?
            if (ExclusiveInProgressWorkItem(out reason))
                return false;

            if (ExclusiveCompetingWorkItem(workItem, out reason))
                return false;

            return true;
        }

        private bool CanStartStudyDelete(WorkItem workItem, out string reason)
        {
            // TODO (CR Jul 2012): Only in progress, or anything not pending or terminated?
            var relatedList = GetInProgressWorkItemsForStudy(workItem);
            if (relatedList != null)
            {
                foreach (var relatedWorkItem in relatedList)
                {
                    reason = string.Format("Waiting for: {0}", relatedWorkItem.Request.ActivityDescription);
                    return false;
                }
            }

            var competingList = GetCompetingWorkItems(workItem);
            if (competingList != null)
            {
                foreach (var competingItem in competingList)
                {
                    reason = string.Format("Waiting for: {0}", competingItem.Request.ActivityDescription);
                    return false;
                }
            }

            // TODO (CR Jul 2012): Only in progress, or anything not pending or terminated?
            if (ExclusiveInProgressWorkItem(out reason))
                return false;

            if (ExclusiveCompetingWorkItem(workItem, out reason))
                return false;

            return true;
        }

        private bool CanStartStudyRead(WorkItem workItem, out string reason)
        {
            // TODO (CR Jul 2012): Only in progress, or anything not pending or terminated?
            var relatedList = GetInProgressWorkItemsForStudy(workItem);
            if (relatedList != null)
            {
                foreach (var relatedWorkItem in relatedList)
                {
                    if (relatedWorkItem.Request.ConcurrencyType == WorkItemConcurrency.StudyRead) continue;

                    reason = string.Format("Waiting for: {0}", relatedWorkItem.Request.ActivityDescription);
                    return false;
                }
            }

            var competingList = GetCompetingWorkItems(workItem);
            if (competingList != null)
            {
                foreach (var competingItem in competingList)
                {
                    if (competingItem.Request.ConcurrencyType == WorkItemConcurrency.StudyRead)
                        continue;
                    reason = string.Format("Waiting for: {0}", competingItem.Request.ActivityDescription);
                    return false;
                }
            }

            // TODO (CR Jul 2012): Only in progress, or anything not pending or terminated?
            if (ExclusiveInProgressWorkItem(out reason))
                return false;

            if (ExclusiveCompetingWorkItem(workItem, out reason))
                return false;

            return true;
        }

        private bool CanStartExclusive(WorkItem workItem, out string reason)
        {
            var relatedList = GetCompetingWorkItems(workItem);
            if (relatedList != null)
            {
                foreach (var relatedWorkItem in relatedList)
                {
                    reason = string.Format("Waiting for: {0}", relatedWorkItem.Request.ActivityDescription);
                    return false;
                }
            }

            // TODO (CR Jul 2012): Only in progress, or anything not pending or terminated?
            return !AnyInProgressWorkItems(out reason);
        }

        /// <summary>
        /// Postpone a <see cref="WorkItem"/>
        /// </summary>
        private void Postpone(WorkItem item)
        {
            DateTime now = Platform.Time;

            var timeWindowRequest = item.Request as IWorkItemRequestTimeWindow;

            if (timeWindowRequest != null && item.Request.Priority != WorkItemPriorityEnum.Stat)
            {
                DateTime scheduledTime = timeWindowRequest.GetScheduledTime(now,
                                                                            WorkItemServiceSettings.Default.
                                                                                PostponeSeconds);
                item.ProcessTime = scheduledTime;
                item.ScheduledTime = scheduledTime;
            }
            else
            {
                item.ProcessTime = now.AddSeconds(WorkItemServiceSettings.Default.PostponeSeconds);
            }

            if (item.ProcessTime > item.ExpirationTime)
                item.ExpirationTime = item.ProcessTime;
        }

        /// <summary>
        /// Returns a list of <see cref="WorkItem"/>s with specified types and status (both are optional)
        /// that are scheduled before the <see cref="WorkItem"/> being processed.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<WorkItem> GetCompetingWorkItems(WorkItem item)
        {
            var broker = _context.GetWorkItemBroker();

            var list = broker.GetWorkItemsScheduledBeforeOrHigherPriority(item.ScheduledTime, item.Priority, item.StudyInstanceUid);

            if (list == null)
                return null;
       
            return list;
        }

        /// <summary>
        /// Returns true if there are any WorkItemStatusEnum.InProgress work items with <see cref="WorkItemConcurrency.Exclusive"/>.
        /// </summary>
        /// <returns></returns>
        private bool ExclusiveInProgressWorkItem(out string reason)
        {
            reason = string.Empty;

            var broker = _context.GetWorkItemBroker();

            var list = broker.GetWorkItems(null, WorkItemStatusEnum.InProgress, null);

            if (list == null)
                return false;

            foreach (var item in list)
            {
                if (item.Request.ConcurrencyType == WorkItemConcurrency.Exclusive)
                {
                    reason = string.Format("Waiting for: {0}", item.Request.ActivityDescription);
                    return true;
                }
            }

            return false;
        }

        private bool ExclusiveCompetingWorkItem(WorkItem workItem, out string reason)
        {
            reason = string.Empty;

            var broker = _context.GetWorkItemBroker();

            var list = broker.GetWorkItemsScheduledBeforeOrHigherPriority(workItem.ScheduledTime, workItem.Priority, null, WorkItemConcurrency.Exclusive);

            if (list == null)
                return false;

            foreach (var item in list)
            {
                if (item.Request.ConcurrencyType == WorkItemConcurrency.Exclusive)
                {
                    reason = string.Format("Waiting for: {0}", item.Request.ActivityDescription);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if there are any WorkItemStatusEnum.InProgress work items.
        /// </summary>
        /// <returns></returns>
        private bool AnyInProgressWorkItems(out string reason)
        {
            reason = string.Empty;

            var broker = _context.GetWorkItemBroker();

            var list = broker.GetWorkItems(null, WorkItemStatusEnum.InProgress, null);

            if (list == null)
                return false;

            foreach (var item in list)
            {
                reason = string.Format("Waiting for: {0}", item.Request.ActivityDescription);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if there are any WorkItemStatusEnum.InProgress work items.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<WorkItem> GetInProgressWorkItemsForStudy(WorkItem workItem)
        {
           
            var broker = _context.GetWorkItemBroker();

            var list = broker.GetWorkItems(null, WorkItemStatusEnum.InProgress, workItem.StudyInstanceUid);

            if (list == null)
                return null;
            
            return list;
        }


        #endregion
    }
}
