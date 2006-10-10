using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	public class ImageBoxEventArgs : CollectionEventArgs<IImageBox>
	{
		public ImageBoxEventArgs()
		{

		}

		public ImageBoxEventArgs(IImageBox imageBox)
		{
			Platform.CheckForNullReference(imageBox, "imageBox");

			base.Item = imageBox;
		}

		public IImageBox ImageBox { get { return base.Item; } }
	}
}
