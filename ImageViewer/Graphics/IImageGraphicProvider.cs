using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Graphics
{
	public interface IImageGraphicProvider
	{
		ImageGraphic Image { get; }
	}
}
