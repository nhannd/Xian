using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Application.Actions
{
    /// <summary>
    /// Models a menu item action, and acts as a binding between the user-interface and a tool.
    /// </summary>
    public class MenuAction : ClickAction
    {
        /// <summary>
        /// Internal constructor
        /// </summary>
        /// <param name="actionID"></param>
        /// <param name="path"></param>
        /// <param name="target"></param>
        /// <param name="flags"></param>
        internal MenuAction(string actionID, ActionPath path, object target, ClickActionFlags flags)
            : base(actionID, ActionCategory.MenuAction, path, target, flags)
        {
        }
    }
}
