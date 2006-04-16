using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    /// <summary>
    /// Defines a generic mechanism for binding to an arbitrary observable property of an object.
    /// An observable property is any property which has a corresponding change notification event.
    /// </summary>
    /// <typeparam name="T">The type of the property</typeparam>
    public interface IObservablePropertyBinding<T>
    {
        /// <summary>
        /// The event that provides notification of when the property value has changed.
        /// </summary>
        event EventHandler PropertyChanged;

        /// <summary>
        /// Access to the underlying property value.
        /// </summary>
        T PropertyValue { get; set; }
    }
}
