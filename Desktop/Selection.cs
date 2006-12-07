using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Default implementation of <see cref="ISelection"/>.
    /// </summary>
    public class Selection : ISelection, IEquatable<ISelection>
    {
        private List<object> _items = new List<object>();

        public Selection()
        {
        }

        public Selection(object item)
        {
            _items.Add(item);
        }

        public Selection(IEnumerable items)
        {
            foreach (object item in items)
                _items.Add(item);
        }

        #region ISelection Members

        public object[] Items
        {
            get { return _items.ToArray(); }
        }

        public object Item
        {
            get { return _items.Count > 0 ? _items[0] : null; }
        }

        public ISelection Union(ISelection other)
        {
            List<object> sum = new List<object>();

            // add all the items from the other selection
            sum.AddRange(other.Items);

            // add only the items from this selection not contained in the other selection
            sum.AddRange(_items.FindAll(delegate(object x) { return !other.Contains(x); }));

            return new Selection(sum);
        }

        public ISelection Intersect(ISelection other)
        {
            // return every item in this selection also contained in the other selection
            return new Selection(_items.FindAll(delegate(object x) { return other.Contains(x); }));
        }

        public ISelection Subtract(ISelection other)
        {
            // return every item in this selection not contained in the other selection
            return new Selection(_items.FindAll(delegate(object x) { return !other.Contains(x); }));
        }

        public bool Contains(object item)
        {
            return _items.Contains(item);
        }

        #endregion

        #region IEquatable<ISelection> Members

        public bool Equals(ISelection other)
        {
            if (other == null)
                return false;

            // true if every item in this selection is contained in the other selection
            return _items.TrueForAll(delegate(object x) { return other.Contains(x); });
        }

        #endregion

        public override bool Equals(object obj)
        {
            ISelection that = obj as ISelection;
            return this.Equals(that);
        }

        public override int GetHashCode()
        {
            int n = 0;
            foreach (object item in _items)
            {
                n ^= item.GetHashCode();
            }
            return n;
        }
    }
}
