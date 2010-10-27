#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.Workflow.Brokers
{
	public partial interface IWorkQueueItemBroker
	{
		/// <summary>
		/// Gets pending work queue items of the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="maxItems"></param>
		IList<WorkQueueItem> GetPendingItems(string type, int maxItems);
	}
}
