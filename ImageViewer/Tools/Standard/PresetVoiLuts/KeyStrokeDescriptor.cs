#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
