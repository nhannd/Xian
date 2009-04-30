#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
