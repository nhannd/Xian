using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ClearCanvas.Common
{
    delegate T PropertyGetDelegate<T>();
    delegate void PropertySetDelegate<T>(T val);

    public class DynamicObservablePropertyBinding<T> : IObservablePropertyBinding<T>
    {
        private PropertyGetDelegate<T> _getDelegate;
        private PropertySetDelegate<T> _setDelegate;

        private object _target;
        private string _propertyName;
        private string _propertyChangedEventName;

        public DynamicObservablePropertyBinding(object target, string propertyName, string propertyChangedEventName)
        {
            _target = target;
            _propertyName = propertyName;
            _propertyChangedEventName = propertyChangedEventName;
        }
        
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

        private static PropertyGetDelegate<T> CreatePropertyGetDelegate(object target, string propertyName)
        {
            PropertyInfo propertyInfo = target.GetType().GetProperty(propertyName);
            MethodInfo propertyGetter = propertyInfo.GetGetMethod();

            return (PropertyGetDelegate<T>)Delegate.CreateDelegate(typeof(PropertyGetDelegate<T>), target, propertyGetter.Name);
        }

        private static PropertySetDelegate<T> CreatePropertySetDelegate(object target, string propertyName)
        {
            PropertyInfo propertyInfo = target.GetType().GetProperty(propertyName);
            MethodInfo propertySetter = propertyInfo.GetSetMethod();

            return (PropertySetDelegate<T>)Delegate.CreateDelegate(typeof(PropertySetDelegate<T>), target, propertySetter.Name);
        }

        private static void AddEventHandler(EventHandler handler, object target, string eventName)
        {
            EventInfo eventInfo = target.GetType().GetEvent(eventName);
            eventInfo.AddEventHandler(target, handler);
        }

        private static void RemoveEventHandler(EventHandler handler, object target, string eventName)
        {
            EventInfo eventInfo = target.GetType().GetEvent(eventName);
            eventInfo.RemoveEventHandler(target, handler);
        }
    }
}
