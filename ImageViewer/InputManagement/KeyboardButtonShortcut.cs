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

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// Represents the current message object's (e.g. <see cref="KeyboardButtonMessage"/>) state.
	/// </summary>
	/// <remarks>
	/// This class is intended for internal framework use only.
	/// </remarks>
	/// <seealso cref="KeyboardButtonDownPreview"/>
	/// <seealso cref="KeyboardButtonMessage"/>
	public sealed class KeyboardButtonShortcut : IEquatable<KeyboardButtonShortcut>, IEquatable<XKeys>
	{
		private readonly XKeys _keyData;

		/// <summary>
		/// Constructor.
		/// </summary>
		public KeyboardButtonShortcut(XKeys keyData)
		{
			_keyData = keyData;
		}

		/// <summary>
		/// Gets the Key Data, in its entirety.
		/// </summary>
		public XKeys KeyData
		{
			get { return _keyData; }
		}

		/// <summary>
		/// Gets the Key Code (modifiers removed), which is extracted from the <see cref="KeyData"/>.
		/// </summary>
		public XKeys KeyCode
		{
			get { return _keyData & XKeys.KeyCode; }
		}

		/// <summary>
		/// Indicates whether the Control key is down.
		/// </summary>
		public bool Control
		{
			get { return ((_keyData & XKeys.Control) == XKeys.Control); }
		}

		/// <summary>
		/// Indicates whether the Alt key is down.
		/// </summary>
		public bool Alt
		{
			get { return ((_keyData & XKeys.Alt) == XKeys.Alt); }
		}

		/// <summary>
		/// Indicates whether the Shift key is down.
		/// </summary>
		public bool Shift
		{
			get { return ((_keyData & XKeys.Shift) == XKeys.Shift); }
		}

		/// <summary>
		/// Gets a hashcode for this object instance.
		/// </summary>
		public override int GetHashCode()
		{
			return _keyData.GetHashCode();
		}

		/// <summary>
		/// Determines if this object is equal to another.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is KeyboardButtonShortcut)
			{
				return this.Equals((KeyboardButtonShortcut)obj);
			}
			if (obj is XKeys)
			{
				return this.Equals((XKeys)obj);
			}

			return false;
		}

		#region IEquatable<KeyboardButtonShortcut> Members

		/// <summary>
		/// Gets whether or not this object is equal to <paramref name="other"/>.
		/// </summary>
		public bool Equals(KeyboardButtonShortcut other)
		{
			return (other != null && other.KeyData == this.KeyData);
		}

		#endregion

		#region IEquatable<XKeys> Members

		/// <summary>
		/// Gets whether or not this object is equal to <paramref name="other"/>.
		/// </summary>
		public bool Equals(XKeys other)
		{
			return this.KeyData == other;
		}

		#endregion

		/// <summary>
		/// Gets a string describing this object instance.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _keyData.ToString();
		}
	}
}
