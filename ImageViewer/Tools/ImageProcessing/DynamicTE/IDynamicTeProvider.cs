using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe
{
	public interface IDynamicTeProvider : IDrawable
	{
		DynamicTe DynamicTe { get; }
	}
}
