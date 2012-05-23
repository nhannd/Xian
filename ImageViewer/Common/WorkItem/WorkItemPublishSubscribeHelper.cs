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

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
	public static class WorkItemPublishSubscribeHelper
	{
		private const string WorkItemsChanged = "WorkItemsChanged";
		private const string StudiesCleared = "StudiesCleared";

		public static void PublishWorkItemChanged(WorkItemData workItem)
		{
			PublishWorkItemsChanged(new List<WorkItemData> { workItem });
		}

		public static void PublishWorkItemsChanged(List<WorkItemData> workItems)
		{
			try
			{
				PublishManager<IWorkItemActivityCallback>.Publish(WorkItemsChanged, workItems);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Unexpected error attempting to publish WorkItemsChanged notification.");
			}
		}

		public static void SubscribeWorkItemsChanged(IWorkItemActivityCallback callback)
		{
			try
			{
				SubscriptionManager<IWorkItemActivityCallback>.Subscribe(callback, WorkItemsChanged);
                SubscriptionManager<IWorkItemActivityCallback>.Subscribe(callback, StudiesCleared);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				throw;
			}
		}

		public static void UnsubscribeWorkItemsChanged(IWorkItemActivityCallback callback)
		{
			try
			{
				SubscriptionManager<IWorkItemActivityCallback>.Unsubscribe(callback, WorkItemsChanged);
                SubscriptionManager<IWorkItemActivityCallback>.Unsubscribe(callback, StudiesCleared);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				throw;
			}
		}

		public static void PublishStudiesCleared()
		{
			try
			{
				PublishManager<IWorkItemActivityCallback>.Publish(StudiesCleared);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Unexpected error attempting to publish StudiesCleared notification.");
			}
		}
	}
}
