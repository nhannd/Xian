using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class TableColumnAttribute : Attribute
    {
        private string _columnName;

        public TableColumnAttribute(string columnName)
        {
            _columnName = columnName;
        }

        public string ColumnName
        {
            get { return _columnName; }
        }
    }
}
