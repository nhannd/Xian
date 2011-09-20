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

namespace ClearCanvas.Ris.Shreds.Merge
{
	/// <summary>
	/// Shred responsible for processing WorkQueue items with type "Merge".
	/// </summary>
	[ExtensionOf(typeof(ShredExtensionPoint))]
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class MergeShred : QueueProcessorShred, IApplicationRoot
	{
		public override string GetDisplayName()
		{
			return SR.MergeShredName;
		}

		public override string GetDescription()
		{
			return SR.MergeShredDescription;
		}

		protected override IList<QueueProcessor> GetProcessors()
		{
			var p = new MergeProcessor(new MergeShredSettings());
			return new [] { p };
		}


		#region IApplicationRoot Members

		public void RunApplication(string[] args)
		{
			Start();
		}

		#endregion
	}
}
