using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IBasicVoiLutLinear : IVoiLutLinear, IVoiLut
	{
		double WindowWidth { get; set; }
		double WindowCenter { get; set; }
	}
}
