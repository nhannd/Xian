#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
    internal class TableDataPropertyDescriptor : PropertyDescriptor
    {
        private TableAdapter _owner;
        private ITableColumn _column;

        internal TableDataPropertyDescriptor(TableAdapter owner, ITableColumn column)
            : base(column.Name, new Attribute[] { })
        {
            _owner = owner;
            _column = column;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return _owner.Table.ItemType; }
        }

        public override object GetValue(object component)
        {
            return _column.GetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            _column.SetValue(component, value);
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

    public class TableAdapter : IBindingList, ITypedList
    {
        private ITable _table;
        private event ListChangedEventHandler _listChanged;
        private Dictionary<ITableColumn, TableDataPropertyDescriptor> _columnToPropertyMap;

        public TableAdapter(ITable table)
        {
            _table = table;
            _table.Items.ItemsChanged += TableDataChangedEventHandler;
            _table.Columns.ItemsChanged += TableStructureChangedEventHandler;

            // init map of columns to property descriptors
            _columnToPropertyMap = new Dictionary<ITableColumn, TableDataPropertyDescriptor>();
            foreach (ITableColumn column in _table.Columns)
            {
                _columnToPropertyMap[column] = new TableDataPropertyDescriptor(this, column);
            }
        }

        public ITable Table
        {
            get { return _table; }
        }

        #region IBindingList Members that are implemented

        bool IBindingList.AllowEdit
        {
            get { return true; }
        }

        bool IBindingList.AllowNew
        {
            get { return false; }
        }

        bool IBindingList.AllowRemove
        {
            get { return false; }
        }

        void IBindingList.ApplySort(PropertyDescriptor prop, ListSortDirection direction)
        {
            ITableColumn column = FindColumnForPropertyDesc(prop);
            if (column != null)
            {
                _table.Sort(new TableSortParams(column, direction == ListSortDirection.Ascending));
            }
        }

        bool IBindingList.IsSorted
        {
            get { return _table.SortParams != null; }
        }

        event ListChangedEventHandler IBindingList.ListChanged
        {
            add { _listChanged += value; }
            remove { _listChanged -= value; }
        }

        ListSortDirection IBindingList.SortDirection
        {
            get { return _table.SortParams.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending; }
        }


        bool IBindingList.SupportsChangeNotification
        {
            get { return true; }
        }

        bool IBindingList.SupportsSearching
        {
            get { return false; }
        }

        bool IBindingList.SupportsSorting
        {
            get { return true; }
        }

        PropertyDescriptor IBindingList.SortProperty
        {
            get
            {
                return _table.SortParams == null ? null : _columnToPropertyMap[_table.SortParams.Column];
            }
        }

        #endregion

        #region IBindingList members that are unimplemented

        void IBindingList.AddIndex(PropertyDescriptor property)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        object IBindingList.AddNew()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int IBindingList.Find(PropertyDescriptor property, object key)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IBindingList.RemoveIndex(PropertyDescriptor property)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IBindingList.RemoveSort()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IList Members - mostly not implemented

        int System.Collections.IList.Add(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void System.Collections.IList.Clear()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool System.Collections.IList.Contains(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int System.Collections.IList.IndexOf(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void System.Collections.IList.Insert(int index, object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool System.Collections.IList.IsFixedSize
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        bool System.Collections.IList.IsReadOnly
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        void System.Collections.IList.Remove(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void System.Collections.IList.RemoveAt(int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        object System.Collections.IList.this[int index]
        {
            get
            {
                return _table.Items[index];
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region ICollection Members - some not implemented

        void System.Collections.ICollection.CopyTo(Array array, int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int System.Collections.ICollection.Count
        {
            get { return _table.Items.Count; }
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get { return false; }
        }

        object System.Collections.ICollection.SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _table.Items.GetEnumerator();
        }

        #endregion

        #region ITypedList Members

        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
            foreach (ITableColumn column in _table.Columns)
            {
                properties.Add(_columnToPropertyMap[column]);
            }

            return new PropertyDescriptorCollection(properties.ToArray());
        }

        string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
        {
            return _table.ItemType.Name;
        }

        #endregion

        #region Private Methods

        private void TableDataChangedEventHandler(object sender, ItemChangedEventArgs e)
        {
            switch (e.ChangeType)
            {
                case ItemChangeType.ItemAdded:
                    NotifyListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, e.ItemIndex));
                    break;
                case ItemChangeType.ItemChanged:
                    NotifyListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, e.ItemIndex));
                    break;
                case ItemChangeType.ItemRemoved:
                    NotifyListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, e.ItemIndex));
                    break;
				case ItemChangeType.ItemInserted:
                case ItemChangeType.Reset:
                    NotifyListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void TableStructureChangedEventHandler(object sender, ItemChangedEventArgs e)
        {
            ITableColumn column = e.ItemIndex >= 0 ? (ITableColumn)_table.Columns[e.ItemIndex] : null;
            switch (e.ChangeType)
            {
                case ItemChangeType.ItemAdded:
                    _columnToPropertyMap[column] = new TableDataPropertyDescriptor(this, column);
                    NotifyListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, -1));
                    break;
                case ItemChangeType.ItemChanged:
                    _columnToPropertyMap[column] = new TableDataPropertyDescriptor(this, column);
                    NotifyListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, -1));
                    break;
                case ItemChangeType.ItemRemoved:
                    _columnToPropertyMap.Remove(column);
                    NotifyListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, -1));
                    break;
				case ItemChangeType.ItemInserted:
				case ItemChangeType.Reset:
					_columnToPropertyMap.Clear();
					NotifyListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            		break;
            }
        }

        private void NotifyListChanged(ListChangedEventArgs args)
        {
            EventsHelper.Fire(_listChanged, this, args);
        }

        private ITableColumn FindColumnForPropertyDesc(PropertyDescriptor prop)
        {
            foreach (ITableColumn column in _table.Columns)
            {
                if (column.Name == prop.Name)
                    return column;
            }
            return null;
        }
        
        #endregion
    }
}
