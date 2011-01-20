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
