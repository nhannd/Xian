using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Clipboard;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyObjects
{
	[ButtonAction("edit", KeyImageClipboard.ToolbarSite + "/ToolbarEditKeyImageInformation", "Edit")]
	[Tooltip("edit", "TooltipEditKeyImageInformation")]
	[IconSet("edit", IconScheme.Colour, "Icons.EditKeyImageInformationToolSmall.png", "Icons.EditKeyImageInformationToolMedium.png", "Icons.EditKeyImageInformationToolLarge.png")]
	
	[ExtensionOf(typeof(ClipboardToolExtensionPoint))]
	internal class EditKeyImageInformationTool : ClipboardTool
	{
		public EditKeyImageInformationTool()
		{
		}

		public void Edit()
		{
			//TODO: can we use an override to add actions?
			KeyImageInformationEditorComponent.Launch(this.Context.DesktopWindow);
		}
	}
}
