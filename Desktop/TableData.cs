using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class TableData<TItem> : BindingList<TItem>, ITableData
    {
        public delegate object ColumnValueGetDelegate<TObject>(TObject obj);
        public delegate bool FindDelegate<TObject>(TObject obj);

        internal class PropertyDescriptorEx : PropertyDescriptor
        {
            private Type _columnType;
            private ColumnValueGetDelegate<TItem> _valueGetDelegate;

            internal PropertyDescriptorEx(string name, Type columnType, ColumnValueGetDelegate<TItem> valueGetDelegate)
                : base(name, new Attribute[] { })
            {
                _columnType = columnType;
                _valueGetDelegate = valueGetDelegate;
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override Type ComponentType
            {
                get { return typeof(TItem); }
            }

            public override object GetValue(object component)
            {
                return _valueGetDelegate((TItem)component);
            }

            public override bool IsReadOnly
            {
                get { return true; }
            }

            public override Type PropertyType
            {
                get { return _columnType; }
            }

            public override void ResetValue(object component)
            {
                throw new NotSupportedException();
            }

            public override void SetValue(object component, object value)
            {
                throw new NotSupportedException();
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }
        }

        private List<PropertyDescriptor> _properties;

        public TableData()
        {
            _properties = new List<PropertyDescriptor>();
        }

        public void AddColumn<TColumnType>(string columnName, ColumnValueGetDelegate<TItem> valueGetDelegate)
        {
            _properties.Add(new PropertyDescriptorEx(columnName, typeof(TColumnType), valueGetDelegate));
        }

        public int FindIndex(FindDelegate<TItem> findDelegate)
        {
            for(int i = 0; i < this.Count; i++)
            {
                if (findDelegate(this[i]))
                    return i;
            }
            return -1;
        }


        #region ITypedList Members

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return new PropertyDescriptorCollection(_properties.ToArray());
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return typeof(TItem).Name;
        }

        #endregion
     }
}
