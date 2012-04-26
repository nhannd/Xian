#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.Shreds
{
	internal static class WorkItemActivityPublisher
	{
		internal static void WorkItemChanged(WorkItemData workItem)
		{
			WorkItemsChanged(new List<WorkItemData> { workItem });
		}

		internal static void WorkItemsChanged(List<WorkItemData> workItems)
		{
			try
			{
				PublishManager<IWorkItemActivityCallback>.Publish("WorkItemsChanged", workItems);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Unexpected error attempting to publish WorkItemsChanged notification.");
			}
		}

		internal static void StudiesCleared()
		{
			try
			{
				PublishManager<IWorkItemActivityCallback>.Publish("StudiesCleared");
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Unexpected error attempting to publish StudiesCleared notification.");
			}
		}
	}
}
