using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public sealed class ViewerShortcutManager : IViewerShortcutManager
	{
		private Dictionary<MouseImageViewerTool, XMouseButtons> _mouseToolButtonMap;
		private Dictionary<MouseButtonShortcut, IMouseButtonHandler> _activeMouseButtonShortcutMap;
		private Dictionary<MouseWheelShortcut, IMouseWheelHandler> _activeMouseWheelShortcutMap;
		private Dictionary<KeyboardButtonShortcut, IClickAction> _keyStrokeShortcutMap;

		public ViewerShortcutManager()
		{
			_mouseToolButtonMap = new Dictionary<MouseImageViewerTool, XMouseButtons>();
			_activeMouseButtonShortcutMap = new Dictionary<MouseButtonShortcut, IMouseButtonHandler>();
			_activeMouseWheelShortcutMap = new Dictionary<MouseWheelShortcut, IMouseWheelHandler>();
			_keyStrokeShortcutMap = new Dictionary<KeyboardButtonShortcut, IClickAction>();
		}

		private void RegisterMouseToolButton(MouseImageViewerTool mouseTool)
		{
			if (mouseTool.MouseButton == XMouseButtons.None)
			{
				Platform.Log(String.Format(SR.ExceptionMouseToolShouldHaveDefault, mouseTool.GetType().FullName));
				return;
			}

			_mouseToolButtonMap.Add(mouseTool, mouseTool.MouseButton);

			if (mouseTool.Active)
				ActivateMouseTool(mouseTool, false);

			mouseTool.MouseButtonChanged += new EventHandler(OnMouseToolMouseButtonChanged);
			mouseTool.ActivationChanged += new EventHandler(OnMouseToolActivationChanged);
		}

		private void RegisterModifiedMouseToolButton(MouseImageViewerTool mouseTool)
		{
			object[] modifiedButtonAssignments = mouseTool.GetType().GetCustomAttributes(typeof(ModifiedMouseToolButtonAttribute), true);
			if (modifiedButtonAssignments == null || modifiedButtonAssignments.Length == 0)
				return;

			ModifiedMouseToolButtonAttribute attribute = modifiedButtonAssignments[0] as ModifiedMouseToolButtonAttribute;

			if (attribute.Shortcut.Modifiers.ModifierFlags == ModifierFlags.None)
			{
				Platform.Log(String.Format(SR.ExceptionAdditionalMouseToolAssignmentsMustBeModified, mouseTool.GetType().FullName));
			}
			else if (_activeMouseButtonShortcutMap.ContainsKey(attribute.Shortcut))
			{
				Platform.Log(String.Format(SR.ExceptionMouseToolAssignmentInUse, mouseTool.GetType().FullName));
			}
			else
			{
				_activeMouseButtonShortcutMap.Add(attribute.Shortcut, mouseTool);
			}
		}

		private void RegisterMouseWheelShortcuts(MouseImageViewerTool mouseTool)
		{
			object[] mouseWheelControlAssignments = mouseTool.GetType().GetCustomAttributes(typeof(MouseToolWheelControlAttribute), true);
			if (mouseWheelControlAssignments == null || mouseWheelControlAssignments.Length == 0)
				return;

			foreach (MouseToolWheelControlAttribute attribute in mouseWheelControlAssignments)
			{
				if (_activeMouseWheelShortcutMap.ContainsKey(attribute.Shortcut))
				{
					Platform.Log(String.Format(SR.ExceptionMouseWheelAssignmentInUse, mouseTool));
				}
				else
				{
					IMouseWheelHandler handler = new MouseWheelHandler(mouseTool, attribute.WheelIncrementDelegateName, attribute.WheelDecrementDelegateName);
					_activeMouseWheelShortcutMap.Add(attribute.Shortcut, handler);
				}
			}
		}

		private void RegisterKeyboardShortcut(IClickAction clickAction)
		{
			if (clickAction.KeyStroke == XKeys.None)
				return;

			KeyboardButtonShortcut shortcut = new KeyboardButtonShortcut(clickAction.KeyStroke);

			if (_keyStrokeShortcutMap.ContainsKey(shortcut))
			{
				Platform.Log(String.Format(SR.ExceptionKeyStrokeAssignmentInUse, clickAction.Path.ToString()));
			}
			else
			{
				_keyStrokeShortcutMap.Add(shortcut, clickAction);
			}
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
				Platform.Log(String.Format(SR.ExceptionMouseToolHasNoAssignment, activateMouseTool.GetType().FullName));
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
		/// <remarks>
		/// The tool's attributes are inspected and its behaviour determined.
		/// </remarks>
		public void RegisterImageViewerTool(ITool tool)
		{
			if (tool is MouseImageViewerTool)
			{
				MouseImageViewerTool mouseTool = tool as MouseImageViewerTool;
				RegisterMouseToolButton(mouseTool);
				RegisterModifiedMouseToolButton(mouseTool);
				RegisterMouseWheelShortcuts(mouseTool);
			}

			foreach (IAction action in tool.Actions)
			{
				if (action is IClickAction)
					RegisterKeyboardShortcut(action as IClickAction);
			}
		}

		#region IViewerShortcutManager Members

		/// <summary>
		/// Gets the <see cref="IClickAction"/> associated with a shortcut.
		/// </summary>
		/// <param name="shortcut">The shortcut for which an <see cref="IClickAction"/> is to be found.</param>
		/// <returns>An <see cref="IClickAction"/> or null.</returns>
		public IClickAction GetKeyboardAction(KeyboardButtonShortcut shortcut)
		{
			if (_keyStrokeShortcutMap.ContainsKey(shortcut))
				return _keyStrokeShortcutMap[shortcut];

			return null;
		}

		/// <summary>
		/// Gets the <see cref="IMouseButtonHandler"/> associated with a shortcut.
		/// </summary>
		/// <param name="shortcut">The shortcut for which an <see cref="IMouseButtonHandler"/> is to be found.</param>
		/// <returns>An <see cref="IMouseButtonHandler"/> or null.</returns>
		public IMouseButtonHandler GetMouseButtonHandler(MouseButtonShortcut shortcut)
		{
			if (_activeMouseButtonShortcutMap.ContainsKey(shortcut))
				return _activeMouseButtonShortcutMap[shortcut];

			return null;
		}

		/// <summary>
		/// Gets the <see cref="IMouseWheelHandler"/> associated with a shortcut.
		/// </summary>
		/// <param name="shortcut">The shortcut for which an <see cref="IMouseWheelHandler"/> is to be found.</param>
		/// <returns>An <see cref="IMouseWheelHandler"/> or null.</returns>
		public IMouseWheelHandler GetMouseWheelHandler(MouseWheelShortcut shortcut)
		{
			if (_activeMouseWheelShortcutMap.ContainsKey(shortcut))
				return _activeMouseWheelShortcutMap[shortcut];

			return null;
		}

		#endregion
	}
}
