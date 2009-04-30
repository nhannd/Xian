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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Abstract base class for action attributes that declare state observers.
    /// </summary>
    public abstract class StateObserverAttribute : ActionDecoratorAttribute
    {
        private readonly string _observedProperty;
        private readonly string _observedChangeEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
		/// <param name="actionID">The unique identifer of the action.</param>
        /// <param name="observedProperty">The name of the property to bind to.</param>
        /// <param name="observedChangeEvent">The name of the event to bind to that notifies subscribers of changes in the property value.</param>
        protected StateObserverAttribute(string actionID, string observedProperty, string observedChangeEvent)
            : base(actionID)
        {
            _observedProperty = observedProperty;
            _observedChangeEvent = observedChangeEvent;
        }

        /// <summary>
        /// The name of the property to bind to.
        /// </summary>
        public string PropertyName { get { return _observedProperty; } }

        /// <summary>
        /// The name of the property change notification event to bind to.
        /// </summary>
        public string ChangeEventName { get { return _observedChangeEvent; } }

		/// <summary>
		/// Binds an <see cref="IAction"/> instance to the <paramref name="actionProperty"/> and 
		/// <paramref name="actionEvent"/> on the target object, via the specified <see cref="IActionBuildingContext"/>.
		/// </summary>
        protected void Bind<T>(IActionBuildingContext builder, string actionProperty, string actionEvent)
        {
            ValidateProperty(builder.ActionTarget, this.PropertyName, typeof(T));
            ValidateEvent(builder.ActionTarget, this.ChangeEventName);

            IObservablePropertyBinding<T> toolBinding = new DynamicObservablePropertyBinding<T>(builder.ActionTarget, this.PropertyName, this.ChangeEventName);
            IObservablePropertyBinding<T> actionBinding = new DynamicObservablePropertyBinding<T>(builder.Action, actionProperty, actionEvent);

            ObservablePropertyCoupler<T>.Couple(toolBinding, actionBinding);
        }

		/// <summary>
		/// Validates the event that is to be bound to exists in the target object.
		/// </summary>
		protected void ValidateEvent(object target, string eventName)
        {
            EventInfo info = target.GetType().GetEvent(eventName);
            if (info == null)
            {
                throw new ActionBuilderException(
                    string.Format(SR.ExceptionActionBuilderEventDoesNotExist, eventName, target.GetType().FullName));
            }
        }

		/// <summary>
		/// Validates the property that is to be bound to exists in the target object.
		/// </summary>
		protected void ValidateProperty(object target, string propertyName, Type type)
        {
            PropertyInfo info = target.GetType().GetProperty(propertyName, type);
            if (info == null)
            {
                throw new ActionBuilderException(
                    string.Format(SR.ExceptionActionBuilderPropertyDoesNotExist, propertyName, target.GetType().FullName));
            }

            MethodInfo getter = info.GetGetMethod();
            if (getter == null)
            {
                throw new ActionBuilderException(
                    string.Format(SR.ExceptionActionBuilderPropertyDoesNotHavePublicGetMethod, propertyName, target.GetType().FullName));
            }
        }
    }
}
