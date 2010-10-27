#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Enumeration of the modifier keys on a keyboard.
	/// </summary>
	[Flags]
	[TypeConverter(typeof (ModifierFlagsConverter))]
	public enum ModifierFlags
	{
		/// <summary>
		/// Represents no modifiers (the empty value).
		/// </summary>
		None = 0,

		/// <summary>
		/// The CTRL modifier.
		/// </summary>
		Control = 1,

		/// <summary>
		/// The ALT modifier.
		/// </summary>
		Alt = 2,

		/// <summary>
		/// The SHIFT modifier.
		/// </summary>
		Shift = 4
	}
}