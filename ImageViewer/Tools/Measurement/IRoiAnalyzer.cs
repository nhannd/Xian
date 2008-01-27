using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	public interface IRoiAnalyzer
	{
		string Analyze(RoiGraphic roiGraphic);
	}
}
