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
	// Note how the same action appears in three different menus: 
	// the main menu, a dropdown menu on the main toolbar, and the context menu
	[MenuAction("apply", "global-menus/MenuTools/MenuFilter/MenuSharpening")]
	[MenuAction("apply", "imageviewer-filterdropdownmenu/MenuSharpening")]
	[MenuAction("apply", "imageviewer-contextmenu/MenuFilter/MenuSharpening")]
	[ClickHandler("apply", "Apply")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[Tooltip("apply", "TooltipSharpening")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class SharpeningFilterTool : ImageViewerTool
	{
		public SharpeningFilterTool()
		{

		}

		public void Apply()
		{
			// Check for nulls before we using anything
			if (this.SelectedImageGraphicProvider == null)
				return;

			ImageGraphic image = this.SelectedImageGraphicProvider.ImageGraphic;

			if (image == null)
				return;

			if (!(image is GrayscaleImageGraphic))
				return;

			// Setup the sharpening kernel
			int nWeight = 10;
			ConvolutionKernel m = new ConvolutionKernel();
			m.SetAll(0);
			m.Pixel = nWeight;
			m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = -2;
			m.Factor = nWeight - 8;

			// Apply the filter
			ConvolutionFilter.Apply(image as GrayscaleImageGraphic, m);

			// Redraw the image.  Note that this will re-render the 
			// entire PresentationImage.
			image.Draw();
		}
	}
}
