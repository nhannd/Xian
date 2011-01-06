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
