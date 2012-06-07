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

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
	public enum WorkItemsChangedEventType
	{
		/// <summary>
		/// The event was raised because the work item(s) have been updated.
		/// </summary>
		Update,

		/// <summary>
		/// The event was raised in response to an explicit refresh request.
		/// </summary>
		Refresh
	}

    public class WorkItemsChangedEventArgs : EventArgs
    {
        public WorkItemsChangedEventArgs(WorkItemsChangedEventType eventType, List<WorkItemData> items)
        {
			Platform.CheckForNullReference(items, "items");

			ChangedItems = items;
        	EventType = eventType;
        }

		public WorkItemsChangedEventType EventType { get; private set; }

		public List<WorkItemData> ChangedItems { get; private set; }
    }
}