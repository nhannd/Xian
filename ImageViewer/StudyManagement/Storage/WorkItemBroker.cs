#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Linq;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
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
		/// <returns></returns>
		public IList<WorkItem> GetPendingWorkItems(int n)
		{
			return (from w in this.Context.WorkItems
					where w.Status == WorkItemStatusEnum.Pending
					select w).Take(n).ToList();
		}

        /// <summary>
        /// Gets the specified number of pending work items.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="studyInstanceUid"></param>
        /// <returns></returns>
        public IList<WorkItem> GetWorkItems(WorkItemTypeEnum? type, WorkItemStatusEnum? status, string studyInstanceUid)
        {
            IQueryable<WorkItem> query = from w in Context.WorkItems select w;

            if (type.HasValue)
                query = query.Where(w => w.Type == type.Value);
            if (status.HasValue)
                query = query.Where(w => w.Status == status.Value);
            if (!string.IsNullOrEmpty(studyInstanceUid))
                query = query.Where(w => w.StudyInstanceUid == studyInstanceUid);

            return query.ToList();
        }

        /// <summary>
        /// Get a specific WorkItem
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
		public WorkItem GetWorkItem(int oid)
		{
			return this.Context.WorkItems.First(w => w.Oid == oid);
		}

        /// <summary>
        /// Get a pending WorkItem of a specific type for a specific study.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="studyInstanceUid"></param>
        /// <returns></returns>
        public WorkItem GetPendingWorkItemForStudy(WorkItemTypeEnum type, string studyInstanceUid)
        {
            return this.Context.WorkItems.First(w => w.StudyInstanceUid == studyInstanceUid 
                && w.Type == type 
                && w.Status != WorkItemStatusEnum.Complete
                && w.Status != WorkItemStatusEnum.Deleted
                && w.Status != WorkItemStatusEnum.Canceled);
        }

        /// <summary>
        /// Insert a WorkItem
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(WorkItem entity)
        {
            Context.WorkItems.InsertOnSubmit(entity);
        }  
	}
}
