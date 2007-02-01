using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class WindowLevelApplicator : ImageOperationApplicator
	{
		public WindowLevelApplicator(IVOILUTLinearProvider associatedWindowLevel)
			: base(associatedWindowLevel as IPresentationImage)
		{

		}

		protected override IMemorable GetOriginator(IPresentationImage image)
		{
			return (image as IVOILUTLinearProvider).VoiLut;
		}
	}
}
