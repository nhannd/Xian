using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.InputManagement
{
	// TODO: For internal use only
	public interface IViewerShortcutManager
	{
		IClickAction GetKeyboardAction(KeyboardButtonShortcut shortcut);
		IMouseButtonHandler GetMouseButtonHandler(MouseButtonShortcut shortcut);
		IMouseWheelHandler GetMouseWheelHandler(MouseWheelShortcut shortcut);
	}
}
