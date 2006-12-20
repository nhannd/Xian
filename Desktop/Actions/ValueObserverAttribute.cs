using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Abstract base class for action attributes that declare value observers.
	/// </summary>
	public abstract class ValueObserverAttribute : ActionDecoratorAttribute
	{
		private string _observedProperty;
		private string _observedChangeEvent;

		public ValueObserverAttribute(string actionID, string observedProperty, string observedChangeEvent)
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

    }
}
