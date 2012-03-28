using System;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    public class WorkItemChangedEventArgs : EventArgs
    {
        public WorkItemChangedEventArgs(WorkItemData itemData)
        {
            ItemData = itemData;
        }

        public WorkItemData ItemData { get; private set; }
    }
}