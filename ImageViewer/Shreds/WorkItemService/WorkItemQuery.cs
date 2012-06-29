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
using ClearCanvas.Common.Utilities;
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
                if (item.Request is ReindexRequest)
                {
                    return query.CanStartReindex(item, out reason);
                }
                return query.CanStartReindex(item, out reason);
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
                    workItems = workItemBroker.GetWorkItemsForProcessingByPriority(count * 3, priority);
                else if (priority == WorkItemPriorityEnum.High)
                    workItems = workItemBroker.GetWorkItemsForProcessingByPriority(count * 3, priority);
                else
                    workItems = workItemBroker.GetWorkItemsForProcessing(count * 3);

                foreach (var item in workItems)
                {
                    string reason;
                    if (item.Request is ReindexRequest)
                    {
                        if (CanStartReindex(item, out reason))
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
                    }
                    else
                    {
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
                    }
                    if (returnedItems.Count >= count)
                        break;
                }

                _context.Commit();

                if (itemsToPublish.Count > 0)
                {
                    WorkItemPublishSubscribeHelper.PublishWorkItemsChanged(WorkItemsChangedEventType.Update, itemsToPublish);
                }

                return returnedItems;

            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Warn, x, "Unexpected error querying for {0} {1} priority WorkItems", count,
                             priority.GetDescription());
                return null;
            }
        }

        private bool CanStartReindex(WorkItem workItem, out string reason)
        {
            if (ScheduledAheadInsertItems(workItem, out reason))
            {
                return false;
            }

            return !AnyInProgressWorkItems(workItem, out reason);
        }

        private bool CanStart(WorkItem item, out string reason)
        {

            if (item.Request.ConcurrencyType == WorkItemConcurrency.StudyInsert)
            {
                var relatedList = GetCompetingWorkItems(item);
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
                
                return !CompetingReindexScheduled(item, out reason);
            }

            if (item.Request.ConcurrencyType == WorkItemConcurrency.StudyDelete)
            {
                var inProgressList = GetCompetingInProgressWorkItems(item);
                if (inProgressList != null)
                {
                    foreach (var relatedWorkItem in inProgressList)
                    {
                        // This check is done because it is possible that 2 entries for the
                        // Same study are entered into the queue, and they both don't run because
                        // of the other entry.  This will allow the one scheduled first to run.
                        if (relatedWorkItem.ScheduledTime < item.ScheduledTime)
                        {
                            reason = string.Format("Waiting for: {0}",
                                                   relatedWorkItem.Request.ActivityDescription);
                            return false;
                        }
                    }
                }

                var relatedList = GetCompetingWorkItems(item);
                if (relatedList != null)
                {
                    foreach (var relatedWorkItem in relatedList)
                    {
                        if (relatedWorkItem.Request.ConcurrencyType != WorkItemConcurrency.NonStudy)
                        {
                            reason = string.Format("Waiting for: {0}",
                                                   relatedWorkItem.Request.ActivityDescription);
                            return false;
                        }
                    }
                }
                return !CompetingReindexScheduled(item, out reason);
            }

            if (item.Request.ConcurrencyType == WorkItemConcurrency.StudyRead)
            {
                var relatedList = GetCompetingWorkItems(item);
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

                return !CompetingReindexScheduled(item, out reason);
            }

            // TODO (CR Jun 2012): I would agree this was true generally, if it weren't for the fact that re-index items are "NonStudy",
            // and the ReindexItemProcessor overrides this method.

            // WorkItemConcurrency.NonStudy entries that haven't overriden this method can just run
            reason = string.Empty;
            return true;
        }

        /// <summary>
        /// Postpone a <see cref="WorkItem"/>
        /// </summary>
        public void Postpone(WorkItem item)
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
            using (var context = new DataAccessContext())
            {
                var broker = context.GetWorkItemBroker();

                var prioritiesToBlock = new List<WorkItemPriorityEnum>();


                if (item.Request.Priority == WorkItemPriorityEnum.Stat)
                {
                    prioritiesToBlock.Add(WorkItemPriorityEnum.High);
                    prioritiesToBlock.Add(WorkItemPriorityEnum.Normal);
                }
                if (item.Request.Priority == WorkItemPriorityEnum.High)
                {
                    prioritiesToBlock.Add(WorkItemPriorityEnum.Normal);
                }
                var list = broker.GetWorkItemsScheduledBeforeTime(item.ScheduledTime, prioritiesToBlock, item.StudyInstanceUid);

                if (list == null)
                    return null;

                var newList = new List<WorkItem>();
                newList.AddRange(list);
                return newList;
            }
        }

        /// <summary>
        /// Returns a list of related <see cref="WorkItem"/>s with specified types and status (both are optional)
        /// that are In Progress and scheduled before the <see cref="WorkItem"/> being processed.
        /// </summary>
        /// <returns></returns>
        private IList<WorkItem> GetCompetingInProgressWorkItems(WorkItem workItem)
        {
            var broker = _context.GetWorkItemBroker();

            var list = broker.GetWorkItems(null, WorkItemStatusEnum.InProgress, workItem.StudyInstanceUid);

            if (list == null)
                return null;

            var newList = new List<WorkItem>();
            foreach (var item in list)
            {
                if (item.Oid != workItem.Oid)
                    newList.Add(item);
            }
            return newList;
        }

        /// <summary>
        /// Checks if a Reindex is scheduled that should prevent the current <see cref="WorkItem"/> from processing.
        /// </summary>
        /// <returns></returns>
        private bool CompetingReindexScheduled(WorkItem workItem, out string reason)
        {
            reason = string.Empty;

            var broker = _context.GetWorkItemBroker();

            var list = broker.GetWorkItems(ReindexRequest.WorkItemTypeString, null, null);

            if (list == null)
                return false;

            var newList = CollectionUtils.Select(list,
                                                 item => item.Status != WorkItemStatusEnum.Complete
                                                         && item.Status != WorkItemStatusEnum.Deleted
                                                         && item.Status != WorkItemStatusEnum.Canceled
                                                         && item.Status != WorkItemStatusEnum.Failed);

            // Study Inserts only wait for reindexes scheduled before itself, not for those after.  All other 
            // WorkItem types wait for any reindex scheduled, whether its scheduled before or after itself.
            if (workItem.Request.ConcurrencyType == WorkItemConcurrency.StudyInsert)
            {
                newList = CollectionUtils.Select(newList,
                                                 item =>
                                                 item.ScheduledTime < workItem.ScheduledTime ||
                                                 item.Status == WorkItemStatusEnum.InProgress);
            }

            foreach (var item in newList)
            {
                reason = string.Format("Waiting for: {0}", item.Request.ActivityDescription);
                break;
            }

            return newList.Count > 0;
        }

        /// <summary>
        /// Returns true if there are any WorkItemStatusEnum.InProgress work items.
        /// </summary>
        /// <returns></returns>
        private bool AnyInProgressWorkItems(WorkItem workItem, out string reason)
        {
            reason = string.Empty;

            var broker = _context.GetWorkItemBroker();

            var list = broker.GetWorkItems(null, WorkItemStatusEnum.InProgress, null);

            if (list == null)
                return false;

            // remove the current item 
            var newList = CollectionUtils.Reject(list, item => item.Oid.Equals(workItem.Oid));

            foreach (var item in newList)
            {
                reason = string.Format("Waiting for: {0}", item.Request.ActivityDescription);
                break;
            }

            return newList.Count > 0;
        }

        private bool ScheduledAheadInsertItems(WorkItem workItem, out string reason)
        {
            reason = string.Empty;

            var broker = _context.GetWorkItemBroker();
            var list = broker.GetWorkItemsScheduledBeforeTime(workItem.ScheduledTime, null, null);

            if (list == null)
                return false;

            foreach (var item in list)
            {
                // TODO (CR Jun 2012 - Med): Does this mean it can run at the same time as a study delete?
                // StudyInserts only wait for reindexes scheduled before themselves.  They will not wait
                // for a reindex after.  So, the reindex must wait until any study insert before it
                // completes.
                // A delete will wait for any re-index scheduled, even ones scheduled after it.
                if (item.Request.ConcurrencyType == WorkItemConcurrency.StudyInsert)
                {
                    reason = string.Format("Waiting for: {0}", item.Request.ActivityDescription);
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
