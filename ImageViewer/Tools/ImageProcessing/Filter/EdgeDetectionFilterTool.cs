using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter
{
	[MenuAction("apply", "global-menus/MenuTools/MenuFilter/MenuEdgeDetection")]
	[MenuAction("apply", "imageviewer-filterdropdownmenu/MenuEdgeDetection")]
	[MenuAction("apply", "imageviewer-contextmenu/MenuFilter/MenuEdgeDetection")]
	[ClickHandler("apply", "Apply")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[Tooltip("apply", "TooltipEdgeDetection")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class EdgeDetectionFilterTool : ImageViewerTool
	{
		public EdgeDetectionFilterTool()
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

			ConvolutionKernel m = new ConvolutionKernel();
			m.TopLeft = m.TopMid = m.TopRight = -1;
			m.MidLeft = m.Pixel = m.MidRight = 0;
			m.BottomLeft = m.BottomMid = m.BottomRight = 1;

			m.Offset = 127;

			ConvolutionFilter.Apply(image as GrayscaleImageGraphic, m);

			image.Draw();
		}
	}
}
