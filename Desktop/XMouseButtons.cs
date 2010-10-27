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
	/// Enumeration of the (potentially) available mouse buttons.
	/// </summary>
	[Flags]
	[TypeConverter(typeof(XMouseButtonsConverter))]
	public enum XMouseButtons
	{
		/// <summary>
		/// Represents no mouse buttons (the empty value).
		/// </summary>
		None = 0x00000000,

		/// <summary>
		/// The left mouse button (mouse button 1).
		/// </summary>
		Left = 0x00100000,

		/// <summary>
		/// The right mouse button (mouse button 2).
		/// </summary>
		Right = 0x00200000,

		/// <summary>
		/// The middle mouse button (mouse button 3).
		/// </summary>
		Middle = 0x00400000,

		/// <summary>
		/// The first X mouse button (mouse button 4).
		/// </summary>
		XButton1 = 0x00800000,

		/// <summary>
		/// The second X mouse button (mouse button 5).
		/// </summary>
		XButton2 = 0x01000000
	}
}