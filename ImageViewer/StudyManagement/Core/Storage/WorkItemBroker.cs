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
    public static class WorkItemQueryExtensions
    {
        public static IQueryable<WorkItem> WhereIsActive(this  IQueryable<WorkItem> query)
        {
            query = query.Where(w => w.Status == WorkItemStatusEnum.Pending
                                     || w.Status == WorkItemStatusEnum.Idle
                                     || w.Status == WorkItemStatusEnum.InProgress
                                     || w.Status == WorkItemStatusEnum.Canceling);
            return query;
        }

        public static IQueryable<WorkItem> WhereIsPending(this  IQueryable<WorkItem> query)
        {
            query = query.Where(w => w.Status == WorkItemStatusEnum.Pending || w.Status == WorkItemStatusEnum.Idle);
            return query;
        }

        public static IQueryable<WorkItem> WhereNotDeleted(this  IQueryable<WorkItem> query)
        {
            query = query.Where(w => w.Status != WorkItemStatusEnum.Deleted);
            query = query.Where(w => w.Status != WorkItemStatusEnum.DeleteInProgress);
            return query;
        } 
    }

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
	    public List<WorkItem> GetWorkItemsForProcessingByPriority(int n, WorkItemPriorityEnum priority)
        {
            IQueryable<WorkItem> query = from w in Context.WorkItems select w;
            query = query.WhereIsPending();
            query = query.Where(w => w.ProcessTime < DateTime.Now);
            query = query.Where(w => w.Priority == priority);
            query = query.OrderBy(w => w.ProcessTime);
            return query.Take(n).ToList();
        }

        /// <summary>
        /// Gets WorkItems to delete.
        /// </summary>
        /// <returns></returns>
        public List<WorkItem> GetWorkItemsToDelete(int n)
        {
            return (from w in Context.WorkItems
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
            return (from w in Context.WorkItems
                    where (w.Status == WorkItemStatusEnum.Deleted)                          
                    select w).Take(n).ToList();
        }

        /// <summary>
        /// Gets the specified number of pending work items.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public List<WorkItem> GetWorkItemsForProcessing(int n)
        {
            IQueryable<WorkItem> query = from w in Context.WorkItems select w;
            query = query.WhereIsPending();
            query = query.Where(w => w.ProcessTime < DateTime.Now);
            query = query.OrderBy(w => w.ProcessTime);
            return query.Take(n).ToList();
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

            query = status.HasValue 
                ? query.Where(w => w.Status == status.Value) 
                : query.WhereNotDeleted();

            if (!string.IsNullOrEmpty(studyInstanceUid))
                query = query.Where(w => w.StudyInstanceUid == studyInstanceUid);

            query = query.OrderBy(w => w.ProcessTime);

            return query.AsEnumerable();
        }

	    /// <summary>
        /// Get the WorkItems scheduled before <paramref name="scheduledTime"/> for <paramref name="studyInstanceUid"/>. 
        /// </summary>
        /// <param name="scheduledTime">The scheduled time to get related WorkItems for.</param>
        /// <param name="studyInstanceUid">The Study Instance UID to search for matching WorkItems.  Can be null.</param>
        /// <returns></returns>
        public IEnumerable<WorkItem> GetWorkItemsScheduledBeforeTime(DateTime scheduledTime,
             string studyInstanceUid)
        {
            IQueryable<WorkItem> query = from w in Context.WorkItems select w;

            query = query.Where(w => w.ScheduledTime < scheduledTime);

	        query = query.WhereIsActive();      

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
            var list = (from w in Context.WorkItems
                        where w.Oid == oid
                        select w);

            if (!list.Any()) return null;

            return list.First();		
		}

        /// <summary>
        /// Get a pending WorkItem of a specific type for a specific study.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="studyInstanceUid"></param>
        /// <returns></returns>
        public IEnumerable<WorkItem> GetActiveWorkItemsForStudy(string type, string studyInstanceUid)
        {
            IQueryable<WorkItem> query = from w in Context.WorkItems select w;
            query = query.Where(w => w.StudyInstanceUid == studyInstanceUid);
            query = query.Where(w => w.Type == type);
            query = query.WhereIsActive();
            return query.AsEnumerable();
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
            Context.WorkItems.DeleteOnSubmit(entity);
        }
	}
}
