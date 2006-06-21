using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Application.Actions
{
    /// <summary>
    /// Declares a button action on a tool class.
    /// </summary>
    public class ButtonActionAttribute : ClickActionAttribute
    {
        /// <summary>
        /// Attribute constructor
        /// </summary>
        /// <param name="actionID">The logical action identifier to associate with this action</param>
        /// <param name="pathHint">The suggested location of this action in the toolbar model</param>
        public ButtonActionAttribute(string actionID, string pathHint)
            : base(actionID, pathHint, ActionCategory.ToolbarAction)
        {
        }

        internal override void Apply(IActionBuilder builder)
        {
            builder.Apply(this);
        }
    }
}
