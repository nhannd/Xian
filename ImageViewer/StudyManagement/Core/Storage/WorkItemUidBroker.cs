#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Linq;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{

    public class WorkItemUidBroker : Broker
    {
        internal WorkItemUidBroker(DicomStoreDataContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Gets the specified number of pending work items.
        /// </summary>
        /// <param name="workItemOid"></param>
        /// <returns></returns>
        public IList<WorkItemUid> GetWorkItemUidsForWorkItem(long workItemOid)
        {
            return (from w in Context.WorkItemUids
                    where w.WorkItemOid == workItemOid
                    select w).ToList();
        }

        /// <summary>
        /// Gets the specified number of pending work items.
        /// </summary>
        /// <param name="workItemOid"></param>
        /// <param name="seriesInstanceUid"></param>
        /// <param name="sopInstanceUid"> </param>
        /// <returns></returns>
        public bool HasWorkItemUidForSop(long workItemOid, string seriesInstanceUid, string sopInstanceUid)
        {
            return (from w in Context.WorkItemUids
                    where w.WorkItemOid == workItemOid
                          && w.SeriesInstanceUid == seriesInstanceUid
                          && w.SopInstanceUid == sopInstanceUid
                    select w).Count() > 1;
        }


        /// <summary>
        /// Get a specific WorkItemUid
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public WorkItemUid GetWorkItemUid(long oid)
        {
            var list = (from w in this.Context.WorkItemUids
                        where w.Oid == oid
                        select w).ToList();

            if (!list.Any()) return null;

            return list.First();
        }

        /// <summary>
        /// Insert a WorkItemUid
        /// </summary>
        /// <param name="entity"></param>
        public void AddWorkItemUid(WorkItemUid entity)
        {
            Context.WorkItemUids.InsertOnSubmit(entity);
        }

        /// <summary>
        /// Delete WorkItemUid entity.
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(WorkItemUid entity)
        {
            this.Context.WorkItemUids.DeleteOnSubmit(entity);
        }
    }
}
