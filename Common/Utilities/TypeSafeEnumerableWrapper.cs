using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Utility class used to wrap an untyped <see cref="IEnumerable"/> as a type-safe one.
    /// </summary>
    /// <typeparam name="T">The type of the items to be enumerated</typeparam>
    public class TypeSafeEnumerableWrapper<T> : IEnumerable<T>, IEnumerable
    {
        private IEnumerable _inner;

        public TypeSafeEnumerableWrapper(IEnumerable inner)
        {
            _inner = inner;
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return new TypeSafeEnumeratorWrapper<T>(_inner.GetEnumerator());
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        #endregion
    }
}
