#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds.HL7
{
	/// <summary>
	/// Responsible for processing a single LogicalHL7Event into one or more HL7Event objects per peer
	/// </summary>
	public class LogicalHL7EventProcessor : WorkQueueProcessor
	{
		private TimeSpan _failedItemRetryDelay;

		internal LogicalHL7EventProcessor(int batchSize, TimeSpan sleepTime, TimeSpan failedItemRetryDelay)
			: base(batchSize, sleepTime)
		{
			_failedItemRetryDelay = failedItemRetryDelay;
		}

		#region WorkQueueProcessor overrides

		protected override void ActOnItem(WorkQueueItem item)
		{
			var logicalEvent = new LogicalHL7EventArgs(item);
			Platform.Log(LogLevel.Info, String.Format("Procssing HL7LogicalEvent {0}", item.OID));

			foreach (ILogicalHL7EventListener listener in new LogicalHL7EventListenerExtensionPoint().CreateExtensions())
			{
				listener.OnEvent(logicalEvent);
			}
		}

		protected override string WorkQueueItemType
		{
			get { return LogicalHL7Event.WorkQueueItemType; }
		}

		protected override bool ShouldReschedule(WorkQueueItem item, Exception error, out DateTime rescheduleTime)
		{
			if (error == null)
				return base.ShouldReschedule(item, null, out rescheduleTime);

			//todo: should we retry? things might end up being processed out of order
			rescheduleTime = Platform.Time + _failedItemRetryDelay;
			return true;
		}

		public override string Name
		{
			get { return this.GetType().Name; }
		}

		#endregion

	}
}