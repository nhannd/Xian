using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public class ViewerShortcutManager
	{
		private Dictionary<MouseTool, XMouseButtons> _mouseToolButtonMap;
		private Dictionary<MouseButtonShortcut, IMouseButtonHandler> _activeMouseButtonShortcutMap;
		private Dictionary<MouseWheelShortcut, MouseWheelDelegatePair> _activeMouseWheelShortcutMap;
		private Dictionary<KeyboardButtonShortcut, IClickAction> _keyStrokeShortcutMap;

		public ViewerShortcutManager()
		{
			_mouseToolButtonMap = new Dictionary<MouseTool, XMouseButtons>();
			_activeMouseButtonShortcutMap = new Dictionary<MouseButtonShortcut, IMouseButtonHandler>();
			_activeMouseWheelShortcutMap = new Dictionary<MouseWheelShortcut, MouseWheelDelegatePair>();
			_keyStrokeShortcutMap = new Dictionary<KeyboardButtonShortcut, IClickAction>();
		}

		public void RegisterMouseShortcuts(ITool tool)
		{
			if (tool is MouseTool)
			{
				MouseTool mouseTool = (MouseTool)tool;

				object[] buttonAssignment = mouseTool.GetType().GetCustomAttributes(typeof(MouseToolButtonAttribute), true);
				if (buttonAssignment != null && buttonAssignment.Length > 0)
				{
					MouseToolButtonAttribute attribute = buttonAssignment[0] as MouseToolButtonAttribute;
					_mouseToolButtonMap.Add(mouseTool, attribute.MouseButton);

					if (attribute.InitiallyActive)
						ActivateMouseTool(mouseTool, false);

					mouseTool.ActivationChanged += new EventHandler(OnMouseToolActivationChanged);
				}
				else
				{
					Exception logException = new InvalidOperationException("A MouseTool must have a default button assignment.  The tool will have no effect.");
					Platform.Log(logException);
				}

				//These must be modified!!!
				object[] additionalButtonAssignments = tool.GetType().GetCustomAttributes(typeof(MouseButtonControlAttribute), true);
				if (additionalButtonAssignments != null && additionalButtonAssignments.Length > 0)
				{
					MouseButtonControlAttribute attribute = additionalButtonAssignments[0] as MouseButtonControlAttribute;

					if (!_activeMouseButtonShortcutMap.ContainsKey(attribute.ShortCut))
						_activeMouseButtonShortcutMap[attribute.ShortCut] = mouseTool;
				}
				else
				{
					Exception logException = new InvalidOperationException("Another tool is already using the specified mouse button assignment.  The assignment has been ignored.");
					Platform.Log(logException);
				}
			}

			object[] wheelAssignment = tool.GetType().GetCustomAttributes(typeof(MouseWheelControlAttribute), true);
			if (wheelAssignment != null && wheelAssignment.Length > 0)
			{
				MouseWheelControlAttribute attribute = wheelAssignment[0] as MouseWheelControlAttribute;

				try
				{
					_activeMouseWheelShortcutMap.Add(new MouseWheelShortcut(attribute.Modifiers), attribute.CreateDelegatePair(tool));
				}
				catch (Exception e)
				{
					Platform.Log(e);
				}
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
					{
						IClickAction clickAction = (IClickAction)action;
						KeyboardButtonShortcut shortcut = new KeyboardButtonShortcut(clickAction.KeyStroke);
						if (!_keyStrokeShortcutMap.ContainsKey(shortcut))
						{
							_keyStrokeShortcutMap.Add(shortcut, clickAction);
						}
					}
				}
				else
				{
					RegisterKeyboardShortcuts(node.ChildNodes);
				}
			}
		}

		public object this[IInputMessage message]
		{
			get
			{
				if (message is MouseButtonMessage)
				{
					MouseButtonShortcut shortcut = ((MouseButtonMessage)message).Shortcut;

					if (_activeMouseButtonShortcutMap.ContainsKey(shortcut))
						return _activeMouseButtonShortcutMap[shortcut];
				}
				else if (message is MouseWheelMessage)
				{
					MouseWheelMessage wheelMessage = (MouseWheelMessage)message;

					if (_activeMouseWheelShortcutMap.ContainsKey(wheelMessage.Shortcut))
					{
						MouseWheelDelegatePair delegates = _activeMouseWheelShortcutMap[wheelMessage.Shortcut];
						if (wheelMessage.WheelDelta > 0)
							return delegates.WheelDecrementDelegate;

						return delegates.WheelIncrementDelegate;
					}
				}
				else if (message is KeyboardButtonMessage)
				{
					KeyboardButtonMessage buttonMessage = message as KeyboardButtonMessage;
					if (buttonMessage.ButtonAction == KeyboardButtonMessage.ButtonActions.Down &&
						_keyStrokeShortcutMap.ContainsKey(buttonMessage.Shortcut))
					{
						return _keyStrokeShortcutMap[buttonMessage.Shortcut];
					}
				}

				return null;
			}
		}
		
		private void OnMouseToolActivationChanged(object sender, EventArgs e)
		{
			MouseTool mouseTool = (MouseTool)sender;
			if (mouseTool.Active)
				ActivateMouseTool(mouseTool, true);
		}

		private void ActivateMouseTool(MouseTool activateMouseTool, bool replaceExisting)
		{
			try
			{
				if (!_mouseToolButtonMap.ContainsKey(activateMouseTool))
					throw new Exception("The selected Mouse Tool does not exist in the dictionary.");

				MouseButtonShortcut shortcut = new MouseButtonShortcut(_mouseToolButtonMap[activateMouseTool]);

				if (_activeMouseButtonShortcutMap.ContainsKey(shortcut))
				{
					if (!replaceExisting)
						return;
					
					//!!Resolve this issue with the IMouseButtonHandler vs MouseTool
					MouseTool oldMouseTool = (MouseTool)_activeMouseButtonShortcutMap[shortcut];
					oldMouseTool.Active = false;
					_activeMouseButtonShortcutMap[shortcut] = activateMouseTool;
				}
				else
				{
					_activeMouseButtonShortcutMap.Add(shortcut, activateMouseTool);
					if (!activateMouseTool.Active)
						activateMouseTool.Active = true;
				}
			}
			catch (Exception e)
			{
				Platform.Log(e);
			}
		}
	}
}
