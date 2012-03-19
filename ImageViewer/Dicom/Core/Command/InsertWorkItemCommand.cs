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
    public class InsertWorkItemCommand : CommandBase
    {
        private WorkItemRequest _request;

        public WorkItemUid WorkItemUid { get; set; }

        public WorkItem WorkItem { get; set; }

        public InsertWorkItemCommand(WorkItemRequest request, string seriesInstanceUid, string sopInstanceUid) : base("Insert a WorkItem", true)
        {
            _request = request;

            WorkItemUid = new WorkItemUid
            {
                Complete = false,
                FailureCount = 0,
                WorkItemOid = WorkItem.Oid,
                SeriesInstanceUid = seriesInstanceUid,
                SopInstanceUid = sopInstanceUid
            };
        }

        public InsertWorkItemCommand(WorkItem item, string seriesInstanceUid, string sopInstanceUid)
            : base("Insert a WorkItem", true)
        {
            _request = item.Request;
            
            WorkItem = item;

            WorkItemUid = new WorkItemUid
            {
                Complete = false,
                FailureCount = 0,
                WorkItemOid = WorkItem.Oid,
                SeriesInstanceUid = seriesInstanceUid,
                SopInstanceUid = sopInstanceUid
            };

        }


        public InsertWorkItemCommand(WorkItemRequest request, string seriesInstanceUid, string sopInstanceUid, string filename)
            : base("Insert a WorkItem", true)
        {
            _request = request;

            WorkItemUid = new WorkItemUid
            {
                Complete = false,
                FailureCount = 0,
                WorkItemOid = WorkItem.Oid,
                SeriesInstanceUid = seriesInstanceUid,
                SopInstanceUid = sopInstanceUid
            };
        }

        public InsertWorkItemCommand(WorkItem item, string seriesInstanceUid, string sopInstanceUid, string filename)
            : base("Insert a WorkItem", true)
        {
            _request = item.Request;

            WorkItem = item;

            WorkItemUid = new WorkItemUid
            {
                Complete = false,
                FailureCount = 0,
                WorkItemOid = WorkItem.Oid,
                SeriesInstanceUid = seriesInstanceUid,
                SopInstanceUid = sopInstanceUid
            };

        }

        protected override void OnExecute(CommandProcessor theProcessor)
        {
            using (var scope = new DataAccessScope())
            {
                var workItemUidBroker = scope.GetWorkItemUidBroker();

                var workItemBroker = scope.GetWorkItemBroker();
                
                DateTime now = Platform.Time;

                if (WorkItem == null)
                {

                    WorkItem = new WorkItem()
                                   {
                                       InsertTime = now,
                                       Request = _request,
                                       ScheduledTime = now.AddSeconds(5),
                                       Priority = _request.Priority,
                                       Type = _request.Type,
                                       DeleteTime = now.AddHours(3),
                                       ExpirationTime = now.AddMinutes(2),
                                   };
                    //Insert

                    // update WOrkItemUid.WorkItemOid
                }
                else
                {
                    WorkItem.ExpirationTime = now.AddMinutes(2);
                    //Update
                }

           
                workItemUidBroker.Insert(WorkItemUid);
                
            }
        }

        protected override void OnUndo()
        {
            // Handle automatically via a rollback
        }
    }
}
