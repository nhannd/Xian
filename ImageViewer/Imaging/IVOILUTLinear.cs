using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Imaging
{
	public interface IVoiLutLinear : IVoiLut
	{
		double WindowWidth { get; }
		double WindowCenter { get; }
	}
}
