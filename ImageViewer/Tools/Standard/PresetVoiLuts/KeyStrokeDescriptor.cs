#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	public sealed class KeyStrokeDescriptor : IEquatable<KeyStrokeDescriptor>, IEquatable<XKeys>
	{
		private readonly XKeys _keyStroke;

		internal KeyStrokeDescriptor(XKeys keyStroke)
		{
			_keyStroke = keyStroke;
		}

		public XKeys KeyStroke
		{
			get { return _keyStroke; }
		}

		public override string ToString()
		{
			if (_keyStroke == XKeys.None)
				return String.Format("({0})", SR.None);

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
			if (other == null)
				return false;

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
