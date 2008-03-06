using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Clipboard
{
	[MenuAction("deleteAll", "clipboard-contextmenu/MenuDeleteAllClipboardItems", "DeleteAll")]
	[ButtonAction("deleteAll", "clipboard-toolbar/ToolbarDeleteAllClipboardItems", "DeleteAll")]
	[Tooltip("deleteAll", "TooltipDeleteAllClipboardItems")]
	[IconSet("deleteAll", IconScheme.Colour, "Icons.DeleteAllClipboardItemsToolSmall.png", "Icons.DeleteAllClipboardItemsToolSmall.png", "Icons.DeleteClipboardItemToolSmall.png")]
	[EnabledStateObserver("deleteAll", "Enabled", "EnabledChanged")]
	
	[ExtensionOf(typeof(ClipboardToolExtensionPoint))]
	public class DeleteAllClipboardItemsTool : ClipboardTool
	{
		public DeleteAllClipboardItemsTool()
		{
		}

		public override void Initialize()
		{
			this.Enabled = this.Context.ClipboardItems.Count > 0;
			this.ApplyOnlyToSelected = false;

			base.Initialize();
		}

		public void DeleteAll()
		{
			while (this.Context.ClipboardItems.Count > 0)
				this.Context.ClipboardItems.RemoveAt(0);
		}
	}
}
