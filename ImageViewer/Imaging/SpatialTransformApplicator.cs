using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public class SpatialTransformApplicator : ImageOperationApplicator
	{
		public SpatialTransformApplicator(ISpatialTransformProvider associatedSpatialTransform)
			: base(associatedSpatialTransform as IPresentationImage)
		{

		}

		protected override IMemorable GetOriginator(IPresentationImage image)
		{
			return (image as ISpatialTransformProvider).SpatialTransform;
		}
	}
}
