using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IVOILUTLinearProvider : IDrawable
	{
		IVOILUTLinear VoiLutLinear { get; }
	}
}
