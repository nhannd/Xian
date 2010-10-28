#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Reflection;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
    /// Declares a click handler binding for a click action.
    /// </summary>
    /// <remarks>
    /// This attribute binds the click handler of the action specified by the action ID to
    /// the specified method on the target class.  The method name must
    /// refer to a void method on the target class that takes no parameters.  
    /// </remarks>
    [Obsolete("Consider binding the click-handler directly in the MenuAction/ButtonAction/KeyboardAction attribute.")]
    public class ClickHandlerAttribute : ActionDecoratorAttribute
    {
        private string _handlerMethod;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="actionID">The logical action ID.</param>
		/// <param name="handlerMethod">The name of the click handler method.</param>
        public ClickHandlerAttribute(string actionID, string handlerMethod)
            :base(actionID)
        {
            _handlerMethod = handlerMethod;
        }

        /// <summary>
        /// Gets the name of the click handler method.
        /// </summary>
		public string HandlerMethodName { get { return _handlerMethod; } }

    	/// <summary>
		/// Applies this attribute to an <see cref="IAction"/> instance, via the specified <see cref="IActionBuildingContext"/>,
		/// by binding the appropriate handler method on the target object to the action.
    	/// </summary>
    	public override void Apply(IActionBuildingContext builder)
        {
            // check that the method exists, etc
            ValidateClickHandler(builder.ActionTarget, this.HandlerMethodName);

            ClickHandlerDelegate clickHandler =
                (ClickHandlerDelegate)Delegate.CreateDelegate(typeof(ClickHandlerDelegate), builder.ActionTarget, this.HandlerMethodName);
            ((ClickAction)builder.Action).SetClickHandler(clickHandler);
        }

        private static void ValidateClickHandler(object target, string methodName)
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
