using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public class TableColumn<TItem, TColumn> : ITableColumn<TItem>
    {
        public delegate TValue GetColumnValueDelegate<TObject, TValue>(TObject obj);
        public delegate void SetColumnValueDelegate<TObject, TValue>(TObject obj, TValue val);


        private string _name;
        private float _widthFactor;
        private GetColumnValueDelegate<TItem, TColumn> _valueGetter;
        private SetColumnValueDelegate<TItem, TColumn> _valueSetter;


        public TableColumn(
            string columnName,
            GetColumnValueDelegate<TItem, TColumn> valueGetter,
            SetColumnValueDelegate<TItem, TColumn> valueSetter,
            float widthFactor)
        {
            _name = columnName;
            _widthFactor = widthFactor;
            _valueGetter = valueGetter;
            _valueSetter = valueSetter;
        }

        public TableColumn(string columnName, GetColumnValueDelegate<TItem, TColumn> valueGetter)
            :this(columnName, valueGetter, null, 1.0f)
        {
        }

        public TableColumn(string columnName, GetColumnValueDelegate<TItem, TColumn> valueGetter, float widthFactor)
            : this(columnName, valueGetter, null, widthFactor)
        {
        }


        public TableColumn(string columnName, GetColumnValueDelegate<TItem, TColumn> valueGetter, SetColumnValueDelegate<TItem, TColumn> valueSetter)
            : this(columnName, valueGetter, valueSetter, 1.0f)
        {
        }


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
    }
}
