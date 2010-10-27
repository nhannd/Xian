#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue
{
	/// <summary>
	/// Encapsulates a collection of <see cref="WorkQueue"/> which can be accessed based on the <see cref="ServerEntityKey"/>
	/// </summary>
	public class WorkQueueItemCollection : KeyedCollectionBase<WorkQueueSummary, ServerEntityKey>
	{

		public WorkQueueItemCollection(IList<WorkQueueSummary> list)
			: base(list)
		{
		}

		protected override ServerEntityKey GetKey(WorkQueueSummary item)
		{
			return item.Key;
		}
	}
}
