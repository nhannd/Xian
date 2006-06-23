using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Application;

namespace ClearCanvas.Workstation.Model.Imaging
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
