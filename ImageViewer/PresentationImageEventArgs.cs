using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	public class PresentationImageEventArgs : CollectionEventArgs<PresentationImage>
	{
		public PresentationImageEventArgs()
		{

		}

		public PresentationImageEventArgs(PresentationImage presentationImage)
		{
			Platform.CheckForNullReference(presentationImage, "presentationImage");

			base.Item = presentationImage;
		}

		public PresentationImage PresentationImage { get { return base.Item; } }
	}
}
