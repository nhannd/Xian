using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	public class KeyStrokeDescriptor : IEquatable<KeyStrokeDescriptor>, IEquatable<XKeys>
	{
		private readonly XKeys _keyStroke;

		internal KeyStrokeDescriptor(XKeys keyStroke)
		{
			_keyStroke = keyStroke;
		}

		public XKeys KeyStroke
		{
			get
			{
				return _keyStroke;
			}
		}

		public override string ToString()
		{
			if (_keyStroke == XKeys.None)
				return String.Format("({0})", _keyStroke.ToString());

			return _keyStroke.ToString();
		}

		public override int GetHashCode()
		{
			return _keyStroke.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is KeyStrokeDescriptor)
				return this.Equals((KeyStrokeDescriptor)obj);

			if (obj is XKeys)
				return this.Equals((XKeys)obj);

			return false;
		}

		#region IEquatable<KeyStrokeDescriptor> Members

		public bool Equals(KeyStrokeDescriptor other)
		{
			return this.KeyStroke == other.KeyStroke;
		}

		#endregion

		#region IEquatable<XKeys> Members

		public bool Equals(XKeys other)
		{
			return _keyStroke == other;
		}

		#endregion

		public static implicit operator XKeys(KeyStrokeDescriptor descriptor)
		{
			return descriptor._keyStroke;
		}

		public static implicit operator KeyStrokeDescriptor(XKeys keyStroke)
		{
			return new KeyStrokeDescriptor(keyStroke);
		}
	}
}
