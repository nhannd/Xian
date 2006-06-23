using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Application;

namespace ClearCanvas.Workstation.Model.Imaging
{
	public class SpatialTransformApplicator : ImageOperationApplicator
	{
		public SpatialTransformApplicator(PresentationImage selectedPresentationImage)
			: base(selectedPresentationImage)
		{

		}

		protected override IMemorable GetOriginator(PresentationImage image)
		{
			return image.LayerManager.SelectedLayerGroup.SpatialTransform;
		}
	}
}
