using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class SpatialTransformApplicator : ImageOperationApplicator
	{
		public SpatialTransformApplicator(IPresentationImage selectedPresentationImage)
			: base(selectedPresentationImage)
		{

		}

		protected override IMemorable GetOriginator(IPresentationImage image)
		{
			return image.LayerManager.SelectedLayerGroup.SpatialTransform;
		}
	}
}
