using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	public class ImageSetEventArgs : CollectionEventArgs<IImageSet>
	{
		public ImageSetEventArgs()
		{
		}

		public ImageSetEventArgs(IImageSet imageSet)
		{
			base.Item  = imageSet;
		}

		public IImageSet ImageSet { get { return base.Item; } }
	}
}
