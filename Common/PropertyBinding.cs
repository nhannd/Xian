using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    public delegate T PropertyGetDelegate<T>();
    public delegate void PropertySetDelegate<T>(T val);

    public class PropertyBinding<T>
    {
        private PropertyGetDelegate<T> _getDelegate;
        private PropertySetDelegate<T> _setDelegate;

        public PropertyBinding(PropertyGetDelegate<T> getDelegate, PropertySetDelegate<T> setDelegate)
        {
            _getDelegate = getDelegate;
            _setDelegate = setDelegate;
        }

        public T PropertyValue
        {
            get
            {
                if (_getDelegate == null)
                    throw new NotSupportedException();
                return _getDelegate();
            }
            set
            {
                if (_setDelegate == null)
                    throw new NotSupportedException();
                _setDelegate(value);
            }
        }
    }
}
