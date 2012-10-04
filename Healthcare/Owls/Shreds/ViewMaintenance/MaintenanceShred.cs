#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using System;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Owls.Shreds.ViewMaintenance
{
	/// <summary>
	/// Finds view items that are older than the retention time and deletes them.
	/// </summary>
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class MaintenanceShred : QueueProcessorShred
	{
		public override string GetDisplayName()
		{
			return SR.MaintenanceShredName;
		}

		public override string GetDescription()
		{
			return SR.MaintenanceShredDescription;
		}

		protected override IList<QueueProcessor> GetProcessors()
		{
			var settings = new MaintenanceShredSettings();
			var sleepTime = TimeSpan.FromSeconds(settings.EmptyQueueSleepTime);
			var batchSize = settings.BatchSize;

			// create processors for every view
			return CollectionUtils.Map<IView, QueueProcessor>(
				new WorklistViewExtensionPoint().CreateExtensions(),
				view => new MaintenanceProcessor(view.CreateShrinker(), batchSize, sleepTime));
		}
	}
}
