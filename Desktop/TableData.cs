using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

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
            private TableData<TItem> _table;
            private ITableColumn<TItem> _column;

            internal PropertyDescriptorEx(TableData<TItem> table, ITableColumn<TItem> column)
                : base(column.Name, new Attribute[] { })
            {
                _table = table;
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

            public int WidthPercent
            {
                get { return _table.GetColumnWidthPercent(_column); }
            }
        }

        class ColumnList : ObservableList<ITableColumn<TItem>, CollectionEventArgs<ITableColumn<TItem>>>
        {
        }


        private ColumnList _columns;
        private Dictionary<ITableColumn<TItem>, PropertyDescriptor> _propertyDescriptors;

        private ListSortDirection _sortDirection;
        private bool _isSorted;
        private PropertyDescriptor _sortProperty;

        /// <summary>
        /// Constructor
        /// </summary>
        public TableData()
        {
            _propertyDescriptors = new Dictionary<ITableColumn<TItem>, PropertyDescriptor>();

            _columns = new ColumnList();
            _columns.ItemAdded += delegate(object sender, CollectionEventArgs<ITableColumn<TItem>> args)
                {
                    _propertyDescriptors.Add(args.Item, new PropertyDescriptorEx(this, args.Item));
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

        /// <summary>
        /// Adds all items in the specified enumerable
        /// </summary>
        /// <param name="enumerable"></param>
        public void AddRange(IEnumerable<TItem> enumerable)
        {
            foreach (TItem item in enumerable)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Adds all items in the specified enumerable
        /// </summary>
        /// <param name="enumerable"></param>
        public void AddRange(IEnumerable enumerable)
        {
            foreach (TItem item in enumerable)
            {
                this.Add(item);
            }
        }

		public void ApplySort()
		{
			if (_sortProperty == null)
				return;

			ApplySortCore(_sortProperty, _sortDirection);
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

        #region BindingList<T> overrides

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            ITableColumn<TItem> column = FindColumnForPropertyDesc(prop);
            if (column != null)
            {
                _sortDirection = direction;
                _sortProperty = prop;

                List<TItem> listRef = (List<TItem>)this.Items;
                listRef.Sort(column.GetComparer(_sortDirection == ListSortDirection.Ascending));
                _isSorted = true;

                // notify that the list has been sorted
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

        protected override void RemoveSortCore()
        {
            _isSorted = false;
            _sortProperty = null;
        }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override bool IsSortedCore
        {
            get { return _isSorted; }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get { return _sortDirection; }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get { return _sortProperty; }
        }

        #endregion


        private ITableColumn<TItem> FindColumnForPropertyDesc(PropertyDescriptor prop)
        {
            foreach (KeyValuePair<ITableColumn<TItem>, PropertyDescriptor> kvp in _propertyDescriptors)
            {
                if (kvp.Value == prop)
                    return kvp.Key;
            }
            return null;
        }

        private int GetColumnWidthPercent(ITableColumn<TItem> column)
        {
            float sum = 0;
            foreach (ITableColumn<TItem> c in _columns)
            {
                sum += c.WidthFactor;
            }
            return (int)(100*column.WidthFactor / sum);
        }
     }
#endif
}
