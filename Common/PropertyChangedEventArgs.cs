using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    public delegate T PropertyQueryDelegate<T>();

    public class PropertyChangedEventArgs<T> : EventArgs
    {
        private T _oldValue;
        private T _newValue;

        public PropertyChangedEventArgs(T oldValue, T newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public T OldValue
        {
            get { return _oldValue; }
        }

        public T NewValue
        {
            get { return _newValue; }
        }
    }
}
