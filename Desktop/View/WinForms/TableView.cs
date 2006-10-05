using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class TableView : UserControl
	{
        class Selection : ISelection
        {
            private List<object> _items;

            internal Selection(DataGridViewSelectedRowCollection rows)
            {
                _items = new List<object>();
                foreach (DataGridViewRow row in rows)
                    _items.Add(row.DataBoundItem);
            }

            #region ISelection Members

            public object[] Items
            {
                get { return _items.ToArray(); }
            }

            public object Item
            {
                get { return _items.Count > 0 ? _items[0] : null; }
            }

            #endregion
        }

        private event EventHandler _itemDoubleClicked;
        private ActionModelNode _toolbarModel;
        private ActionModelNode _menuModel;
		private ToolStripItemDisplayStyle _toolStripItemDisplayStyle = ToolStripItemDisplayStyle.Image;
        private ITable _table;

		public TableView()
		{
			InitializeComponent();

            // if we allow the framework to generate columns, there seems to be a bug with 
            // setting the minimum column width > 100 pixels
            // therefore, turn off the auto-generate and create the columns ourselves
            _dataGridView.AutoGenerateColumns = false;
		}

        public bool ReadOnly
        {
            get { return _dataGridView.ReadOnly; }
            set { _dataGridView.ReadOnly = value; }
        }

        public ActionModelNode ToolbarModel
        {
            get { return _toolbarModel;  }
            set
            {
                _toolbarModel = value;
                ToolStripBuilder.Clear(_toolStrip.Items);
                if (_toolbarModel != null)
                {
                    if (_toolStrip.RightToLeft == RightToLeft.Yes)
                    {
                        ActionModelNode[] childNodesCopy = new ActionModelNode[_toolbarModel.ChildNodes.Count];
                        for (int i = 0; i < _toolbarModel.ChildNodes.Count; i++)
                        {
                            childNodesCopy[childNodesCopy.Length - i - 1] = _toolbarModel.ChildNodes[i];
                        }
                        ToolStripBuilder.BuildToolbar(_toolStrip.Items, childNodesCopy);
                    }
                    else
                    {
                        ToolStripBuilder.BuildToolbar(_toolStrip.Items, _toolbarModel.ChildNodes);
                    }

					foreach (ToolStripItem item in _toolStrip.Items)
						item.DisplayStyle = _toolStripItemDisplayStyle;
                }
            }
        }

        public ActionModelNode MenuModel
        {
            get { return _menuModel; }
            set
            {
                _menuModel = value;
                ToolStripBuilder.Clear(_contextMenu.Items);
                if (_menuModel != null)
                {
                    ToolStripBuilder.BuildMenu(_contextMenu.Items, _menuModel.ChildNodes);
                }
            }
        }

        public ITable Table
        {
            get { return _table; }
            set
            {
                _table = value;

                InitColumns();

                if (_table != null)
                {
                    _bindingSource.DataSource = new TableAdapter(_table);
                    _dataGridView.DataSource = _bindingSource;
                }
            }
        }

        public RightToLeft ToolStripRightToLeft
        {
            get { return _toolStrip.RightToLeft; }
            set { _toolStrip.RightToLeft = value; }
        }

		public ToolStripItemDisplayStyle ToolStripItemDisplayStyle
		{
			get { return _toolStripItemDisplayStyle; }
			set { _toolStripItemDisplayStyle = value; }
		}

        /// <summary>
        /// Returns the current selection
        /// </summary>
        public ISelection CurrentSelection
        {
            get { return new Selection(_dataGridView.SelectedRows); }
        }

        public event EventHandler SelectionChanged
        {
            add { _dataGridView.SelectionChanged += value; }
            remove { _dataGridView.SelectionChanged -= value; }
        }

        public event EventHandler ItemDoubleClicked
        {
            add { _itemDoubleClicked += value; }
            remove { _itemDoubleClicked -= value; }
        }

        private void InitColumns()
        {
            // clear the old columns
            _dataGridView.Columns.Clear();

            if (_table != null)
            {
                foreach (ITableColumn col in _table.Columns)
                {
                    // this is ugly but somebody's gotta do it
                    DataGridViewColumn dgcol;
                    if (col.ColumnType == typeof(bool))
                        dgcol = new DataGridViewCheckBoxColumn();
                    else if (col.ColumnType == typeof(Image))
                        dgcol = new DataGridViewImageColumn();
                    else
                    {
                        // assume any other type of column will be displayed as text
                        dgcol = new DataGridViewTextBoxColumn();
                    }

                    // initialize the necessary properties
                    dgcol.Name = col.Name;
                    dgcol.HeaderText = col.Name;
                    dgcol.DataPropertyName = col.Name;
                    dgcol.MinimumWidth = (int)(col.WidthFactor * _table.BaseColumnWidth);
                    dgcol.FillWeight = col.WidthFactor;
                    _dataGridView.Columns.Add(dgcol);
                }
            }
        }

        private void _dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)    // rowindex == -1 represents a header click
            {
                EventsHelper.Fire(_itemDoubleClicked, this, new EventArgs());
            }
        }

        private void _contextMenu_Opening(object sender, CancelEventArgs e)
        {
			// Find the row we're on
			Point pt = _dataGridView.PointToClient(DataGridView.MousePosition);
			DataGridView.HitTestInfo info = _dataGridView.HitTest(pt.X, pt.Y);

			// If only one row is selected, assume that the user's intent
			// is not to multiselect, so deselect the selected row.  If multiple
			// rows are selected we don't want to deselect anything, since the
			// user's intent is to perform a context menu operation on all
			// selected rows.
			if (_dataGridView.SelectedRows.Count == 1)
			{
				foreach (DataGridViewRow row in _dataGridView.SelectedRows)
					row.Selected = false;
			}

			// Now select the new row
			if (info.RowIndex >= 0)
				_dataGridView.Rows[info.RowIndex].Selected = true;
        }

        private void _contextMenu_Opened(object sender, EventArgs e)
        {

        }

        private void _contextMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {

        }

        private void _contextMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {

        }
	}
}
