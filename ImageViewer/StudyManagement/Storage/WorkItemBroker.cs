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
using System.Linq;
using System.Text;
using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
	public class WorkItemBroker : Broker
	{
		internal WorkItemBroker(DicomStoreDataContext context)
			: base(context)
		{
		}

		/// <summary>
		/// Gets the specified number of pending work items.
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		public IList<WorkItem> GetPendingWorkItems(int n)
		{
			return (from w in this.Context.WorkItems
					where w.Status == WorkItemStatusEnum.Pending
					select w).Take(n).ToList();
		}

		public WorkItem GetWorkItem(int oid)
		{
			return this.Context.WorkItems.First(w => w.Oid == oid);
		}
	}
}
