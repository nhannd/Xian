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
	/// Delegate used to get a property of type <typeparamref name="T"/>.
	/// </summary>
	internal delegate T PropertyGetDelegate<T>();

	/// <summary>
	/// Delegate used to set a property of type <typeparamref name="T"/>.
	/// </summary>
	internal delegate void PropertySetDelegate<T>(T val);

	/// <summary>
	/// Provides a dynamic implementation of <see cref="IObservablePropertyBinding{T}" />.
	/// </summary>
	/// <typeparam name="T">The type of the underlying property.</typeparam>
	/// <remarks>
	/// This class can be used for runtime binding to an arbitrary property and corresponding change
	/// notification event of a target object.  Binding is accomplished using reflection, which means
	/// that compile-time knowledge of the type of the target object is not needed.
	/// </remarks>
	internal class DynamicObservablePropertyBinding<T> : IObservablePropertyBinding<T>
	{
		private PropertyGetDelegate<T> _getDelegate;
		private PropertySetDelegate<T> _setDelegate;

		private object _target;
		private string _propertyName;
		private string _propertyChangedEventName;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="target">The target object to bind to.</param>
		/// <param name="propertyName">The name of the property to bind to on the target object.</param>
		/// <param name="propertyChangedEventName">The name of a change notification event on the target object.</param>
		public DynamicObservablePropertyBinding(object target, string propertyName, string propertyChangedEventName)
		{
			_target = target;
			_propertyName = propertyName;
			_propertyChangedEventName = propertyChangedEventName;
		}
        
		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public T PropertyValue
		{
			get
			{
				if (_getDelegate == null)
				{
					_getDelegate = CreatePropertyGetDelegate(_target, _propertyName);
				}
				return _getDelegate();
			}
			set
			{
				if (_setDelegate == null)
				{
					_setDelegate = CreatePropertySetDelegate(_target, _propertyName);
				}
				_setDelegate(value);
			}
		}

		/// <summary>
		/// Event that is fired when the <see cref="PropertyValue"/> has changed.
		/// </summary>
		public event EventHandler PropertyChanged
		{
			add
			{
				AddEventHandler(value, _target, _propertyChangedEventName);
			}
			remove
			{
				RemoveEventHandler(value, _target, _propertyChangedEventName);
			}
		}

		/// <summary>
		/// Creates a delegate that can get the specified property on the specified target object.
		/// </summary>
		/// <param name="target">The target object.</param>
		/// <param name="propertyName">The name of the property to bind.</param>
		/// <returns>A delegate that can be used to get the property.</returns>
		private static PropertyGetDelegate<T> CreatePropertyGetDelegate(object target, string propertyName)
		{
			PropertyInfo propertyInfo = target.GetType().GetProperty(propertyName);
			MethodInfo propertyGetter = propertyInfo.GetGetMethod();

			return (PropertyGetDelegate<T>)Delegate.CreateDelegate(typeof(PropertyGetDelegate<T>), target, propertyGetter);
		}

		/// <summary>
		/// Creates a delegate that can set the specified property on the specified target object.
		/// </summary>
		/// <param name="target">The target object.</param>
		/// <param name="propertyName">The name of the property to bind.</param>
		/// <returns>A delegate that can be used to set the property.</returns>
		private static PropertySetDelegate<T> CreatePropertySetDelegate(object target, string propertyName)
		{
			PropertyInfo propertyInfo = target.GetType().GetProperty(propertyName);
			MethodInfo propertySetter = propertyInfo.GetSetMethod();

			return (PropertySetDelegate<T>)Delegate.CreateDelegate(typeof(PropertySetDelegate<T>), target, propertySetter.Name);
		}

		/// <summary>
		/// Adds the specified handler to the specified event on the target object.
		/// </summary>
		/// <param name="handler">An event handler.</param>
		/// <param name="target">The target object.</param>
		/// <param name="eventName">The name of the event to add the handler to.</param>
		private static void AddEventHandler(EventHandler handler, object target, string eventName)
		{
			EventInfo eventInfo = target.GetType().GetEvent(eventName);
			eventInfo.AddEventHandler(target, handler);
		}

		/// <summary>
		/// Removes the specified handler from the specified event on the target object.
		/// </summary>
		/// <param name="handler">An event handler.</param>
		/// <param name="target">The target object.</param>
		/// <param name="eventName">The name of the event to remove the handler from.</param>
		private static void RemoveEventHandler(EventHandler handler, object target, string eventName)
		{
			EventInfo eventInfo = target.GetType().GetEvent(eventName);
			eventInfo.RemoveEventHandler(target, handler);
		}
	}
}