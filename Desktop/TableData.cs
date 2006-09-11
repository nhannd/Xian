using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{

#if !MONO
    /// <summary>
    /// Useful generic implementation of <see cref="ITableData"/>
    /// </summary>
    /// <typeparam name="TItem">The type of item that the table will display</typeparam>
    public class TableData<TItem> : BindingList<TItem>, ITableData
    {
        internal class PropertyDescriptorEx : PropertyDescriptor
        {
            private ITableColumn<TItem> _column;

            internal PropertyDescriptorEx(ITableColumn<TItem> column)
                : base(column.Name, new Attribute[] { })
            {
                _column = column;
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
                return _column.GetValue((TItem)component);
            }

            public override void SetValue(object component, object value)
            {
                _column.SetValue((TItem)component, value);
            }

            public override bool IsReadOnly
            {
                get { return _column.ReadOnly; }
            }

            public override Type PropertyType
            {
                get { return _column.ColumnType; }
            }

            public override void ResetValue(object component)
            {
                throw new NotSupportedException();
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }
        }

        class ColumnList : ObservableList<ITableColumn<TItem>, CollectionEventArgs<ITableColumn<TItem>>>
        {
        }


        private ColumnList _columns;
        private Dictionary<ITableColumn<TItem>, PropertyDescriptor> _propertyDescriptors;

        /// <summary>
        /// Constructor
        /// </summary>
        public TableData()
        {
            _propertyDescriptors = new Dictionary<ITableColumn<TItem>, PropertyDescriptor>();

            _columns = new ColumnList();
            _columns.ItemAdded += delegate(object sender, CollectionEventArgs<ITableColumn<TItem>> args)
                {
                    _propertyDescriptors.Add(args.Item, new PropertyDescriptorEx(args.Item));
                };
            _columns.ItemRemoved += delegate(object sender, CollectionEventArgs<ITableColumn<TItem>> args)
                {
                    _propertyDescriptors.Remove(args.Item);
                };
        }

        /// <summary>
        /// Accesses the list of columns that describe this table data.  Use this property to add
        /// <see cref="TableColumn"/> objects.
        /// </summary>
        public IList<ITableColumn<TItem>> Columns
        {
            get { return _columns; }
        }

        /// <summary>
        /// Searches the rows for an item that meets the criteria of the specified delegate and returns
        /// the index of the first such item.
        /// </summary>
        /// <param name="findDelegate">A delegate that accepts an item and returns a boolean to indicate if the item is the item sought</param>
        /// <returns>The index of the first matching item, or -1 if no matching items are found</returns>
        public int FindIndex(Predicate<TItem> findDelegate)
        {
            for(int i = 0; i < this.Count; i++)
            {
                if (findDelegate(this[i]))
                    return i;
            }
            return -1;
        }

        public void AddRange(IEnumerable<TItem> enumerable)
        {
            foreach (TItem item in enumerable)
            {
                this.Add(item);
            }
        }

        public void AddRange(IEnumerable enumerable)
        {
            foreach (TItem item in enumerable)
            {
                this.Add(item);
            }
        }



        #region ITypedList Members

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
            foreach (ITableColumn<TItem> column in _columns)
            {
                properties.Add(_propertyDescriptors[column]);
            }

            return new PropertyDescriptorCollection(properties.ToArray());
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return typeof(TItem).Name;
        }

        #endregion
     }
#endif
}
