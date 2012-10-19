#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	public static class MergeWorkQueueItem
	{
		public const string Tag = "Merge";


		public static WorkQueueItem Create(EntityRef targetRef)
		{
			var workQueueItem = new WorkQueueItem(Tag);
			workQueueItem.ExtendedProperties.Add("Target", targetRef.Serialize());
			return workQueueItem;
		}

		public static EntityRef GetTargetRef(WorkQueueItem item)
		{
			return new EntityRef(item.ExtendedProperties["Target"]);
		}
	}
}
