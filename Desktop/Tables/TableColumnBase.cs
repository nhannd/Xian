using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace ClearCanvas.Desktop.Tables
{
    public abstract class TableColumnBase<TItem> : ITableColumn
    {
        /// <summary>
        /// Comparer for sorting operations
        /// </summary>
        class SortComparer : IComparer
        {
            private int _direction;
            private Comparison<TItem> _comp;

            public SortComparer(Comparison<TItem> comparison, bool ascending)
            {
                _comp = comparison;
                _direction = ascending ? 1 : -1;
            }

            public int Compare(object x, object y)
            {
                return _comp((TItem)x, (TItem)y) * _direction;
            }
        }

        private string _name;
        private Type _columnType;
        private float _widthFactor;

        private Comparison<TItem> _comparison;

        public TableColumnBase(string columnName, Type columnType, float widthFactor, Comparison<TItem> comparison)
        {
            _name = columnName;
            _widthFactor = widthFactor;
            _columnType = columnType;
            _comparison = comparison;

            // if no comparison operator was specified, assign a default comparison
            if (_comparison == null)
            {
                // if TColumn implements IComparable, can compare by column value
                if (typeof(IComparable).IsAssignableFrom(_columnType))
                    _comparison = ValueComparsion;
                else
                    _comparison = NopComparison;    // no comparison is possible
            }
        }

        public Comparison<TItem> Comparison
        {
            get { return _comparison; }
            set { _comparison = value; }
        }

        #region ITableColumn members

        public string Name
        {
            get { return _name; }
        }

        public Type ColumnType
        {
            get { return _columnType; }
        }

        public float WidthFactor
        {
            get { return _widthFactor; }
        }

        public abstract bool ReadOnly { get; }

        public abstract object GetValue(object item);

        public abstract void SetValue(object item, object value);

        public IComparer GetComparer(bool ascending)
        {
            return new SortComparer(_comparison, ascending);
        }

        #endregion

        /// <summary>
        /// Default comparison used when TColumn is IComparable
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int ValueComparsion(TItem x, TItem y)
        {
            object valueX = GetValue(x);
            object valueY = GetValue(y);
            if (valueX == null)
            {
                if (valueY == null)
                    return 0;
                else
                    return -1;
            }
            else if (valueY == null)
            {
                return 1;
            }

            return ((IComparable)valueX).CompareTo(valueY);
        }

        /// <summary>
        /// Default comparison used when TColumn is not IComparable (in which case, sorting is not possible)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int NopComparison(TItem x, TItem y)
        {
            return 0;
        }
   }
}
