using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Application.Actions
{
    /// <summary>
    /// Declares a button action on a tool class.
    /// </summary>
    public class MenuActionAttribute : ClickActionAttribute
    {
        /// <summary>
        /// Attribute constructor
        /// </summary>
        /// <param name="actionID">The logical action identifier to associate with this action</param>
        /// <param name="pathHint">The suggested location of this action in the menu model</param>
        public MenuActionAttribute(string actionID, string pathHint)
            : base(actionID, pathHint, ActionCategory.MenuAction)
        {
        }

        internal override void Apply(IActionBuilder builder)
        {
            builder.Apply(this);
        }
    }
}
