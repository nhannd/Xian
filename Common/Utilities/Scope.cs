using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Taken and modified from the following MSDN article by Stephen Toub:
    /// http://msdn.microsoft.com/msdnmag/issues/06/09/NETMatters/default.aspx
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Scope<T> : IDisposable
        where T : class
    {
        private bool _disposed, _ownsInstance;
        private T _instance;
        private Scope<T> _parent;

        [ThreadStatic]
        private static Scope<T> _head;

        public Scope(T instance) : this(instance, true) { }

        public Scope(T instance, bool ownsInstance)
        {
            Platform.CheckForNullReference(instance, "instance");

            _instance = instance;
            _ownsInstance = ownsInstance;

            _parent = _head;
            _head = this;
        }

        public static T Current
        {
            get { return _head != null ? _head._instance : null; }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                System.Diagnostics.Debug.Assert(this == _head, "Disposed out of order.");
                _head = _parent;

                if (_ownsInstance)
                {
                    IDisposable disposable = _instance as IDisposable;
                    if (disposable != null) disposable.Dispose();
                }
            }
        }
    }
}
