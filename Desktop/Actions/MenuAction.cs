using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Models a menu item action, and acts as a binding between the user-interface and a tool.
    /// </summary>
    public class MenuAction : ClickAction
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="actionID"></param>
        /// <param name="path"></param>
        /// <param name="target"></param>
        /// <param name="flags"></param>
        public MenuAction(string actionID, ActionPath path, ClickActionFlags flags, IResourceResolver resourceResolver)
            : base(actionID, path, flags, resourceResolver)
        {
        }
    }
}
