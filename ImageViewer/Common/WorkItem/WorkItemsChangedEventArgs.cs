using System;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    public class WorkItemsChangedEventArgs : EventArgs
    {
        public WorkItemsChangedEventArgs(List<WorkItemData> items)
        {
			ChangedItems = items;
        }

		public List<WorkItemData> ChangedItems { get; private set; }
    }
}