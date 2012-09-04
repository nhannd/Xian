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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds.Publication
{
	/// <summary>
	/// Responsible for processing work items that represent Publication Actions.
	/// </summary>
	public class PublicationActionProcessor : WorkQueueProcessor
	{
		private readonly TimeSpan _failedItemRetryDelay;

		internal PublicationActionProcessor(PublicationShredSettings settings)
			: base(settings.BatchSize, TimeSpan.FromSeconds(settings.EmptyQueueSleepTime))
		{
			_failedItemRetryDelay = TimeSpan.FromSeconds(settings.FailedItemRetryDelay);
		}

		#region WorkQueueProcessor overrides

		protected override void ActOnItem(WorkQueueItem item)
		{

			var actionType = item.ExtendedProperties["ActionType"];
			var action = (IPublicationAction)new PublicationActionExtensionPoint().CreateExtension(new ClassNameExtensionFilter(actionType));
			var reportPartRef = new EntityRef(item.ExtendedProperties["ReportPartRef"]);
			var reportPart = PersistenceScope.CurrentContext.Load<ReportPart>(reportPartRef, EntityLoadFlags.None);

			Platform.Log(LogLevel.Info, String.Format("Processing Publication Action {0} for report part {1}", actionType, reportPart.OID));

			action.Execute(reportPart, PersistenceScope.CurrentContext);
		}

		protected override string WorkQueueItemType
		{
			get { return "Publication Action"; }
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