using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	public class PresentationImageEventArgs : CollectionEventArgs<IPresentationImage>
	{
		public PresentationImageEventArgs()
		{

		}

		public PresentationImageEventArgs(IPresentationImage presentationImage)
		{
			Platform.CheckForNullReference(presentationImage, "presentationImage");

			base.Item = presentationImage;
		}

		public IPresentationImage PresentationImage { get { return base.Item; } }
	}
}
