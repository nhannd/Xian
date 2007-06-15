using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	public class DynamicTeApplicator : ImageOperationApplicator
	{
		public DynamicTeApplicator(IPresentationImage presentationImage)
			: base(presentationImage)
		{

		}

		protected override IMemorable GetOriginator(IPresentationImage image)
		{
			IDynamicTeProvider teProvider = image as IDynamicTeProvider;

			if (teProvider == null)
				throw new InvalidOperationException();

			return teProvider.DynamicTe as IMemorable;
		}
	}
}
