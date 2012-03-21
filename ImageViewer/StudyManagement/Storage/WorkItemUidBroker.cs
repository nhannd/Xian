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

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
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

        public void Update(WorkItemUid uid)
        {
            Context.WorkItemUids.Attach(uid);
        }
    }
}
