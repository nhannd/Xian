using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public interface IMouseInformation
	{
		ITile Tile { get; }
		Point Location { get; }
		XMouseButtons MouseButtonDown { get; }
	}
}
