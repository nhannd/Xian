using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IVOILUTLinear : IMemorable
	{
		double WindowCenter { get; set; }
		double WindowWidth { get; set; }
	}
}
