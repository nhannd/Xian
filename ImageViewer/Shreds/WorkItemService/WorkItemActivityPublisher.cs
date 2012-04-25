#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService
{
	internal static class WorkItemActivityPublisher
	{
		internal static void WorkItemsChanged(WorkItemData workItem)
		{
			WorkItemsChanged(new List<WorkItemData> { workItem });
		}

		internal static void WorkItemsChanged(List<WorkItemData> workItems)
		{
			PublishManager<IWorkItemActivityCallback>.Publish("WorkItemsChanged", workItems);
		}

		internal static void StudiesCleared()
		{
			PublishManager<IWorkItemActivityCallback>.Publish("StudiesCleared");
		}
	}
}
