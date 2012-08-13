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
using ClearCanvas.Common.Shreds;

namespace ClearCanvas.Ris.Shreds.HL7
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class LogicalHL7EventShred : QueueProcessorShred, IApplicationRoot
	{
		public LogicalHL7EventShred()
		{
		}

		public override string GetDisplayName()
		{
			return SR.LogicalHL7EventShredName;
		}

		public override string GetDescription()
		{
			return SR.LogicalHL7EventShredDescription;
		}

		protected override IList<QueueProcessor> GetProcessors()
		{
			var settings = new LogicalHL7EventShredSettings();
			var p = new LogicalHL7EventProcessor(
				settings.BatchSize,
				TimeSpan.FromSeconds(settings.EmptyQueueSleepTime),
				TimeSpan.FromSeconds(settings.FailedItemRetryDelay));
			return new QueueProcessor[] { p };
		}

		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			Start();
		}

		#endregion
	}
}