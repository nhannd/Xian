using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Application.Actions
{
    /// <summary>
    /// Abstract base class for the set of attributes that are used to decorate an action
    /// once it has been declared by an <see cref="ActionInitiatorAttribute"/>.
    /// </summary>
    public abstract class ActionDecoratorAttribute : ActionAttribute
    {
        public ActionDecoratorAttribute(string actionID)
            : base(actionID)
        {
        }
    }
}
