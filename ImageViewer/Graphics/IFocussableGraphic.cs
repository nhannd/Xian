using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Graphics
{
	public interface IFocussableGraphic : IGraphic
	{
		bool Focussed { get; set; }
	}
}
