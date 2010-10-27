#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Set of flags that customize the behaviour of click actions.
    /// </summary>
    [Flags]
    public enum ClickActionFlags
    {
        /// <summary>
        /// Default value.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Specifies that the action is a "check" action (e.g. that it has toggle behaviour).
        /// </summary>
        CheckAction = 0x01,

		/// <summary>
		/// Specifies that parents of the action should be checked when the action is checked.
		/// </summary>
		CheckParents = 0x02
    }
}
