using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Declares a button action with the specifed action identifier and path hint.
    /// </summary>
    public class MenuActionAttribute : ClickActionAttribute
    {
        /// <summary>
        /// Attribute constructor
        /// </summary>
        /// <param name="actionID">The logical action identifier to associate with this action</param>
        /// <param name="pathHint">The suggested location of this action in the menu model</param>
        public MenuActionAttribute(string actionID, string pathHint)
            : base(actionID, pathHint)
        {
        }

        internal override void Apply(IActionBuildingContext builder)
        {
            ActionPath path = new ActionPath(this.Path, builder.ResourceResolver);
            builder.Action = new MenuAction(builder.ActionID, path, this.Flags, builder.ResourceResolver);
            ((ClickAction)builder.Action).SetKeyStroke(this.KeyStroke);
            builder.Action.Persistent = true;
            builder.Action.Label = path.LastSegment.LocalizedText;
        }
    }
}
