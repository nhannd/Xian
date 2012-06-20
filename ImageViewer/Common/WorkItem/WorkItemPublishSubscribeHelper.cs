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
    // TODO (CR Jun 2012): I think this should go back in Shreds, or at least be put in a sub-namespace somewhere. Feels weird in here.
	public static class WorkItemPublishSubscribeHelper
	{
		private const string WorkItemsChanged = "WorkItemsChanged";
		private const string StudiesCleared = "StudiesCleared";

        public static void Subscribe(IWorkItemActivityCallback callback)
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

        public static void Unsubscribe(IWorkItemActivityCallback callback)
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
        
        public static void PublishWorkItemChanged(WorkItemsChangedEventType eventType, WorkItemData workItem)
		{
			PublishWorkItemsChanged(eventType, new List<WorkItemData> { workItem });
		}

		public static void PublishWorkItemsChanged(WorkItemsChangedEventType eventType, List<WorkItemData> workItems)
		{
			try
			{
				PublishManager<IWorkItemActivityCallback>.Publish(WorkItemsChanged, eventType, workItems);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Unexpected error attempting to publish WorkItemsChanged notification.");
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
