using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Volume
{
	public interface IAssociatedTissues
	{
		GraphicCollection TissueLayers { get; }
	}
}
