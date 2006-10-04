using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
    [ClearCanvas.Common.ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class Zoom1XTool : ZoomFixedTool
	{
		public Zoom1XTool()
		{
		}

		private void GetDefaults()
		{
			//base.MenuPath.AddStandardItem(Model.SR.MenuTools);
			//base.MenuPath.AddStandardItem(Model.SR.MenuToolsStandard);
			//base.MenuPath.AddCustomItem(SR.MenuToolsStandardZoom, 30);

			//base.ToolbarPath.AddStandardItem(Model.SR.ToolbarStandard);
			//base.ToolbarPath.AddCustomItem(SR.ToolbarToolsStandardZoom, 50);
			//base.ToolbarPath.AddCustomItem(SR.ToolbarToolsStandardZoom1X, 10);

			//base.Tooltip = SR.ToolbarToolsStandardZoom1X;
		}

        public override void Activate()
        {
            this.ApplyZoom(1.0f);
        }
	}
}
