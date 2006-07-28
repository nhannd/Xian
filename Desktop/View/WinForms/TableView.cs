using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

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

		public TableView()
		{
			InitializeComponent();
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
                        ActionModelNode[] childNodesCopy = new ActionModelNode[_toolbarModel.ChildNodes.Length];
                        for (int i = 0; i < _toolbarModel.ChildNodes.Length; i++)
                        {
                                childNodesCopy[childNodesCopy.Length - i - 1] = _toolbarModel.ChildNodes[i];
                        }
                        ToolStripBuilder.BuildToolbar(_toolStrip.Items, childNodesCopy);
                    }
                    else
                    {
                        ToolStripBuilder.BuildToolbar(_toolStrip.Items, _toolbarModel.ChildNodes);
                    }
                }
            }
        }

        public object DataSource
        {
            get { return _bindingSource.DataSource; }
            set
            {
                _bindingSource.DataSource = value;
                _dataGridView.DataSource = _bindingSource;
            }
        }

        public RightToLeft ToolStripRightToLeft
        {
            get { return _toolStrip.RightToLeft; }
            set { _toolStrip.RightToLeft = value; }
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

        private void _dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)    // rowindex == -1 represents a header click
            {
                EventsHelper.Fire(_itemDoubleClicked, this, new EventArgs());
            }
        }
	}
}
