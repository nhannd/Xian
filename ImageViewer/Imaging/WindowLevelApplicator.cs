using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class WindowLevelApplicator : ImageOperationApplicator
	{
		public WindowLevelApplicator(IPresentationImage selectedPresentationImage)
			: base(selectedPresentationImage)
		{

		}

		protected override IMemorable GetOriginator(IPresentationImage image)
		{
			return new WindowLevelOperator(image);
		}
	}
}
