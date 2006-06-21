using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Application.Actions
{
    /// <summary>
    /// Declares an observer binding for the enabled state of an action.
    /// </summary>
    /// <remarks>
    /// This attribute causes the enabled state of the action specified by the action ID to be
    /// bound to the state of the specified property on the tool.  The property name must
    /// refer to a public boolean property on the target tool that has get access.  The change event name
    /// must refer to a public event on the tool that will fire whenever the state of the property
    /// changes.
    /// </remarks>
    public class EnabledStateObserverAttribute : StateObserverAttribute
    {
        /// <summary>
        /// Attribute constructor
        /// </summary>
        /// <param name="actionID">The logical action identifier to which this attribute applies</param>
        /// <param name="propertyName">The name of the property to bind to</param>
        /// <param name="changeEventName">The name of the property change notification event to bind to</param>
        public EnabledStateObserverAttribute(string actionID, string propertyName, string changeEventName)
            : base(actionID, propertyName, changeEventName)
        {
        }

        internal override void Apply(IActionBuilder builder)
        {
            builder.Apply(this);
        }
    }
}
