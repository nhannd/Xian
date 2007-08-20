using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Graphics
{
	public interface IGrayscaleImageGraphicProvider
	{
		/// <summary>
		/// Gets a <see cref="GrayscaleImageGraphic"/>.
		/// </summary>
		GrayscaleImageGraphic ImageGraphic { get; }
	}
}
