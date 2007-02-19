using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class SpatialTransformApplicator : ImageOperationApplicator
	{
		public SpatialTransformApplicator(IPresentationImage image)
			: base(image)
		{

		}

		protected override IMemorable GetOriginator(IPresentationImage image)
		{
			ISpatialTransformProvider voiLutLinear = image as ISpatialTransformProvider;

			if (voiLutLinear == null)
				throw new Exception("PresentationImage does not support ISpatialTransformProvider");

			return voiLutLinear.SpatialTransform as IMemorable;
		}
	}
}
