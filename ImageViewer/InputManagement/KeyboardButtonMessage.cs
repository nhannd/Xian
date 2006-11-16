using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public sealed class KeyboardButtonMessage : IInputMessage
	{
		public enum ButtonActions { Down, Up, Press };

		private ButtonActions _buttonAction;
		private KeyboardButtonShortcut _buttonShortcut;

		public KeyboardButtonMessage(XKeys keyData, ButtonActions buttonAction)
		{
			_buttonAction = buttonAction;
			_buttonShortcut = new KeyboardButtonShortcut(keyData);
		}

		private KeyboardButtonMessage()
		{
		}

		public ButtonActions ButtonAction
		{
			get { return _buttonAction; }
		}

		public KeyboardButtonShortcut Shortcut
		{
			get { return _buttonShortcut; }
		}
	}
}
