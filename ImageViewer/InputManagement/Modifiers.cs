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
	/// An object representing modifier keys (e.g. Control, Alt, Shift).
	/// </summary>
	/// <remarks>
	/// This class is intended for internal framework use only.
	/// </remarks>
	public sealed class Modifiers : IEquatable<Modifiers>, IEquatable<ModifierFlags>
	{
		private readonly ModifierFlags _modifierFlags;

		/// <summary>
		/// Constructor.
		/// </summary>
		public Modifiers(bool control, bool alt, bool shift)
		{
			_modifierFlags = ModifierFlags.None;

			_modifierFlags |= (control) ? ModifierFlags.Control : ModifierFlags.None;
			_modifierFlags |= (alt) ? ModifierFlags.Alt : ModifierFlags.None;
			_modifierFlags |= (shift) ? ModifierFlags.Shift : ModifierFlags.None;
		}

		/// <summary>
		/// Constructor that takes a <see cref="ModifierFlags"/> as input.
		/// </summary>
		public Modifiers(ModifierFlags modifierFlags)
		{
			_modifierFlags = modifierFlags;
		}

		/// <summary>
		/// Gets whether or not the Control key is down.
		/// </summary>
		public bool Control
		{
			get { return ((_modifierFlags & ModifierFlags.Control) == ModifierFlags.Control); } 
		}

		/// <summary>
		/// Gets whether or not the Alt key is down.
		/// </summary>
		public bool Alt
		{
			get { return ((_modifierFlags & ModifierFlags.Alt) == ModifierFlags.Alt); }
		}

		/// <summary>
		/// Gets whether or not the Shift key is down.
		/// </summary>
		public bool Shift
		{
			get { return ((_modifierFlags & ModifierFlags.Shift) == ModifierFlags.Shift); } 
		}

		/// <summary>
		/// Gets the state of all modifiers via a <see cref="ModifierFlags"/> enum.
		/// </summary>
		public ModifierFlags ModifierFlags
		{
			get { return _modifierFlags; }
		}

		/// <summary>
		/// Determines whether another object is equal to this object.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is Modifiers)
				return this.Equals((Modifiers)obj);
			else if (obj is ModifierFlags)
				return this.Equals((ModifierFlags) obj);

			return false;
		}

		#region IEquatable<Modifiers> Members

		/// <summary>
		/// Gets whether or not this object is equal to <paramref name="other"/>.
		/// </summary>
		public bool Equals(Modifiers other)
		{
			return other != null && this.ModifierFlags == other.ModifierFlags;
		}

		#endregion

		#region IEquatable<ModifierFlags> Members

		/// <summary>
		/// Gets whether or not this object is equal to <paramref name="other"/>.
		/// </summary>
		public bool Equals(ModifierFlags other)
		{
			return this.ModifierFlags == other;
		}

		#endregion

		/// <summary>
		/// Gets a hashcode for this object instance. 
		/// </summary>
		public override int GetHashCode()
		{
			return _modifierFlags.GetHashCode();
		}

		/// <summary>
		/// Gets a string describing the object instance.
		/// </summary>
		public override string ToString()
		{
			return _modifierFlags.ToString();
		}
	}
}
