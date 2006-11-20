using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public sealed class KeyboardButtonShortcut
	{
		private XKeys _keyData;

		public KeyboardButtonShortcut(XKeys keyData)
		{
			_keyData = keyData;
		}

		private KeyboardButtonShortcut()
		{
		}

		public XKeys KeyData
		{
			get { return _keyData; }
		}

		public XKeys KeyCode
		{
			get { return _keyData & XKeys.KeyCode; }
		}

		public bool Control
		{
			get { return ((_keyData & XKeys.Control) == XKeys.Control); }
		}

		public bool Alt
		{
			get { return ((_keyData & XKeys.Alt) == XKeys.Alt); }
		}

		public bool Shift
		{
			get { return ((_keyData & XKeys.Shift) == XKeys.Shift); }
		}

		public override bool Equals(object obj)
		{
			if (obj is KeyboardButtonShortcut)
			{
				KeyboardButtonShortcut shortcut = (KeyboardButtonShortcut)obj;
				return (shortcut.KeyData == this.KeyData);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return _keyData.GetHashCode();
		}
	}
}
