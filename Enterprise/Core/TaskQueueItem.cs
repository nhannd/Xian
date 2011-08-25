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

namespace ClearCanvas.Enterprise.Core
{
	public abstract class TaskQueueItem : Entity
	{
		protected TaskQueueItem()
		{
			this.CreationTime = this.ScheduledTime = Platform.Time;
			this.TaskProperties = new Dictionary<string, string>();
		}

		public DateTime CreationTime { get; private set; }
		public DateTime ScheduledTime { get; private set;}
		public TaskQueueItemStatus Status { get; private set; }
		public IDictionary<string, string> TaskProperties { get; private set; }

		public void Schedule(DateTime time)
		{
			if(this.Status == TaskQueueItemStatus.Scheduled)
			{
				this.ScheduledTime = time;
			}
			else
			{
				throw new InvalidOperationException("This item has already been processed.");
			}
		}

		public void Process()
		{
			try
			{
				Execute();
				this.Status = TaskQueueItemStatus.Completed;
			}
			catch(Exception e)
			{
				this.Status = TaskQueueItemStatus.Failed;
				// todo: record the exception message in the queue item?
			}
		}


		protected abstract void Execute();
	}
}
