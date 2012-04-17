#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Tools
{
    /// <summary>
    /// Defines the external interface to a tool set, which manages a set of tools.
    /// </summary>
    public interface IToolSet : IDisposable
    {
        /// <summary>
        /// Gets the tools contained in this tool set.
        /// </summary>
        ITool[] Tools { get; }

		/// <summary>
		/// Finds the tool of the specified type.
		/// </summary>
		/// <typeparam name="TTool"></typeparam>
		/// <returns>The instance of the tool of the specified type, or null if no such exists.</returns>
		TTool Find<TTool>()
			where TTool: ITool;
        
        /// <summary>
        /// Returns the union of all actions defined by all tools in this tool set.
        /// </summary>
        IActionSet Actions { get; }
    }
}
