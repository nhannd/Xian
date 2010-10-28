#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections;

namespace ClearCanvas.Common.Scripting
{
    /// <summary>
    /// Defines the interface to an executable script returned by an instance of an <see cref="IScriptEngine"/>.
    /// </summary>
    public interface IExecutableScript
    {
        /// <summary>
        /// Executes this script, using the supplied values to initialize any variables in the script.
        /// </summary>
        /// <param name="context">The set of values to substitute into the script.</param>
        /// <returns>The return value of the script.</returns>
        object Run(IDictionary context);
    }
}
