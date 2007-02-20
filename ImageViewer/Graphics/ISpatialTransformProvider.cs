using System;

namespace ClearCanvas.ImageViewer.Graphics
{
	public interface ISpatialTransformProvider : IDrawable
	{
		ISpatialTransform SpatialTransform { get; }
	}
}
