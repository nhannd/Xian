using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
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
