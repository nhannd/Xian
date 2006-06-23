using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public class DisplaySetEventArgs : CollectionEventArgs<DisplaySet>
	{
		public DisplaySetEventArgs()
		{
		}

		public DisplaySetEventArgs(DisplaySet displaySet)
		{
			//Platform.CheckForNullReference(displaySet, "displaySet");

			base.Item  = displaySet;
		}

		public DisplaySet DisplaySet { get { return base.Item; } }
	}
}
