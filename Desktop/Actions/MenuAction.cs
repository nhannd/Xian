#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Models a menu item action.
    /// </summary>
    public class MenuAction : ClickAction
    {
        /// <summary>.
        /// Constructor
        /// </summary>
        /// <param name="actionID">The fully qualified action ID.</param>
        /// <param name="path">The action path.</param>
        /// <param name="flags">Flags that control the style of the action.</param>
        /// <param name="resourceResolver">A resource resolver that will be used to resolve text and image resources.</param>
        public MenuAction(string actionID, ActionPath path, ClickActionFlags flags, IResourceResolver resourceResolver)
            : base(actionID, path, flags, resourceResolver)
        {
        }
    }
}
