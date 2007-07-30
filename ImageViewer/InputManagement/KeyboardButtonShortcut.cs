using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public sealed class KeyboardButtonShortcut : IEquatable<KeyboardButtonShortcut>, IEquatable<XKeys>
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

		public override int GetHashCode()
		{
			return _keyData.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is KeyboardButtonShortcut)
			{
				return this.Equals(obj as KeyboardButtonShortcut);
			}
			if (obj is XKeys)
			{
				return this.Equals((XKeys)obj);
			}

			return false;
		}

		#region IEquatable<KeyboardButtonShortcut> Members

		public bool Equals(KeyboardButtonShortcut other)
		{
			return (other != null && other.KeyData == this.KeyData);
		}

		#endregion

		#region IEquatable<XKeys> Members

		public bool Equals(XKeys other)
		{
			return this.KeyData == other;
		}

		#endregion

		public override string ToString()
		{
			return _keyData.ToString();
		}
	}
}
