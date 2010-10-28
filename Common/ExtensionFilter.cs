#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Common
{
    /// <summary>
    /// An abstract base class for extension filters.  
    /// </summary>
    /// <remarks>
	/// Extension filters are used to filter the extension points returned by 
	/// one of the <b>CreateExtensions</b> methods.  Subclasses of this
	/// class implement specific types of filters.
	/// </remarks>
    public abstract class ExtensionFilter
    {
        /// <summary>
        /// Tests the specified extension against the criteria of this filter.
        /// </summary>
        /// <param name="extension">The extension to test.</param>
        /// <returns>True if the extension meets the criteria, false otherwise.</returns>
        public abstract bool Test(ExtensionInfo extension);
    }
}
