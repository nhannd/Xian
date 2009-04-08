using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public interface IExportedActionsProvider
	{
		IActionSet GetExportedActions(string site, IMouseInformation mouseInformation);
	}
}