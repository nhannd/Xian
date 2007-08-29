using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IPresentationLutProvider : IDrawable
	{
		IPresentationLutManager PresentationLutManager { get; }
	}
}
