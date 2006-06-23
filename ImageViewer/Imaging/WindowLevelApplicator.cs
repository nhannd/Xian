using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class WindowLevelApplicator : ImageOperationApplicator
	{
		public WindowLevelApplicator(PresentationImage selectedPresentationImage)
			: base(selectedPresentationImage)
		{

		}

		protected override IMemorable GetOriginator(PresentationImage image)
		{
			return new WindowLevelOperator(image);
		}
	}
}
