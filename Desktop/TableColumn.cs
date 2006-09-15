using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Use this class to describe a column in conjunction with the <see cref="TableData"/> class.
    /// </summary>
    /// <typeparam name="TItem">The type of item on which the table is based</typeparam>
    /// <typeparam name="TColumn">The type of value that this column holds</typeparam>
    public class TableColumn<TItem, TColumn> : ITableColumn<TItem>
    {
        /// <summary>
        /// Comparer for sorting operations
        /// </summary>
        class SortComparer : Comparer<TItem>
        {
            private int _direction;
            private Comparison<TItem> _comp;

            public SortComparer(TableColumn<TItem, TColumn> column, bool ascending)
            {
                _comp = column.Comparison;
                _direction = ascending ? 1 : -1;
            }

            public override int Compare(TItem x, TItem y)
            {
                return _comp(x, y) * _direction;
            }
        }


        /// <summary>
        /// Delegate that is used to pull the value of a column from an object.
        /// </summary>
        /// <typeparam name="TObject">The type of the object</typeparam>
        /// <typeparam name="TValue">The expected type of the value to pull</typeparam>
        /// <param name="obj">The object from which to pull the value</param>
        /// <returns>The value</returns>
        public delegate TValue GetColumnValueDelegate<TObject, TValue>(TObject obj);

        /// <summary>
        /// Delegate that is used to push the value of a column to an object.
        /// </summary>
        /// <typeparam name="TObject">The type of the object</typeparam>
        /// <typeparam name="TValue">The type of the value to push</typeparam>
        /// <param name="obj">The object to which the value is pushed</param>
        /// <param name="val">The value</param>
        public delegate void SetColumnValueDelegate<TObject, TValue>(TObject obj, TValue val);


        private string _name;
        private float _widthFactor;
        private GetColumnValueDelegate<TItem, TColumn> _valueGetter;
        private SetColumnValueDelegate<TItem, TColumn> _valueSetter;

        private Comparison<TItem> _comparison;


        /// <summary>
        /// Constructs a table column
        /// </summary>
        /// <param name="columnName">The name of the column</param>
        /// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item</param>
        /// <param name="valueSetter">A delegate that accepts an item and a value, and pushes the value to the item.  May be null if the column is read-only.</param>
        /// <param name="widthFactor">A weighting factor that is applied to the width of the column</param>
        /// <param name="comparison">A custom comparison operator that is used for sorting based on this column</param>
        public TableColumn(
            string columnName,
            GetColumnValueDelegate<TItem, TColumn> valueGetter,
            SetColumnValueDelegate<TItem, TColumn> valueSetter,
            float widthFactor,
            Comparison<TItem> comparison)
        {
            _name = columnName;
            _widthFactor = widthFactor;
            _valueGetter = valueGetter;
            _valueSetter = valueSetter;
            _comparison = comparison;

            // if no comparison operator was specified, assign a default comparison
            if (_comparison == null)
            {
                // if TColumn implements IComparable, can compare by column value
                if (typeof(IComparable).IsAssignableFrom(typeof(TColumn)))
                    _comparison = ValueComparsion;
                else
                    _comparison = NopComparison;    // no comparison is possible
            }
        }

        /// <summary>
        /// Constructs a table column
        /// </summary>
        /// <param name="columnName">The name of the column</param>
        /// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item</param>
        /// <param name="valueSetter">A delegate that accepts an item and a value, and pushes the value to the item.  May be null if the column is read-only.</param>
        /// <param name="widthFactor">A weighting factor that is applied to the width of the column</param>
        public TableColumn(
            string columnName,
            GetColumnValueDelegate<TItem, TColumn> valueGetter,
            SetColumnValueDelegate<TItem, TColumn> valueSetter,
            float widthFactor)
            :this(columnName, valueGetter, valueSetter, widthFactor, null)
        {
        }

        /// <summary>
        /// Constructs a read-only table column
        /// </summary>
        /// <param name="columnName">The name of the column</param>
        /// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item</param>
        public TableColumn(string columnName, GetColumnValueDelegate<TItem, TColumn> valueGetter)
            :this(columnName, valueGetter, null, 1.0f)
        {
        }

        /// <summary>
        /// Constructs a read-only table column
        /// </summary>
        /// <param name="columnName">The name of the column</param>
        /// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item</param>
        /// <param name="widthFactor">A weighting factor that is applied to the width of the column</param>
        public TableColumn(string columnName, GetColumnValueDelegate<TItem, TColumn> valueGetter, float widthFactor)
            : this(columnName, valueGetter, null, widthFactor)
        {
        }


        /// <summary>
        /// Constructs a table column
        /// </summary>
        /// <param name="columnName">The name of the column</param>
        /// <param name="valueGetter">A delegate that accepts an item and pulls the column value from the item</param>
        /// <param name="valueSetter">A delegate that accepts an item and a value, and pushes the value to the item.  May be null if the column is read-only.</param>
        public TableColumn(string columnName, GetColumnValueDelegate<TItem, TColumn> valueGetter, SetColumnValueDelegate<TItem, TColumn> valueSetter)
            : this(columnName, valueGetter, valueSetter, 1.0f)
        {
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
            get { return typeof(TColumn); }
        }

        public float WidthFactor
        {
            get { return _widthFactor; }
        }

        public bool ReadOnly
        {
            get { return _valueSetter == null; }
        }

        public object GetValue(TItem item)
        {
            return _valueGetter(item);
        }

        public void SetValue(TItem item, object value)
        {
            _valueSetter(item, (TColumn)value);
        }

        public IComparer<TItem> GetComparer(bool ascending)
        {
            return new SortComparer(this, ascending);
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
            return ((IComparable)GetValue(x)).CompareTo(GetValue(y));
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
