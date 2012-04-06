#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Dicom.Core.Command
{
    /// <summary>
    /// Insert a <see cref="WorkItem"/> and <see cref="WorkItemUid"/> entry for a specific SOP Instance UID.  
    /// </summary>
    /// <remarks>
    /// <para>
    /// A <see cref="WorkItem"/> is inserted if it doesn't exist already for the Study.  Otherwise, the <see cref="WorkItemUid"/>
    /// is only inserted.
    /// </para>
    /// </remarks>
    public class InsertWorkItemCommand : DataAccessCommand
    {
        private readonly WorkItemRequest _request;
        private readonly string _studyInstanceUid;

        public WorkItemUid WorkItemUid { get; set; }

        public WorkItem WorkItem { get; set; }

        public InsertWorkItemCommand(WorkItemRequest request, string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid) : base("Insert a WorkItem")
        {
            _request = request;
            _studyInstanceUid = studyInstanceUid;

            WorkItemUid = new WorkItemUid
            {
                Complete = false,
                FailureCount = 0,
                SeriesInstanceUid = seriesInstanceUid,
                SopInstanceUid = sopInstanceUid,
                Failed = false
            };
        }

        public InsertWorkItemCommand(WorkItem item, string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid)
            : base("Insert a WorkItem")
        {
            _request = item.Request;
            _studyInstanceUid = studyInstanceUid;

            WorkItem = item;

            WorkItemUid = new WorkItemUid
            {
                Complete = false,
                FailureCount = 0,
                WorkItemOid = WorkItem.Oid,
                SeriesInstanceUid = seriesInstanceUid,
                SopInstanceUid = sopInstanceUid,
                Failed = false
            };

        }


        public InsertWorkItemCommand(WorkItemRequest request, string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid, string filename)
            : base("Insert a WorkItem")
        {
            _request = request;
            _studyInstanceUid = studyInstanceUid;

            WorkItemUid = new WorkItemUid
            {
                Complete = false,
                FailureCount = 0,
                SeriesInstanceUid = seriesInstanceUid,
                SopInstanceUid = sopInstanceUid,
                Failed = false,
                File = filename
            };
        }

        public InsertWorkItemCommand(WorkItem item, string studyInstanceUid, string seriesInstanceUid, string sopInstanceUid, string filename)
            : base("Insert a WorkItem")
        {
            _request = item.Request;
            _studyInstanceUid = studyInstanceUid;

            WorkItem = item;

            WorkItemUid = new WorkItemUid
            {
                Complete = false,
                FailureCount = 0,
                WorkItemOid = WorkItem.Oid,
                SeriesInstanceUid = seriesInstanceUid,
                SopInstanceUid = sopInstanceUid,
                Failed = false,
                File = filename
            };

        }

        protected override void OnExecute(CommandProcessor theProcessor)
        {
            var workItemBroker = DataAccessContext.GetWorkItemBroker();

            DateTime now = Platform.Time;

            if (WorkItem != null)
            {
                // Already have a committed WorkItem, just set the Oid
                WorkItemUid.WorkItemOid = WorkItem.Oid;
                //WorkItem.ExpirationTime = Platform.Time.AddSeconds(WorkItemServiceSettings.Instance.)
                var workItemUidBroker = DataAccessContext.GetWorkItemUidBroker();
                workItemUidBroker.AddWorkItemUid(WorkItemUid);
            }
            else
            {
                WorkItem = workItemBroker.GetPendingWorkItemForStudy(_request.Type, _studyInstanceUid);
                if (WorkItem == null)
                {
                    WorkItem = new WorkItem
                                   {
                                       InsertTime = now,
                                       Request = _request,
                                       ScheduledTime = now.AddSeconds(5),
                                       Priority = _request.Priority,
                                       Type = _request.Type,
                                       DeleteTime = now.AddHours(2),
                                       ExpirationTime = now.AddMinutes(2),
                                       StudyInstanceUid = _studyInstanceUid,
                                   };

                    workItemBroker.AddWorkItem(WorkItem);
                    WorkItemUid.WorkItem = WorkItem;

                    var workItemUidBroker = DataAccessContext.GetWorkItemUidBroker();
                    workItemUidBroker.AddWorkItemUid(WorkItemUid);
                }
                else
                {
                    WorkItemUid.WorkItemOid = WorkItem.Oid;
                    var workItemUidBroker = DataAccessContext.GetWorkItemUidBroker();
                    workItemUidBroker.AddWorkItemUid(WorkItemUid);
                }
            }
        }
    }
}
