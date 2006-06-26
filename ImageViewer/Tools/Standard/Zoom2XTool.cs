using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	/// <summary>
	/// Summary description for ZoomTool.
	/// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.ImageViewer.ImageWorkspaceToolExtensionPoint))]
	public class Zoom2XTool : ZoomFixedTool
	{
		public Zoom2XTool()
		{
		}

		private void GetDefaults()
		{
			//base.MenuPath.AddStandardItem(Model.SR.MenuTools);
			//base.MenuPath.AddStandardItem(Model.SR.MenuToolsStandard);
			//base.MenuPath.AddCustomItem(SR.MenuToolsStandardZoom, 30);

			//base.ToolbarPath.AddStandardItem(Model.SR.ToolbarStandard);
			//base.ToolbarPath.AddCustomItem(SR.ToolbarToolsStandardZoom, 50);
			//base.ToolbarPath.AddCustomItem(SR.ToolbarToolsStandardZoom2X, 20);

			//base.Tooltip = SR.ToolbarToolsStandardZoom2X;
		}

        public override void Activate()
        {
            this.ApplyZoom(2.0f);
        }
    }
}
