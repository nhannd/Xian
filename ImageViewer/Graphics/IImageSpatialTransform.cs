using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Graphics
{
	public interface IImageSpatialTransform : ISpatialTransform
	{
		bool ScaleToFit { get; set; }
	}
}
