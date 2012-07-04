#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
    public static class WorkItemQueryExtensions
    {
        public static IQueryable<WorkItem> WhereIsActive(this IQueryable<WorkItem> query)
        {
            // TODO (CR Jul 2012): Is Pending active? Is this WhereNotTerminated?
            query = query.Where(w => w.Status == WorkItemStatusEnum.Pending
                                     || w.Status == WorkItemStatusEnum.Idle
                                     || w.Status == WorkItemStatusEnum.InProgress
                                     || w.Status == WorkItemStatusEnum.Canceling);
            return query;
        }

        // TODO (CR Jul 2012): WhereWaitingToProcess?
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
        #region Static Exclusive WorkItemRequestTypes
        public static string[] ExclusiveWorkItemRequestTypes = GetExclusiveWorkItemTypes();

        private static string[] GetExclusiveWorkItemTypes()
        {
            try
            {
                // build the contract map by finding all types having a T attribute
                var types = (from p in Platform.PluginManager.Plugins
                             from t in p.Assembly.GetTypes()
                             let a = AttributeUtils.GetAttribute<WorkItemRequestAttribute>(t)
                             where (a != null)
                             select t).ToList();

                var requestTypes = new List<string>();

                foreach (Type t in types)
                {
                    var dataObject = Activator.CreateInstance(t,
                                                              BindingFlags.Public | BindingFlags.NonPublic |
                                                              BindingFlags.Instance, null, null, null);
                    var request = dataObject as WorkItemRequest;
                    if (request != null && request.ConcurrencyType == WorkItemConcurrency.Exclusive)
                        requestTypes.Add(request.WorkItemType);
                }
                return requestTypes.ToArray();
            }
            catch (Exception)
            {
                return new[] {ReindexRequest.WorkItemTypeString};
            }
        }

	    #endregion

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
            // TODO (CR Jul 2012): Should this still order by priority, even though the only caller of this method knows there's only Normal priority items?
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
	    /// Get the WorkItems scheduled before <paramref name="scheduledTime"/> for <paramref name="studyInstanceUid"/> or that are a higher priority
	    /// and eligible to be run. 
	    /// </summary>
	    /// <param name="scheduledTime">The scheduled time to get related WorkItems for.</param>
	    /// <param name="priority">The priority of the workitem to compare with </param>
	    /// <param name="studyInstanceUid">The Study Instance UID to search for matching WorkItems.  Can be null.</param>
	    /// <returns></returns>
	    public IEnumerable<WorkItem> GetWorkItemsScheduledBeforeOrHigherPriority(DateTime scheduledTime, WorkItemPriorityEnum priority,
             string studyInstanceUid)
        {
            IQueryable<WorkItem> query = from w in Context.WorkItems select w;

            query = query.Where(w => w.ScheduledTime < DateTime.Now );
            query = query.Where(w => (w.ScheduledTime < scheduledTime && w.Priority <= priority) || w.Priority < priority);
	        query = query.WhereIsActive();      

            if (!string.IsNullOrEmpty(studyInstanceUid))
                query = query.Where(w => w.StudyInstanceUid == studyInstanceUid);

            query = query.OrderBy(w => w.ScheduledTime);
            query = query.OrderBy(w => w.Priority);
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

        public IEnumerable<WorkItem> GetExclusiveWorkItemsScheduledBeforeOrHigherPriority(DateTime scheduledTime, WorkItemPriorityEnum priority)
	    {
            IQueryable<WorkItem> query = from w in Context.WorkItems select w;
            query = query.Where(w => ExclusiveWorkItemRequestTypes.Contains(w.Type));
            query = query.Where(w => w.ScheduledTime < DateTime.Now);
            query = query.Where(w => w.ScheduledTime < scheduledTime || w.Priority < priority);
            query = query.WhereIsActive();
            
            query = query.OrderBy(w => w.Priority);
            query = query.OrderBy(w => w.ScheduledTime);
            return query.AsEnumerable();
	    }
    }
}
