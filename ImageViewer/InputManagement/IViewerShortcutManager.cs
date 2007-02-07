using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public interface IViewerShortcutManager
	{
		void ChangeMouseToolAssignment(MouseImageViewerTool mouseTool, XMouseButtons button);
		
		IClickAction GetKeyboardAction(KeyboardButtonShortcut shortcut);
		IMouseButtonHandler GetMouseButtonHandler(MouseButtonShortcut shortcut);
		IMouseWheelHandler GetMouseWheelHandler(MouseWheelShortcut shortcut);
	}
}
