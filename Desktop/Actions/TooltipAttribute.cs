using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Declares a tooltip message to associate with an action.
    /// </summary>
    public class TooltipAttribute : ActionDecoratorAttribute
    {
        private string _tooltip;

        /// <summary>
        /// Attribute constructor
        /// </summary>
        /// <param name="actionID">The logical action identifier to which this attribute applies</param>
        /// <param name="tooltip">The tooltip message to associate with the action</param>
        public TooltipAttribute(string actionID, string tooltip)
            :base(actionID)
        {
            _tooltip = tooltip;
        }

        /// <summary>
        /// The tooltip message
        /// </summary>
        public string TooltipText { get { return _tooltip; } }

        public override void Apply(IActionBuildingContext builder)
        {
            // assert _action != null
            builder.Action.Tooltip = builder.ResourceResolver.LocalizeString(this.TooltipText);
        }
    }
}
