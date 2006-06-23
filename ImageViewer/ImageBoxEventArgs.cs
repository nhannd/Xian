using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model
{
	public class ImageBoxEventArgs : CollectionEventArgs<ImageBox>
	{
		public ImageBoxEventArgs()
		{

		}

		public ImageBoxEventArgs(ImageBox imageBox)
		{
			Platform.CheckForNullReference(imageBox, "imageBox");

			base.Item = imageBox;
		}

		public ImageBox ImageBox { get { return base.Item; } }
	}
}
