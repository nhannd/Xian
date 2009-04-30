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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// Represents the current message object's (e.g. <see cref="MouseWheelMessage"/>) state.
	/// </summary>
	/// <seealso cref="MouseWheelMessage"/>
	public sealed class MouseWheelShortcut : IEquatable<MouseWheelShortcut>, IEquatable<Modifiers>
	{
		private readonly Modifiers _modifiers;
		private readonly string _description;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseWheelShortcut()
			: this(false, false, false)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseWheelShortcut(Modifiers modifiers)
		{
			_modifiers = modifiers ?? new Modifiers(ModifierFlags.None);
			_description = String.Format(SR.FormatMouseWheelShortcutDescription, _modifiers.ToString());
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseWheelShortcut(ModifierFlags modifierFlags)
			: this(new Modifiers(modifierFlags))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public MouseWheelShortcut(bool control, bool alt, bool shift)
			: this(new Modifiers(control, alt, shift))
		{
		}

		/// <summary>
		/// Gets the state of the modifier keys as a <see cref="ModifierFlags"/>.
		/// </summary>
		public Modifiers Modifiers
		{
			get { return _modifiers; }
		}

		/// <summary>
		/// Determines if another object instance is equal to this one.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is MouseWheelShortcut)
				return this.Equals((MouseWheelShortcut)obj);
			else if (obj is Modifiers)
				return this.Equals((Modifiers) obj);

			return false;
		}

		#region IEquatable<MouseWheelShortcut> Members

		/// <summary>
		/// Gets whether or not this object is equal to <paramref name="other"/>.
		/// </summary>
		public bool Equals(MouseWheelShortcut other)
		{
			return other != null && Modifiers.Equals(other.Modifiers);
		}

		#endregion

		#region IEquatable<Modifiers> Members

		/// <summary>
		/// Gets whether or not this object is equal to <paramref name="other"/>.
		/// </summary>
		public bool Equals(Modifiers other)
		{
			return other != null && Modifiers.Equals(other);
		}

		#endregion

		/// <summary>
		/// Gets a hash code for this object instance.
		/// </summary>
		public override int GetHashCode()
		{
			return _modifiers.GetHashCode();
		}

		/// <summary>
		/// Gets a string describing this object instance.
		/// </summary>
		public override string ToString()
		{
			return _description;
		}
	}
}
