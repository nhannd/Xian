using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Utility class used to wrap an untyped <see cref="IComparer"/> as a type-safe one.
    /// </summary>
    /// <typeparam name="T">The type of the items to be compared</typeparam>
    public class TypeSafeComparerWrapper<T> : Comparer<T>, IComparer
    {
        private IComparer _inner;
        public TypeSafeComparerWrapper(IComparer inner)
        {
            _inner = inner;
        }
        public override int Compare(T x, T y)
        {
            return _inner.Compare(x, y);
        }

        #region IComparer Members

        int IComparer.Compare(object x, object y)
        {
            return _inner.Compare(x, y);
        }

        #endregion
    }
}
