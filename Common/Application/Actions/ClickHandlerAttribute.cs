using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Application.Actions
{
    /// <summary>
    /// Declares a click handler binding for a click action.
    /// </summary>
    /// <remarks>
    /// This attribute binds the click handler of the action specified by the action ID to
    /// the specified method on the tool class.  The method name must
    /// refer to a public void method on the target tool that has no parameters.
    /// </remarks>
    public class ClickHandlerAttribute : ActionDecoratorAttribute
    {
        private string _handlerMethod;

        public ClickHandlerAttribute(string actionID, string handlerMethod)
            :base(actionID)
        {
            _handlerMethod = handlerMethod;
        }

        public string HandlerMethodName { get { return _handlerMethod; } }

        internal override void Apply(IActionBuilder builder)
        {
            builder.Apply(this);
        }
    }
}
