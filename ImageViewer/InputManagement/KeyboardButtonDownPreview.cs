using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public sealed class KeyboardButtonDownPreview : IInputMessage
	{
		private KeyboardButtonShortcut _buttonShortcut;

		public KeyboardButtonDownPreview(XKeys keyData)
		{
			_buttonShortcut = new KeyboardButtonShortcut(keyData);
		}

		private KeyboardButtonDownPreview()
		{
		}

		public KeyboardButtonShortcut Shortcut
		{
			get { return _buttonShortcut; }
		}
	}
}
