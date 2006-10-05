using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace ClearCanvas.Desktop.Tables
{
    /// <summary>
    /// Use this class to describe a column in conjunction with the <see cref="TableData"/> class.
    /// </summary>
    /// <typeparam name="TItem">The type of item on which the table is based</typeparam>
    /// <typeparam name="TColumn">The type of value that this column holds</typeparam>
    public class TableColumn<TItem, TColumn> : TableColumnBase<TItem>
    {
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


        private GetColumnValueDelegate<TItem, TColumn> _valueGetter;
        private SetColumnValueDelegate<TItem, TColumn> _valueSetter;


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
            :base(columnName, typeof(TColumn), widthFactor, comparison)
        {
            _valueGetter = valueGetter;
            _valueSetter = valueSetter;
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

        public override bool ReadOnly
        {
            get { return _valueSetter != null; }
        }

        public override object GetValue(object item)
        {
            return _valueGetter((TItem)item);
        }

        public override void SetValue(object item, object value)
        {
            _valueSetter((TItem)item, (TColumn)value);
        }
    }
}
