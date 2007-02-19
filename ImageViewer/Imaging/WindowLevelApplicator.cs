using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class WindowLevelApplicator : ImageOperationApplicator
	{
		public WindowLevelApplicator(IPresentationImage image)
			: base(image)
		{

		}

		protected override IMemorable GetOriginator(IPresentationImage image)
		{
			IVOILUTLinearProvider voiLutLinear = image as IVOILUTLinearProvider;

			if (voiLutLinear == null)
				throw new Exception("PresentationImage does not support IVOILUTLinear");

			return voiLutLinear.VoiLutLinear as IMemorable;
		}
	}
}
