using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Samples.Filter
{
	[MenuAction("apply", "global-menus/MenuTools/MenuFilter/MenuSmoothing")]
	[MenuAction("apply", "imageviewer-filterdropdownmenu/MenuSmoothing")]
	[MenuAction("apply", "imageviewer-contextmenu/MenuFilter/MenuSmoothing")]
	[ClickHandler("apply", "Apply")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[Tooltip("apply", "TooltipSmoothing")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class SmoothingFilterTool : ImageViewerTool
	{
		public SmoothingFilterTool()
		{

		}

		public void Apply()
		{
			if (this.SelectedImageGraphicProvider == null)
				return;

			ImageGraphic image = this.SelectedImageGraphicProvider.ImageGraphic;

			if (image == null)
				return;

			if (!(image is GrayscaleImageGraphic))
				return;

			int nWeight = 10;
			ConvolutionKernel m = new ConvolutionKernel();
			m.SetAll(1);
			m.Pixel = nWeight;
			m.Factor = nWeight + 8;

			ConvolutionFilter.Apply(image as GrayscaleImageGraphic, m);

			image.Draw();
		}
	}
}
