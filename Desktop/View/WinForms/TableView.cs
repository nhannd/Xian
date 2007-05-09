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
        private event EventHandler _itemDoubleClicked;
        private event EventHandler _selectionChanged;
        private event EventHandler<ItemDragEventArgs> _itemDrag;

        private ActionModelNode _toolbarModel;
        private ActionModelNode _menuModel;
		private ToolStripItemDisplayStyle _toolStripItemDisplayStyle = ToolStripItemDisplayStyle.Image;
        private ITable _table;
        private bool _multiLine;

        private bool _delaySelectionChangeNotification = true; // see bug 386

		public TableView()
		{
			InitializeComponent();

            // if we allow the framework to generate columns, there seems to be a bug with 
            // setting the minimum column width > 100 pixels
            // therefore, turn off the auto-generate and create the columns ourselves
            _dataGridView.AutoGenerateColumns = false;
        }

        #region Design Time properties

        [DefaultValue(true)]
        public bool ReadOnly
        {
            get { return _dataGridView.ReadOnly; }
            set { _dataGridView.ReadOnly = value; }
        }

        [DefaultValue(true)]
        public bool MultiSelect
        {
            get { return _dataGridView.MultiSelect; }
            set { _dataGridView.MultiSelect = value; }
        }

        [DefaultValue(false)]
        [Description("Enables or disables multi-line rows.  If enabled, text longer than the column width is wrapped and the row is auto-sized. If disabled, a single line of truncated text is followed by an ellipsis")]
        public bool MultiLine
        {
            get { return _multiLine; }
            set
            {
                _multiLine = value;
                if (_multiLine == true)
                {
                    this._dataGridView.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
                    this._dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
                }
                else
                {
                    this._dataGridView.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
                    this._dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.None;
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

        [DefaultValue(true)]
        public bool ShowToolbar
        {
            get { return _toolStrip.Visible; }
            set { _toolStrip.Visible = value; }
        }

		protected ToolStrip ToolStrip
		{
			get { return _toolStrip; }
		}

		protected new ContextMenuStrip ContextMenuStrip
		{
			get { return _contextMenu; }
		}

        #endregion

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
                        ToolStripBuilder.BuildToolbar(_toolStrip.Items, childNodesCopy, _toolStripItemDisplayStyle);
                    }
                    else
                    {
                        ToolStripBuilder.BuildToolbar(_toolStrip.Items, _toolbarModel.ChildNodes, _toolStripItemDisplayStyle);
                    }
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
				UnsubscribeFromOldTable();

                _table = value;

                // by setting the datasource to null here, we eliminate the SelectionChanged events that
                // would get fired during the call to InitColumns()
                _dataGridView.DataSource = null;

                InitColumns();

                if (_table != null)
                {
                    //_bindingSource.DataSource = new TableAdapter(_table);
                    //_dataGridView.DataSource = _bindingSource;
                    _dataGridView.DataSource = new TableAdapter(_table);
                }
            }
        }

        /// <summary>
        /// Gets/sets the current selection
        /// </summary>
        public ISelection Selection
        {
            get
            {
                return GetSelectionHelper();
            }
            set
            {
                // if someone tries to assign null, just convert it to an empty selection - this makes everything easier
                ISelection newSelection = (value == null) ? new Selection() : value;

                // get the existing selection
                ISelection existingSelection = GetSelectionHelper();

                if (!existingSelection.Equals(newSelection))
                {
                    // de-select any rows that should not be selected
                    foreach (DataGridViewRow row in _dataGridView.SelectedRows)
                    {
                        if (!CollectionUtils.Contains(newSelection.Items, delegate(object item) { return item == row.DataBoundItem; }))
                        {
                            row.Selected = false;
                        }
                    }

                    // select any rows that should be selected
                    foreach (object item in newSelection.Items)
                    {
                        DataGridViewRow row = CollectionUtils.SelectFirst<DataGridViewRow>(_dataGridView.Rows,
                                delegate(DataGridViewRow r) { return r.DataBoundItem == item; });
                        if (row != null)
                            row.Selected = true;
                    }

                    EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler SelectionChanged
        {
            add { _selectionChanged += value; }
            remove { _selectionChanged -= value; }
        }

        public event EventHandler ItemDoubleClicked
        {
            add { _itemDoubleClicked += value; }
            remove { _itemDoubleClicked -= value; }
        }

        public event EventHandler<ItemDragEventArgs> ItemDrag
        {
            add { _itemDrag += value; }
            remove { _itemDrag -= value; }
        }

        private Selection GetSelectionHelper()
        {
            return new Selection(
                CollectionUtils.Map<DataGridViewRow, object>(_dataGridView.SelectedRows,
                    delegate(DataGridViewRow row) { return row.DataBoundItem; }));
        }

        private void InitColumns()
        {
            // clear the old columns
            _dataGridView.Columns.Clear();

            if (_table != null)
            {
                float fontSize = this.Font.SizeInPoints;
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
                    dgcol.ReadOnly = col.ReadOnly;
                    dgcol.MinimumWidth = (int)(col.WidthFactor * _table.BaseColumnWidthChars * fontSize);
                    dgcol.FillWeight = col.WidthFactor;
					dgcol.Visible = col.Visible;

					// Associate the ITableColumn with the DataGridViewColumn
					dgcol.Tag = col;

					col.VisibilityChanged += OnColumnVisibilityChanged;

                    _dataGridView.Columns.Add(dgcol);
                }
            }
        }

		private void UnsubscribeFromOldTable()
		{
			if (_table != null)
			{
				foreach (ITableColumn column in _table.Columns)
					column.VisibilityChanged -= OnColumnVisibilityChanged;
			}
		}

		private void OnColumnVisibilityChanged(object sender, EventArgs e)
		{
			// NY: Yes, I know, this is really cheap. The original plan was
			// to use anonymous delegates to "bind" the ITableColumn to the
			// DataGridViewColumn, but unsubscribing from ITableColumn.VisiblityChanged
			// was problematic.  This is the next best thing if we want
			// easy unsubscription.
			ITableColumn column = sender as ITableColumn;
			DataGridViewColumn dgcolumn = FindDataGridViewColumn(column);

			if (dgcolumn != null)
				dgcolumn.Visible = column.Visible;
		}

		private DataGridViewColumn FindDataGridViewColumn(ITableColumn column)
		{
			foreach (DataGridViewColumn dgcolumn in _dataGridView.Columns)
			{
				if (dgcolumn.Tag == column)
					return dgcolumn;
			}

			return null;
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
            // if a context menu is being opened, need to flush any pending selection change notification immediately before
            // showing menu (bug 386)
            FlushPendingSelectionChangeNotification();
            
            // Find the row we're on
			Point pt = _dataGridView.PointToClient(DataGridView.MousePosition);
			DataGridView.HitTestInfo info = _dataGridView.HitTest(pt.X, pt.Y);


            try
            {
                // temporarily disable the delaying of selection change notifications
                // if we modify the selection while opening the context menu, we need those notifications to propagate immediately
                _delaySelectionChangeNotification = false;

                if (_dataGridView.SelectedRows.Count == 0)
                {
                    // select the new row
                    if (info.RowIndex >= 0)
                        _dataGridView.Rows[info.RowIndex].Selected = true;
                }
                else if (_dataGridView.SelectedRows.Count == 1 && _dataGridView.SelectedRows[0].Index != info.RowIndex)
                {
                    // deselect the selected row
                    _dataGridView.SelectedRows[0].Selected = false;

                    // Now select the new row
                    if (info.RowIndex >= 0)
                        _dataGridView.Rows[info.RowIndex].Selected = true;
                }
                else
                {
                    // If multiple
                    // rows are selected we don't want to deselect anything, since the
                    // user's intent is to perform a context menu operation on all
                    // selected rows.
                }

            }
            finally
            {
                // re-enable the delaying of selection change notifications
                _delaySelectionChangeNotification = true;
            }
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

        private void _dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (_delaySelectionChangeNotification)
            {
                // fix Bug 386: rather than firing our own _selectionChanged event immediately, post delayed notification
                PostSelectionChangeNotification();
            }
            else
            {
                NotifySelectionChanged();
            }
        }

        /// <summary>
        /// Handling this event is necessary to ensure that changes to checkbox cells are propagated
        /// immediately to the underlying <see cref="ITable"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // if the state of a checkbox cell has changed, commit the edit immediately
            if (_dataGridView.CurrentCell is DataGridViewCheckBoxCell)
            {
                _dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        /// <summary>
        /// Handle the ItemDrag event of the internal control, so that this control can fire its own 
        /// event, using the current selection as the "item" that is being dragged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _dataGridView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // if a drag is being initiated, need to flush any pending selection change notification immediately before
            // proceeding (bug 386)
            FlushPendingSelectionChangeNotification();

            ItemDragEventArgs args = new ItemDragEventArgs(e.Button, this.GetSelectionHelper());
            EventsHelper.Fire(_itemDrag, this, args);
        }

        /// <summary>
        /// Fix Bug 386: Add a 50ms delay before posting selection changes to outside clients
        /// this has the effect of filtering out very quick selection changes that usually reflect
        /// "bugs" in the windowing framework rather than user actions
        /// </summary>
        private void PostSelectionChangeNotification()
        {
            // restart the timer - this effectively "posts" a selection change notification to occur on the timer tick
            // if a change notification is already pending, it is cleared
            _selectionChangeTimer.Stop();
            _selectionChangeTimer.Start();
        }

        /// <summary>
        /// If a selection change notification is pending, this method will force it to occur now rather than
        /// waiting for the timer tick. (Bug 386)
        /// </summary>
        private void FlushPendingSelectionChangeNotification()
        {
            bool pending = _selectionChangeTimer.Enabled;
            _selectionChangeTimer.Stop();   // stop the timer before firing event, in case event handler runs long duration

            // if there was a "pending" notification, send it now
            if (pending)
            {
                NotifySelectionChanged();
            }
        }

        /// <summary>
        /// Delayed selection change notification (fix for bug 386)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _selectionChangeTimer_Tick(object sender, EventArgs e)
        {
            // stop the timer (one-shot behaviour)
            _selectionChangeTimer.Stop();

            // notify clients
            NotifySelectionChanged();
        }

        private void NotifySelectionChanged()
        {
            // notify clients of this class of a *real* selection change
            EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
        }


        protected DataGridView DataGridView
        {
            get { return _dataGridView; }
        }
	}
}
