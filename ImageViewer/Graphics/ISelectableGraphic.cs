using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Graphics
{
	public interface ISelectableGraphic : IGraphic
	{
		bool Selected { get; set; }
	}
}
