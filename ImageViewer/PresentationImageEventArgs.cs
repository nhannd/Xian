using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model
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
