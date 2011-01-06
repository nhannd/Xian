#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Common.Actions
{
    /// <summary>
	/// Interface representing a compiled set of actions returned by <see cref="XmlActionCompiler{TActionContext, TSchemaContext}"/>.
    /// </summary>
    public interface IActionSet<T>
    {
        /// <summary>
        /// Execute a set of actions.
        /// </summary>
		/// <param name="context">An implementation specific context for the actions.</param>
        /// <returns>A <see cref="TestResult"/> object describing the results.</returns>
        TestResult Execute(T context);
    }
}