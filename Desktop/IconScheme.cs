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
	/// Enumeration for different standard icon sizes.
	/// </summary>
	public enum IconSize
	{
		/// <summary>
		/// Small icon.
		/// </summary>
		Small, 
		/// <summary>
		/// Medium icon.
		/// </summary>
		Medium, 
		/// <summary>
		/// Large icon.
		/// </summary>
		Large
	};

	/// <summary>
    /// Defines possible icon schemes.
    /// </summary>
    [Flags]
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
