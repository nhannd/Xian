#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Workflow
{
	/// <summary>
	/// Enforces loose coupling between WorkQueueItem.Type and WorkQueueItemTypeEnum
	/// </summary>
	[ExtensionOf(typeof(EntityChangeSetListenerExtensionPoint))]
	public class WorkQueueItemChangeSetListener : LooseEnumPropertyChangeSetListener<WorkQueueItem, WorkQueueItemTypeEnum>
	{
		#region Implementation of EntityWithEchoedEnumPropertyChangeSetListener

		public override string GetEnumCodeFromEntity(WorkQueueItem workQueueItem)
		{
			return workQueueItem.Type;
		}

		#endregion
	}

}
