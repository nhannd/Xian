using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	public class DisplaySetEventArgs : CollectionEventArgs<IDisplaySet>
	{
		public DisplaySetEventArgs()
		{
		}

		public DisplaySetEventArgs(IDisplaySet displaySet)
		{
			//Platform.CheckForNullReference(displaySet, "displaySet");

			base.Item  = displaySet;
		}

		public IDisplaySet DisplaySet { get { return base.Item; } }
	}
}
