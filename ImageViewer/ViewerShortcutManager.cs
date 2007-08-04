using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer
{
	internal sealed class ViewerShortcutManager : IViewerShortcutManager
	{
		private Dictionary<IMouseButtonHandler, XMouseButtons> _mouseToolButtonMap;
		private Dictionary<MouseButtonShortcut, IMouseButtonHandler> _activeMouseButtonShortcutMap;
		private Dictionary<ITool, ITool> _setRegisteredTools;

		public ViewerShortcutManager()
		{
			_mouseToolButtonMap = new Dictionary<IMouseButtonHandler, XMouseButtons>();
			_activeMouseButtonShortcutMap = new Dictionary<MouseButtonShortcut, IMouseButtonHandler>();
			_setRegisteredTools = new Dictionary<ITool, ITool>();
		}

		private void RegisterMouseToolButton(MouseImageViewerTool mouseTool)
		{
			if (mouseTool.MouseButton == XMouseButtons.None)
			{
				Platform.Log(LogLevel.Error, String.Format(SR.ExceptionMouseToolMustHaveButtonAssignment, mouseTool.GetType().FullName));
				return;
			}

			_mouseToolButtonMap.Add(mouseTool, mouseTool.MouseButton);

			if (mouseTool.Active)
				ActivateMouseTool(mouseTool, false);

			mouseTool.MouseButtonChanged += new EventHandler(OnMouseToolMouseButtonChanged);
			mouseTool.ActivationChanged += new EventHandler(OnMouseToolActivationChanged);
		}

		private void DeactivateMouseTool(MouseImageViewerTool mouseTool)
		{
			if (_mouseToolButtonMap.ContainsKey(mouseTool))
			{
				MouseButtonShortcut shortcut = new MouseButtonShortcut(_mouseToolButtonMap[mouseTool]);

				if (_activeMouseButtonShortcutMap.ContainsKey(shortcut))
				{
					if (_activeMouseButtonShortcutMap[shortcut] == mouseTool)
						_activeMouseButtonShortcutMap.Remove(shortcut);
				}
			}
		}

		private void ActivateMouseTool(MouseImageViewerTool activateMouseTool, bool replaceExisting)
		{
			if (!_mouseToolButtonMap.ContainsKey(activateMouseTool))
			{
				Platform.Log(LogLevel.Error, String.Format(SR.ExceptionMouseToolHasNoAssignment, activateMouseTool.GetType().FullName));
				return;
			}

			MouseButtonShortcut shortcut = new MouseButtonShortcut(_mouseToolButtonMap[activateMouseTool]);

			if (_activeMouseButtonShortcutMap.ContainsKey(shortcut))
			{
				if (!replaceExisting)
					return;

				MouseImageViewerTool oldMouseTool = (MouseImageViewerTool)_activeMouseButtonShortcutMap[shortcut];
				if (oldMouseTool != activateMouseTool)
				{
					_activeMouseButtonShortcutMap[shortcut] = activateMouseTool;
					oldMouseTool.Active = false;
				}
			}
			else
			{
				_activeMouseButtonShortcutMap.Add(shortcut, activateMouseTool);
			}

			if (!activateMouseTool.Active)
				activateMouseTool.Active = true;
		}

		private void OnMouseToolActivationChanged(object sender, EventArgs e)
		{
			MouseImageViewerTool mouseTool = (MouseImageViewerTool)sender;

			if (mouseTool.Active)
				ActivateMouseTool(mouseTool, true);
			else
				DeactivateMouseTool(mouseTool);
		}

		private void OnMouseToolMouseButtonChanged(object sender, EventArgs e)
		{
			MouseImageViewerTool mouseTool = sender as MouseImageViewerTool;

			if (!_mouseToolButtonMap.ContainsKey(mouseTool))
			{
				_mouseToolButtonMap.Add(mouseTool, mouseTool.MouseButton);
			}
			else
			{
				if (_mouseToolButtonMap[mouseTool] == mouseTool.MouseButton)
					return;

				//remove it from the active map, if it's there.
				DeactivateMouseTool(mouseTool);

				_mouseToolButtonMap[mouseTool] = mouseTool.MouseButton;
			}

			//Add it to the active map with the new key.
			if (mouseTool.Active)
				ActivateMouseTool(mouseTool, true);
		}

		/// <summary>
		/// Registers the tool with the viewer shortcut manager.
		/// </summary>
		/// <param name="tool">the tool to register.</param>
		public void RegisterImageViewerTool(ITool tool)
		{
			Platform.CheckForNullReference(tool, "tool");

			if (tool is MouseImageViewerTool)
			{
				RegisterMouseToolButton(tool as MouseImageViewerTool);
			}

			_setRegisteredTools[tool] = tool;
		}

		#region IViewerShortcutManager Members

		/// <summary>
		/// Gets the <see cref="IMouseButtonHandler"/> associated with a shortcut.
		/// </summary>
		/// <param name="shortcut">The shortcut for which an <see cref="IMouseButtonHandler"/> is to be found.</param>
		/// <returns>An <see cref="IMouseButtonHandler"/> or null.</returns>
		public IMouseButtonHandler GetMouseButtonHandler(MouseButtonShortcut shortcut)
		{
			Platform.CheckForNullReference(shortcut, "shortcut");

			if (_activeMouseButtonShortcutMap.ContainsKey(shortcut))
				return _activeMouseButtonShortcutMap[shortcut];

			foreach (ITool tool in _setRegisteredTools.Keys)
			{
				MouseImageViewerTool mouseButtonHandler = tool as MouseImageViewerTool;
				if (mouseButtonHandler != null)
				{
					if (shortcut.Equals(mouseButtonHandler.ModifiedMouseButtonShortcut))
						return mouseButtonHandler;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the <see cref="IMouseWheelHandler"/> associated with a shortcut.
		/// </summary>
		/// <param name="shortcut">The shortcut for which an <see cref="IMouseWheelHandler"/> is to be found.</param>
		/// <returns>An <see cref="IMouseWheelHandler"/> or null.</returns>
		public IMouseWheelHandler GetMouseWheelHandler(MouseWheelShortcut shortcut)
		{
			Platform.CheckForNullReference(shortcut, "shortcut");

			foreach (ITool tool in _setRegisteredTools.Keys)
			{
				ImageViewerTool viewerTool = tool as ImageViewerTool;
				if (viewerTool != null)
				{
					if (shortcut.Equals(viewerTool.MouseWheelShortcut))
						return viewerTool;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the <see cref="IClickAction"/> associated with a shortcut.
		/// </summary>
		/// <param name="shortcut">The shortcut for which an <see cref="IClickAction"/> is to be found.</param>
		/// <returns>An <see cref="IClickAction"/> or null.</returns>
		public IClickAction GetKeyboardAction(KeyboardButtonShortcut shortcut)
		{
			foreach (ITool tool in _setRegisteredTools.Keys)
			{
				foreach (IAction action in tool.Actions)
				{
					IClickAction clickAction = action as IClickAction;
					if (clickAction != null)
					{
						if (shortcut.Equals(clickAction.KeyStroke))
							return clickAction;
					}
				}
			}

			return null;
		}

		#endregion
	}
}
