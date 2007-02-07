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
	public class ViewerShortcutManager : IViewerShortcutManager
	{
		private KeyStrokeSettings _keyStrokeSettings;

		private Dictionary<MouseImageViewerTool, XMouseButtons> _mouseToolButtonMap;
		private Dictionary<MouseButtonShortcut, IMouseButtonHandler> _activeMouseButtonShortcutMap;
		private Dictionary<MouseWheelShortcut, IMouseWheelHandler> _activeMouseWheelShortcutMap;
		private Dictionary<KeyboardButtonShortcut, IClickAction> _keyStrokeShortcutMap;

		public ViewerShortcutManager()
		{
			_keyStrokeSettings = new KeyStrokeSettings();

			_mouseToolButtonMap = new Dictionary<MouseImageViewerTool, XMouseButtons>();
			_activeMouseButtonShortcutMap = new Dictionary<MouseButtonShortcut, IMouseButtonHandler>();
			_activeMouseWheelShortcutMap = new Dictionary<MouseWheelShortcut, IMouseWheelHandler>();
			_keyStrokeShortcutMap = new Dictionary<KeyboardButtonShortcut, IClickAction>();
		}

		private void RegisterMouseToolButton(MouseImageViewerTool mouseTool)
		{
			try
			{
				object[] buttonAssignment = mouseTool.GetType().GetCustomAttributes(typeof(MouseToolButtonAttribute), true);
				if (buttonAssignment == null || buttonAssignment.Length == 0)
					throw new InvalidOperationException(String.Format(SR.ExceptionMouseToolShouldHaveDefault, mouseTool.GetType().FullName));

				MouseToolButtonAttribute attribute = buttonAssignment[0] as MouseToolButtonAttribute;
				_mouseToolButtonMap.Add(mouseTool, attribute.MouseButton);

				if (attribute.InitiallyActive)
					ActivateMouseTool(mouseTool, false);

				mouseTool.ActivationChanged += new EventHandler(OnMouseToolActivationChanged);

			}
			catch (Exception e)
			{
				Platform.Log(e, LogLevel.Warn);
			}
		}

		private void RegisterModifiedMouseToolButton(MouseImageViewerTool mouseTool)
		{
			try
			{
				object[] modifiedButtonAssignments = mouseTool.GetType().GetCustomAttributes(typeof(ModifiedMouseToolButtonAttribute), true);
				if (modifiedButtonAssignments == null || modifiedButtonAssignments.Length == 0)
					return;

				ModifiedMouseToolButtonAttribute attribute = modifiedButtonAssignments[0] as ModifiedMouseToolButtonAttribute;

				if (attribute.Shortcut.Modifiers.ModifierFlags == ModifierFlags.None)
					throw new InvalidOperationException(String.Format(SR.ExceptionAdditionalMouseToolAssignmentsMustBeModified, mouseTool.GetType().FullName));

				if (_activeMouseButtonShortcutMap.ContainsKey(attribute.Shortcut))
					throw new InvalidOperationException(String.Format(SR.ExceptionMouseToolAssignmentInUse, mouseTool.GetType().FullName));

				_activeMouseButtonShortcutMap.Add(attribute.Shortcut, mouseTool);
			}
			catch (Exception e)
			{
				Platform.Log(e);
			}
		}

		private void RegisterMouseWheelShortcuts(MouseImageViewerTool mouseTool)
		{
			object[] mouseWheelControlAssignments = mouseTool.GetType().GetCustomAttributes(typeof(MouseWheelControlAttribute), true);
			if (mouseWheelControlAssignments == null || mouseWheelControlAssignments.Length == 0)
				return;

			foreach (MouseWheelControlAttribute attribute in mouseWheelControlAssignments)
			{
				try
				{
					if (_activeMouseWheelShortcutMap.ContainsKey(attribute.Shortcut))
						throw new InvalidOperationException(String.Format(SR.ExceptionMouseWheelAssignmentInUse, mouseTool));

					IMouseWheelHandler handler = new MouseWheelHandler(mouseTool, attribute.WheelIncrementDelegateName, attribute.WheelDecrementDelegateName);
					_activeMouseWheelShortcutMap.Add(attribute.Shortcut, handler);
				}
				catch (Exception e)
				{
					Platform.Log(e);
				}
			}
		}

		private void RegisterKeyboardShortcut(IClickAction clickAction)
		{
			try
			{
				if (_keyStrokeSettings.Settings != null && _keyStrokeSettings.Settings.Count > 0)
				{
					KeyStrokeSetting keyOverride = _keyStrokeSettings.Settings.Find(delegate(KeyStrokeSetting setting) { return clickAction.Path.ToString() == setting.ActionPath; });
					if (keyOverride != null)
						clickAction.KeyStroke = keyOverride.KeyStroke;
				}

				if (clickAction.KeyStroke == 0)
					return;

				KeyboardButtonShortcut shortcut = new KeyboardButtonShortcut(clickAction.KeyStroke);

				if (_keyStrokeShortcutMap.ContainsKey(shortcut))
					throw new InvalidOperationException(String.Format(SR.ExceptionKeyStrokeAssignmentInUse, clickAction.Path.ToString()));

				_keyStrokeShortcutMap.Add(shortcut, clickAction);
			}
			catch (Exception e)
			{
				Platform.Log(e);
			}
		}

		private void OnMouseToolActivationChanged(object sender, EventArgs e)
		{
			MouseImageViewerTool mouseTool = (MouseImageViewerTool)sender;

			if (mouseTool.Active)
				ActivateMouseTool(mouseTool, true);
			else
				DeactivateMouseTool(mouseTool);
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
			try
			{
				if (!_mouseToolButtonMap.ContainsKey(activateMouseTool))
					throw new InvalidOperationException(String.Format(SR.ExceptionMouseToolHasNoAssignment, activateMouseTool.GetType().FullName));

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
			catch (Exception e)
			{
				Platform.Log(e);
			}
		}

		public void RegisterMouseShortcuts(IEnumerable<ITool> tools)
		{
			foreach (ITool tool in tools)
			{
				if (!(tool is MouseImageViewerTool))
					continue;

				MouseImageViewerTool mouseTool = tool as MouseImageViewerTool;

				RegisterMouseToolButton(mouseTool);
				RegisterModifiedMouseToolButton(mouseTool);
				RegisterMouseWheelShortcuts(mouseTool);
			}
		}

		public void RegisterKeyboardShortcuts(ActionModelNodeList nodes)
		{
			foreach (ActionModelNode node in nodes)
			{
				if (node.IsLeaf)
				{
					IAction action = node.Action;

					if (action is IClickAction)
						RegisterKeyboardShortcut(action as IClickAction);
				}
				else
				{
					RegisterKeyboardShortcuts(node.ChildNodes);
				}
			}
		}

		#region IViewerShortcutManager Members

		public void ChangeMouseToolAssignment(MouseImageViewerTool mouseTool, XMouseButtons button)
		{
			Platform.CheckForNullReference(mouseTool, "mouseTool");

			if (!_mouseToolButtonMap.ContainsKey(mouseTool))
			{
				_mouseToolButtonMap.Add(mouseTool, button);
			}
			else
			{
				if (_mouseToolButtonMap[mouseTool] == button)
					return;

				//remove it from the active map, if it's there.
				DeactivateMouseTool(mouseTool);

				_mouseToolButtonMap[mouseTool] = button;
			}

			//Add it to the active map with the new key.
			if (mouseTool.Active)
				ActivateMouseTool(mouseTool, true);
		}

		public IClickAction GetKeyboardAction(KeyboardButtonShortcut shortcut)
		{
			if (_keyStrokeShortcutMap.ContainsKey(shortcut))
				return _keyStrokeShortcutMap[shortcut];

			return null;
		}

		public IMouseButtonHandler GetMouseButtonHandler(MouseButtonShortcut shortcut)
		{
			if (_activeMouseButtonShortcutMap.ContainsKey(shortcut))
				return _activeMouseButtonShortcutMap[shortcut];

			return null;
		}

		public IMouseWheelHandler GetMouseWheelHandler(MouseWheelShortcut shortcut)
		{
			if (_activeMouseWheelShortcutMap.ContainsKey(shortcut))
				return _activeMouseWheelShortcutMap[shortcut];

			return null;
		}

		#endregion
	}
}
