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

namespace ClearCanvas.ImageViewer.Shreds.FilestoreManagement
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	public class FilestoreManagementShred : QueueProcessorShred
	{
		public FilestoreManagementShred()
		{
			
		}

		public override string GetDisplayName()
		{
			return SR.FilestoreManagementShred;
		}

		public override string GetDescription()
		{
			return SR.FilestoreManagementShred;
		}

		protected override IList<QueueProcessor> GetProcessors()
		{
			return new QueueProcessor[] {new FilestoreManagementProcessor(10, TimeSpan.FromSeconds(60))};
		}
	}
}
