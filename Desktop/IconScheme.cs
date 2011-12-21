#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Defines possible icon schemes.
	/// </summary>
	/// <remarks>
	/// The use of icon schemes has been deprecated in favour of extensible application GUI themes.
	/// </remarks>
	[Flags]
	[Obsolete("The use of icon schemes has been deprecated in favour of extensible application GUI themes")]
	public enum IconScheme
	{
		/// <summary>
		/// Colour icons.
		/// </summary>
		Colour,

		/// <summary>
		/// Monochrome icons.
		/// </summary>
		Monochrome
	}
}