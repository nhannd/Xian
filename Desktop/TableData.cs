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
        /// <summary>
        /// Extends <see cref="PropertyDescriptor"/> in order to give us control
        /// over implementation of abstract methods.  This implementation
        /// simply wraps an existing property descriptor and delegates all methods
        /// to the inner instance, however it allows us to control the Name property
        /// which is otherwise not available.
        /// </summary>
        internal class PropertyDescriptorEx : PropertyDescriptor
        {
            private static Attribute[] ConvertAttrCollectionToArray(AttributeCollection col)
            {
                Attribute[] attrs = new Attribute[col.Count];
                int i = 0;
                foreach (Attribute a in col)
                    attrs[i++] = a;
                return attrs;
            }

            private PropertyDescriptor _p;

            internal PropertyDescriptorEx(string name, PropertyDescriptor p)
                : base(name, ConvertAttrCollectionToArray(p.Attributes))
            {
                _p = p;
            }

            public override bool CanResetValue(object component)
            {
                return _p.CanResetValue(component);
            }

            public override Type ComponentType
            {
                get { return _p.ComponentType; }
            }

            public override object GetValue(object component)
            {
                return _p.GetValue(component);
            }

            public override bool IsReadOnly
            {
                get { return _p.IsReadOnly; }
            }

            public override Type PropertyType
            {
                get { return _p.PropertyType; }
            }

            public override void ResetValue(object component)
            {
                _p.ResetValue(component);
            }

            public override void SetValue(object component, object value)
            {
                _p.SetValue(component, value);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return _p.ShouldSerializeValue(component);
            }
        }




        #region ITypedList Members

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(TItem));

            List<PropertyDescriptor> wrappedProperties = new List<PropertyDescriptor>();
            foreach (PropertyDescriptor property in properties)
            {
                TableColumnAttribute a = GetTableColumnAttribute(property);
                if (a != null)
                {
                    wrappedProperties.Add(new PropertyDescriptorEx(a.ColumnName, property));
                }
            }
            return new PropertyDescriptorCollection(wrappedProperties.ToArray());
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return typeof(TItem).Name;
        }

        #endregion

        private TableColumnAttribute GetTableColumnAttribute(PropertyDescriptor property)
        {
            foreach (Attribute a in property.Attributes)
            {
                if (a.GetType() == typeof(TableColumnAttribute))
                    return (TableColumnAttribute)a;
            }
            return null;
        }
    }
}
