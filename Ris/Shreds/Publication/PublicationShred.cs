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

namespace ClearCanvas.Ris.Shreds.Publication
{
	[ExtensionOf(typeof(ShredExtensionPoint))]
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class PublicationShred : QueueProcessorShred, IApplicationRoot
	{
		public PublicationShred()
		{
		}

		public override string GetDisplayName()
		{
			return SR.PublicationShredName;
		}

		public override string GetDescription()
		{
			return SR.PublicationShredDescription;
		}

		protected override IList<QueueProcessor> GetProcessors()
		{
			var p1 = new PublicationStepProcessor(new PublicationShredSettings());
			var p2 = new PublicationActionProcessor(new PublicationShredSettings());
			return new QueueProcessor[] { p1, p2 };
		}

		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			Start();
		}

		#endregion
	}
}