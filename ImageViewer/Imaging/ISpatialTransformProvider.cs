using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface ISpatialTransformProvider : IDrawable
	{
		ISpatialTransform SpatialTransform { get; }
	}
}
