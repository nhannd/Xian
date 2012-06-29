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
                    workItems = workItemBroker.GetWorkItemsForProcessingByPriority(count * 4, priority);
                else if (priority == WorkItemPriorityEnum.High)
                    workItems = workItemBroker.GetWorkItemsForProcessingByPriority(count * 4, priority);
                else
                    workItems = workItemBroker.GetWorkItemsForProcessing(count * 4);

                foreach (var item in workItems)
                {
                    string reason;
                    if (CanStart(item, out reason))
                    {
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
                    WorkItemPublishSubscribeHelper.PublishWorkItemsChanged(WorkItemsChangedEventType.Update, itemsToPublish);
                }   
            }
        }

        private bool CanStart(WorkItem item, out string reason)
        {
            if (item.Request.ConcurrencyType == WorkItemConcurrency.Free)
            {
                reason = string.Empty;
                return true;
            }

            if (item.Request.ConcurrencyType == WorkItemConcurrency.StudyInsert)
                return CanStartStudyInsert(item, out reason);

            if (item.Request.ConcurrencyType == WorkItemConcurrency.StudyDelete)
                return CanStartStudyDelete(item, out reason);

            if (item.Request.ConcurrencyType == WorkItemConcurrency.StudyRead)
                return CanStartStudyRead(item, out reason);

            // Blocking Concurrency Type here
            return CanStartBlocking(out reason);
        }

        private bool CanStartStudyInsert(WorkItem workItem, out string reason)
        {
            var relatedList = GetCompetingWorkItems(workItem);
            if (relatedList != null)
            {
                foreach (var relatedWorkItem in relatedList)
                {
                    if (relatedWorkItem.Request.ConcurrencyType == WorkItemConcurrency.StudyDelete
                     || relatedWorkItem.Request.ConcurrencyType == WorkItemConcurrency.StudyInsert)
                    {
                        reason = string.Format("Waiting for: {0}",
                                                     relatedWorkItem.Request.ActivityDescription);
                        return false;
                    }
                }
            }

            if (BlockingInProgressWorkItem(out reason))
                return false;

            return true;
        }

        private bool CanStartStudyDelete(WorkItem workItem, out string reason)
        {
            var relatedList = GetCompetingWorkItems(workItem);
            if (relatedList != null)
            {
                foreach (var relatedWorkItem in relatedList)
                {
                    if (relatedWorkItem.Request.ConcurrencyType != WorkItemConcurrency.Blocking)
                    {
                        reason = string.Format("Waiting for: {0}", relatedWorkItem.Request.ActivityDescription);
                        return false;
                    }
                }
            }

            if (BlockingInProgressWorkItem(out reason))
                return false;

            return true;
        }

        private bool CanStartStudyRead(WorkItem workItem, out string reason)
        {
            var relatedList = GetCompetingWorkItems(workItem);
            if (relatedList != null)
            {
                foreach (var relatedWorkItem in relatedList)
                {
                    if (relatedWorkItem.Request.ConcurrencyType == WorkItemConcurrency.StudyDelete
                        || relatedWorkItem.Request.ConcurrencyType == WorkItemConcurrency.StudyInsert)
                    {
                        reason = string.Format("Waiting for: {0}", relatedWorkItem.Request.ActivityDescription);
                        return false;
                    }
                }
            }

            if (BlockingInProgressWorkItem(out reason))
                return false;

            return true;
        }

        private bool CanStartBlocking(out string reason)
        {
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
                DateTime scheduledTime = timeWindowRequest.GetScheduledTime(now, WorkItemServiceSettings.Default.PostponeSeconds);
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

            var list = broker.GetWorkItemsScheduledBeforeTime(item.ScheduledTime, item.StudyInstanceUid);

            if (list == null)
                return null;

            var newList = new List<WorkItem>();
            newList.AddRange(list);
            return newList;
        }

        /// <summary>
        /// Returns true if there are any WorkItemStatusEnum.InProgress work items with <see cref="WorkItemConcurrency.Blocking"/>.
        /// </summary>
        /// <returns></returns>
        private bool BlockingInProgressWorkItem(out string reason)
        {
            reason = string.Empty;

            var broker = _context.GetWorkItemBroker();

            var list = broker.GetWorkItems(null, WorkItemStatusEnum.InProgress, null);

            if (list == null)
                return false;

            foreach (var item in list)
            {
                if (item.Request.ConcurrencyType == WorkItemConcurrency.Blocking)
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

        #endregion
    }
}
