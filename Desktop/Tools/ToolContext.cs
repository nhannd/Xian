#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Base class for all tool contexts.
    /// </summary>
    /// <remarks>
	/// Developers are encouraged to inherit this class 
	/// rather than implement <see cref="IToolContext"/> directly.
	/// </remarks>
    public abstract class ToolContext : IToolContext
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		protected ToolContext()
		{
		}
    }
}
