using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Declares a click handler binding for a click action.
    /// </summary>
    /// <remarks>
    /// This attribute binds the click handler of the action specified by the action ID to
    /// the specified method on the target class.  The method name must
    /// refer to a public void method on the target class that takes no parameters.
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

        public override void Apply(IActionBuildingContext builder)
        {
            // check that the method exists, etc
            ValidateClickHandler(builder.ActionTarget, this.HandlerMethodName);

            ClickHandlerDelegate clickHandler =
                (ClickHandlerDelegate)Delegate.CreateDelegate(typeof(ClickHandlerDelegate), builder.ActionTarget, this.HandlerMethodName);
            ((ClickAction)builder.Action).SetClickHandler(clickHandler);
        }

        private void ValidateClickHandler(object target, string methodName)
        {
            MethodInfo info = target.GetType().GetMethod(
                methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                Type.EmptyTypes,
                null);

            if (info == null)
            {
                throw new ActionBuilderException(
                    string.Format(SR.ExceptionActionBuilderMethodDoesNotExist, methodName, target.GetType().FullName));
            }
        }
    }
}
