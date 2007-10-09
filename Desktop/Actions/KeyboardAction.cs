using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Models an action that is invoked via the keyboard.
    /// </summary>
	public class KeyboardAction : ClickAction
	{
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actionID"></param>
        /// <param name="path"></param>
        /// <param name="flags"></param>
        /// <param name="resourceResolver"></param>
		public KeyboardAction(string actionID, ActionPath path, ClickActionFlags flags, IResourceResolver resourceResolver)
            : base(actionID, path, flags, resourceResolver)
        {
        }
	}
}
