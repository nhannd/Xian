using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Abstract base class for the set of attributes that are used to decorate an action
    /// once it has been declared by an <see cref="ActionInitiatorAttribute"/>.
    /// </summary>
    public abstract class ActionDecoratorAttribute : ActionAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="actionID"></param>
        public ActionDecoratorAttribute(string actionID)
            : base(actionID)
        {
        }
    }
}
