using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace ClearCanvas.Common.Utilities
{
    public class EnumerableWrapper<T> : IEnumerable<T>
    {
        private IEnumerable _inner;

        public EnumerableWrapper(IEnumerable inner)
        {
            _inner = inner;
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return new EnumeratorWrapper<T>(_inner.GetEnumerator());
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
