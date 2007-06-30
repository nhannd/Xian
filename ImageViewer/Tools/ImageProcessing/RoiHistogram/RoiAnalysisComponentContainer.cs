using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.RoiHistogram
{
	public class RoiAnalysisComponentContainer : TabComponentContainer
	{
		public RoiAnalysisComponentContainer(IImageViewerToolContext imageViewerToolContext)
		{
			TabPage page = new TabPage("Rectangle", new RoiHistogramComponent(imageViewerToolContext));
			this.Pages.Add(page);
		}
	}
}
