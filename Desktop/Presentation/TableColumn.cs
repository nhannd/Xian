using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Presentation
{
    public delegate object ColumnValueDelegate<TItem>(TItem item);

    public class TableColumn<TItem> : ITableColumn
    {
        private string _header;
        private ColumnValueDelegate<TItem> _valueCallback;
        private float _width;

        public TableColumn(string header, ColumnValueDelegate<TItem> valueCallback, float width)
        {
            _header = header;
            _valueCallback = valueCallback;
            _width = width;
        }

        public TableColumn(string header, ColumnValueDelegate<TItem> valueCallback)
            : this(header, valueCallback, 1)
        {
        }

        public string Header
        {
            get { return _header; }
        }

        public float Width
        {
            get { return _width; }
        }

        public object GetValue(TItem item)
        {
            return _valueCallback(item);
        }
    }
}
