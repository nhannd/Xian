using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

#pragma warning disable 0419,1574,1587,1591

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

		public void Delete()
		{
			bool anyLocked = false;

			foreach (ClipboardItem item in this.Context.SelectedClipboardItems)
			{
				if (item.Locked)
				{
					anyLocked = true;
				}
				else
				{
					((IDisposable)item).Dispose(); 
					this.Context.ClipboardItems.Remove(item);
				}
			}

			if (anyLocked)
				this.Context.DesktopWindow.ShowMessageBox(SR.MessageUnableToClearClipboardItems, MessageBoxActions.Ok);
		}
	}
}
