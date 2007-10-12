#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class ListItemSelector : UserControl
    {
        #region Private Members

        private readonly string _allColumns = "(All)";

        private ITable _availableItemsTable = null;
        private ITable _selectedItemsTable = null;

        private event EventHandler _itemAdded;
        private event EventHandler _itemRemoved;

        #endregion

        #region Constructor

        public ListItemSelector()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Properties

        public ITable AvailableItemsTable
        {
            get { return _availableItemsTable; }
            set
            {
                _availableItemsTable = value;
                _availableItems.Table = _availableItemsTable;

                if(_availableItemsTable != null)
                {
                    UpdateFilterChoices();
                    _availableItemsTable.Sort();
                } 
            }
        }

        public ITable SelectedItemsTable
        {
            get { return _selectedItemsTable; }
            set
            {
                _selectedItemsTable = value;
                _selectedItems.Table = _selectedItemsTable;

                if(_selectedItemsTable != null) _selectedItemsTable.Sort();
            }
        }

        public void OnAvailableItemsChanged(object sender, EventArgs args)
        {
            if(_availableItemsTable.IsFiltered) _availableItemsTable.Filter();
            _availableItems.Table = _availableItemsTable;
        }

        #endregion

        #region Public Events

        public event EventHandler ItemAdded
        {
            add { _itemAdded += value; }
            remove { _itemAdded -= value; }
        }

        public event EventHandler ItemRemoved
        {
            add { _itemRemoved += value; }
            remove { _itemRemoved -= value; }
        }

        #endregion

        #region Private Methods

        private void UpdateFilterChoices()
        {
            IList<string> columnNames = new List<string>();
            columnNames.Add(_allColumns);

            foreach (ITableColumn column in _availableItemsTable.Columns)
            {
                columnNames.Add(column.Name);
            }

            _filterColumn.DataSource = columnNames;
        }

        private void _applyFilterButton_Click(object sender, EventArgs e)
        {
            ITableColumn theColumn = null;
            foreach (ITableColumn column in _availableItemsTable.Columns)
            {
                if(string.Compare(column.Name, (string)_filterColumn.Value) == 0)
                {
                    theColumn = column;
                    break;
                }
            }

            TableFilterParams filterParams = new TableFilterParams(theColumn, _filterValue.Value);
            _selectedItemsTable.Filter(filterParams);
            _availableItemsTable.Filter(filterParams);

            RefreshTables();
        }

        private void _clearFilterButton_Click(object sender, EventArgs e)
        {
            _filterColumn.Value = _allColumns;
            _filterValue.Value = string.Empty;

            _selectedItemsTable.RemoveFilter();
            _availableItemsTable.RemoveFilter();

            RefreshTables();
        }

        private void AddSelection(object sender, EventArgs e)
        {
            ISelection selection = _availableItems.Selection;
            foreach (object item in selection.Items)
            {
                _selectedItems.Table.Items.Add(item);
                _availableItems.Table.Items.Remove(item);
            }
            _selectedItemsTable.Sort();

            RefreshTables();

            EventsHelper.Fire(_itemAdded, this, EventArgs.Empty);
        }

        private void RemoveSelection(object sender, EventArgs e)
        {
            ISelection selection = _selectedItems.Selection;
            foreach (object item in selection.Items)
            {
                _selectedItemsTable.Items.Remove(item);
                _availableItemsTable.Items.Add(item);
            }
            _availableItemsTable.Sort();

            RefreshTables();

            EventsHelper.Fire(_itemRemoved, this, EventArgs.Empty);
        }

        private void RefreshTables()
        {
            _selectedItems.Table = _selectedItemsTable;
            _availableItems.Table = _availableItemsTable;
        }

        #endregion
    }
}