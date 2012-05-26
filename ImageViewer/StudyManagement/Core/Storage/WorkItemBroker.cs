#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
	public class WorkItemBroker : Broker
	{
		internal WorkItemBroker(DicomStoreDataContext context)
			: base(context)
		{
		}

	    /// <summary>
	    /// Gets the specified number of pending work items.
	    /// </summary>
	    /// <param name="n"></param>
	    /// <param name="priority"> </param>
	    /// <returns></returns>
	    public List<WorkItem> GetPendingWorkItemsByPriority(int n, WorkItemPriorityEnum priority)
        {
            return (from w in this.Context.WorkItems
                    where (w.Status == WorkItemStatusEnum.Pending
                           || w.Status == WorkItemStatusEnum.Idle)
                          && w.ScheduledTime < DateTime.Now
                          && w.Priority == priority
                    orderby w.ScheduledTime ascending
                    select w).Take(n).ToList();
        }

        /// <summary>
        /// Gets WorkItems to delete.
        /// </summary>
        /// <returns></returns>
        public List<WorkItem> GetWorkItemsToDelete(int n)
        {
            return (from w in this.Context.WorkItems
                    where (w.Status == WorkItemStatusEnum.Complete)
                          && w.DeleteTime < DateTime.Now
                    select w).Take(n).ToList();
        }

        /// <summary>
        /// Gets WorkItems marked as deleted.
        /// </summary>
        /// <returns></returns>
        public List<WorkItem> GetWorkItemsDeleted(int n)
        {
            return (from w in this.Context.WorkItems
                    where (w.Status == WorkItemStatusEnum.Deleted)                          
                    select w).Take(n).ToList();
        }

        /// <summary>
        /// Gets the specified number of pending work items.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public List<WorkItem> GetPendingWorkItems(int n)
        {
            return (from w in this.Context.WorkItems
                    where (w.Status == WorkItemStatusEnum.Pending
                           || w.Status == WorkItemStatusEnum.Idle)
                          && w.ScheduledTime < DateTime.Now
                    orderby w.ScheduledTime ascending
                    select w).Take(n).ToList();
        }

        /// <summary>
        /// General the WorkItems with the specified parameters.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="studyInstanceUid"></param>
        /// <returns></returns>
        public IEnumerable<WorkItem> GetWorkItems(string type, WorkItemStatusEnum? status, string studyInstanceUid)
        {
            IQueryable<WorkItem> query = from w in Context.WorkItems select w;
            
            if (!string.IsNullOrEmpty(type))
                query = query.Where(w => w.Type == type);

            if (status.HasValue)
                query = query.Where(w => w.Status == status.Value);
            else
            {
                query = query.Where(w => w.Status != WorkItemStatusEnum.Deleted);
                query = query.Where(w => w.Status != WorkItemStatusEnum.DeleteInProgress);
            }

            if (!string.IsNullOrEmpty(studyInstanceUid))
                query = query.Where(w => w.StudyInstanceUid == studyInstanceUid);

            query = query.OrderBy(w => w.ScheduledTime);

            return query.AsEnumerable();
        }

        /// <summary>
        /// General the WorkItems with the specified parameters.
        /// </summary>
        /// <param name="scheduledTime"></param>
        /// <param name="prioritiesToBlock"></param>
        /// <param name="studyInstanceUid"></param>
        /// <returns></returns>
        public IEnumerable<WorkItem> GetPriorWorkItems(DateTime scheduledTime,
            IEnumerable<WorkItemPriorityEnum> prioritiesToBlock, string studyInstanceUid)
        {
            IQueryable<WorkItem> query = from w in Context.WorkItems select w;

            query = query.Where(w => w.ScheduledTime < scheduledTime);

            query = query.Where(w => w.Status != WorkItemStatusEnum.Deleted);
            query = query.Where(w => w.Status != WorkItemStatusEnum.DeleteInProgress);
            query = query.Where(w => w.Status != WorkItemStatusEnum.Canceled);
            query = query.Where(w => w.Status != WorkItemStatusEnum.Canceling);
            query = query.Where(w => w.Status != WorkItemStatusEnum.Complete);
            query = query.Where(w => w.Status != WorkItemStatusEnum.Failed);

            foreach (var priority in prioritiesToBlock)
            {
                WorkItemPriorityEnum priority1 = priority;
                query = query.Where(w => w.Priority != priority1);
            }

            if (!string.IsNullOrEmpty(studyInstanceUid))
                query = query.Where(w => w.StudyInstanceUid == studyInstanceUid);
  
            query = query.OrderBy(w => w.ScheduledTime);

            return query.AsEnumerable();
        }


	    /// <summary>
        /// Get a specific WorkItem
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
		public WorkItem GetWorkItem(long oid)
		{
            var list = (from w in this.Context.WorkItems
                        where w.Oid == oid
                        select w).ToList();

            if (!list.Any()) return null;

            return list.First();		
		}

        /// <summary>
        /// Get a pending WorkItem of a specific type for a specific study.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="studyInstanceUid"></param>
        /// <returns></returns>
        public IList<WorkItem> GetPendingWorkItemForStudy(string type, string studyInstanceUid)
        {
            var list = (from w in this.Context.WorkItems
                        where w.StudyInstanceUid == studyInstanceUid
                              && w.Type == type
                              && w.Status != WorkItemStatusEnum.Complete
                              && w.Status != WorkItemStatusEnum.Deleted
                              && w.Status != WorkItemStatusEnum.Canceled
                        select w).ToList();
            
            if (!list.Any()) return null;

            return list;
        }

        /// <summary>
        /// Insert a WorkItem
        /// </summary>
        /// <param name="entity"></param>
        public void AddWorkItem(WorkItem entity)
        {
            Context.WorkItems.InsertOnSubmit(entity);
        }

        /// <summary>
        /// Delete WorkItemUid entity.
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(WorkItem entity)
        {
            this.Context.WorkItems.DeleteOnSubmit(entity);
        }
	}
}
