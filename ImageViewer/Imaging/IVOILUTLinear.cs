using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IVOILUTLinear : IMemorable
	{
		double WindowWidth { get; set; }
		double WindowCenter { get; set; }
	}
}
