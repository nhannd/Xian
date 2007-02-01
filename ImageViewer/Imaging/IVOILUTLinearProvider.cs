using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IVOILUTLinearProvider : IDrawable
	{
		IVOILUTLinear VoiLut { get; }
	}
}
