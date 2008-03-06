using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Clipboard
{
	[MenuAction("delete", "clipboard-contextmenu/MenuDeleteClipboardItem", "Delete")]
	[ButtonAction("delete", "clipboard-toolbar/ToolbarDeleteClipboardItem", "Delete")]
	[Tooltip("delete", "TooltipDeleteClipboardItem")]
	[IconSet("delete", IconScheme.Colour, "Icons.DeleteClipboardItemToolSmall.png", "Icons.DeleteClipboardItemToolSmall.png", "Icons.DeleteClipboardItemToolSmall.png")]
	[EnabledStateObserver("delete", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ClipboardToolExtensionPoint))]
	public class DeleteClipboardItemTool : ClipboardTool
	{
		public DeleteClipboardItemTool()
		{
		}

		public override void Initialize()
		{
			this.Enabled = this.Context.SelectedClipboardItems.Count > 0;
			base.Initialize();
		}

		public void Delete()
		{
			foreach (IClipboardItem item in this.Context.SelectedClipboardItems)
				this.Context.ClipboardItems.Remove(item);
		}
	}
}
