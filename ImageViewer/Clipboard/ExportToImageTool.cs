using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Clipboard
{
	[MenuAction("export", "clipboard-contextmenu/MenuExportToImage", "Export")]
	[ButtonAction("export", "clipboard-toolbar/ToolbarExportToImage", "Export")]
	[Tooltip("export", "TooltipExportToImage")]
	[IconSet("export", IconScheme.Colour, "Icons.ExportToImageToolSmall.png", "Icons.ExportToImageToolSmall.png", "Icons.ExportToImageToolSmall.png")]
	[EnabledStateObserver("delete", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ClipboardToolExtensionPoint))]
	public class ExportToImageTool : ClipboardTool
	{
		public ExportToImageTool()
		{
		}

		public override void Initialize()
		{
			this.Enabled = this.Context.SelectedClipboardItems.Count > 0;
			base.Initialize();
		}

		public void Export()
		{
			string str = "";

			foreach (IClipboardItem item in this.Context.ClipboardItems)
			{
				IDisplaySet ds = item.Item as IDisplaySet;
				str += ds.Name + "\n";
			}

			this.Context.DesktopWindow.ShowMessageBox(str, MessageBoxActions.Ok);
		}
	}
}
