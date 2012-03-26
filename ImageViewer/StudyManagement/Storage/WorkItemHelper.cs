#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
    public static class WorkItemHelper
    {
        public static WorkItemData FromWorkItem(WorkItem item)
        {
            return new WorkItemData
                       {
                           Type = item.Type,
                           DeleteTime = item.DeleteTime,
                           ExpirationTime = item.ExpirationTime,
                           FailureCount = 0,
                           Identifier = item.Oid,
                           InsertTime = item.InsertTime,
                           Priority = item.Priority,
                           StudyInstanceUid = item.StudyInstanceUid,
                           Request = item.Request,
                           Progress = item.Progress
                       };
        }
    }
}
