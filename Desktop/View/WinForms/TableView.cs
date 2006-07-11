using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Presentation;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class TableView : UserControl
	{
        class Selection : ISelection
        {
            private ITableRow[] _rows;

            internal Selection(ITableRow[] rows)
            {
                _rows = rows;
            }

            #region ISelection Members

            public ITableRow[] SelectedRows
            {
                get { return _rows; }
            }

            public ITableRow SelectedRow
            {
                get { return _rows.Length > 0 ? _rows[0] : null; }
            }

            #endregion
        }

        private ITableData _tableData;
        private event EventHandler<TableViewEventArgs> _itemDoubleClicked;
        private event EventHandler<TableViewEventArgs> _selectionChanged;



		public TableView()
		{
			InitializeComponent();
		}

        public void SetData(ITableData tableData)
        {
            _tableData = tableData;

            RefreshColumns();
            RefreshRows();
        }

        public void RefreshColumns()
        {
            _dataGridView.Columns.Clear();

            foreach(ITableColumn tc in _tableData.Columns)
            {
			    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
			    column.HeaderText = tc.Header;
			    column.Width = (int)(tc.Width * 100);
                _dataGridView.Columns.Add(column);
            }
        }

        public void RefreshRows()
        {
            _dataGridView.SuspendLayout();
            _dataGridView.Rows.Clear();

            foreach(ITableRow tr in _tableData.Rows)
            {
                object[] rowValues = new object[_tableData.Columns.Length];
                for(int c = 0; c < _tableData.Columns.Length; c++)
                {
                    rowValues[c] = tr.GetValue(c);
                }

                int i = _dataGridView.Rows.Add(rowValues);
                _dataGridView.Rows[i].Tag = tr;
            }
            _dataGridView.ResumeLayout(true);

        }

        /// <summary>
        /// Returns the current selection
        /// </summary>
        public ISelection CurrentSelection
        {
            get
            {
                List<ITableRow> rows = new List<ITableRow>();
                foreach (DataGridViewRow dr in _dataGridView.SelectedRows)
                {
                    rows.Add((ITableRow)dr.Tag);
                }
                return new Selection(rows.ToArray());
            }
        }

        public event EventHandler<TableViewEventArgs> SelectionChanged
        {
            add { _selectionChanged += value; }
            remove { _selectionChanged -= value; }
        }

        public event EventHandler<TableViewEventArgs> ItemDoubleClicked
        {
            add { _itemDoubleClicked += value; }
            remove { _itemDoubleClicked -= value; }
        }

        private void _dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)    // rowindex == -1 represents a header click
            {
                EventsHelper.Fire(_itemDoubleClicked, this, new TableViewEventArgs());
            }
        }

        private void _dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            EventsHelper.Fire(_selectionChanged, this, new TableViewEventArgs());
        }
	}
}
