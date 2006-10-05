using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Utility class used to wrap an untyped <see cref="IEnumerator"/> as a type-safe one.
    /// This is an interim measure because NHibernate does not yet support generics.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumeratorWrapper<T> : IEnumerator<T>
    {
        private IEnumerator _inner;

        public EnumeratorWrapper(IEnumerator inner)
        {
            _inner = inner;
        }

        #region IEnumerator<T> Members

        public T Current
        {
            get { return (T)_inner.Current; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // nothing to do?
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { return _inner.Current; }
        }

        public bool MoveNext()
        {
            return _inner.MoveNext();
        }

        public void Reset()
        {
            _inner.Reset();
        }

        #endregion
    }
}
