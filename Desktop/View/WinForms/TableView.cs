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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class TableView : UserControl
    {
        private event EventHandler _itemDoubleClicked;
        private event EventHandler _selectionChanged;
        private event EventHandler<ItemDragEventArgs> _itemDrag;

        private ActionModelNode _toolbarModel;
        private ActionModelNode _menuModel;

        private ITable _table;
        private bool _multiLine;

        private bool _delaySelectionChangeNotification = true; // see bug 386
        private bool _surpressSelectionChangedEvent = false;

        private bool _isLoaded = false;

        private const int CELL_SUBROW_HEIGHT = 18;
        private readonly int _rowHeight = 0;

    	private ISelection _selectionBeforeSort;

		public TableView()
        {
            InitializeComponent();

            // if we allow the framework to generate columns, there seems to be a bug with 
            // setting the minimum column width > 100 pixels
            // therefore, turn off the auto-generate and create the columns ourselves
            _dataGridView.AutoGenerateColumns = false;

            _rowHeight = this.DataGridView.RowTemplate.Height;
            this.DataGridView.RowPrePaint += SetCustomBackground;
            this.DataGridView.RowPostPaint += DisplayCellSubRows;
            this.DataGridView.RowPostPaint += OutlineCell;
            this.DataGridView.RowPostPaint += SetLinkColor;
        }

        #region Design Time properties and Events

        [DefaultValue(false)]
        public bool SortButtonVisible
        {
            get { return _sortButton.Visible; }
            set { _sortButton.Visible = value; }
        }

        [DefaultValue(false)]
        public bool FilterTextBoxVisible
        {
            get { return _filterTextBox.Visible; }
            set
            {
                _filterTextBox.Visible = value;
                _clearFilterButton.Visible = value;
            }
        }

        [DefaultValue(100)]
        public int FilterTextBoxWidth
        {
            get { return _filterTextBox.Width; }
            set { _filterTextBox.Size = new Size(value, _filterTextBox.Height); }
        }

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

		[DefaultValue(DataGridViewAutoSizeColumnsMode.Fill)]
		public DataGridViewAutoSizeColumnsMode AutoSizeColumnsMode
    	{
			get { return _dataGridView.AutoSizeColumnsMode; }
			set { _dataGridView.AutoSizeColumnsMode = value; }
    	}

        [DefaultValue(false)]
        [Description("Enables or disables multi-line rows.  If enabled, text longer than the column width is wrapped and the row is auto-sized. If disabled, a single line of truncated text is followed by an ellipsis")]
        public bool MultiLine
        {
            get { return _multiLine; }
            set
            {
                _multiLine = value;
                if (_multiLine)
                {
                    this._dataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    this._dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                }
                else
                {
                    this._dataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
                    this._dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                }
            }
        }

        [Obsolete("Toolstrip item display style is controlled by ToolStripBuilder.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolStripItemDisplayStyle ToolStripItemDisplayStyle
        {
            get { return ToolStripItemDisplayStyle.Image; }
            set
            {
            	// this is not a settable property anymore, but this is here for backward compile-time compatability
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int FirstDisplayedScrollingRowIndex
        {
            get { return _dataGridView.FirstDisplayedScrollingRowIndex; }
            set { _dataGridView.FirstDisplayedScrollingRowIndex = value; }
        }

        [DefaultValue(true)]
        public bool ShowToolbar
        {
            get { return _toolStrip.Visible; }
            set { _toolStrip.Visible = value; }
        }

        [DefaultValue(false)]
        public bool StatusBarVisible
        {
            get { return _statusStrip.Visible; }
            set { _statusStrip.Visible = value; }
        }

        [DefaultValue(true)]
        public bool ShowColumnHeading
        {
            get { return _dataGridView.ColumnHeadersVisible; }
            set { _dataGridView.ColumnHeadersVisible = value; }
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

        #endregion

        #region Public Properties and Events

        [Obsolete("Toolstrip item alignment is controlled by ToolStripBuilder.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RightToLeft ToolStripRightToLeft
        {
            get { return RightToLeft.No; }
			set
			{
				// this is not a settable property anymore, but this is here for backward compile-time compatability
			}
		}

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SuppressSelectionChangedEvent
        {
            get { return _surpressSelectionChangedEvent; }
            set { _surpressSelectionChangedEvent = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ActionModelNode ToolbarModel
        {
            get { return _toolbarModel; }
            set
            {
                _toolbarModel = value;

                // Defer initialization of ToolStrip until after Load() has been called
                // so that parameters from application settings are initialized properly
                if (_isLoaded) InitializeToolStrip();
            }
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ActionModelNode MenuModel
        {
            get { return _menuModel; }
            set
            {
                _menuModel = value;

                // Defer initialization of ToolStrip until after Load() has been called
                // so that parameters from application settings are initialized properly
                if (_isLoaded) InitializeMenu();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string StatusText
        {
            get { return _statusLabel.Text; }
            set { _statusLabel.Text = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITable Table
        {
            get
            {
                return _table;
            }
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
                    // Set a cell padding to provide space for the top of the focus 
                    // rectangle and for the content that spans multiple columns. 
                    var newPadding = new Padding(0, 1, 0,
                        CELL_SUBROW_HEIGHT * (_table.CellRowCount - 1));
                    this.DataGridView.RowTemplate.DefaultCellStyle.Padding = newPadding;

                    // Set the row height to accommodate the content that 
                    // spans multiple columns.
                    this.DataGridView.RowTemplate.Height = _rowHeight + CELL_SUBROW_HEIGHT * (_table.CellRowCount - 1);

                    // DataSource must be set after RowTemplate in order for changes to take effect
                    _dataGridView.DataSource = new TableAdapter(_table);
					_dataGridView.ColumnHeaderMouseClick += _dataGridView_ColumnHeaderMouseClick;

					_table.BeforeSorted += _table_BeforeSortedEvent;
					_table.Sorted += _table_SortedEvent;
                }

                InitializeSortButton();
                IntializeFilter();
            }
        }

        /// <summary>
        /// Gets/sets the current selection
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ISelection Selection
        {
            get
            {
                return GetSelectionHelper();
            }
            set
            {
                // if someone tries to assign null, just convert it to an empty selection - this makes everything easier
                var newSelection = value ?? new Selection();

                // get the existing selection
                var existingSelection = GetSelectionHelper();

                if (!existingSelection.Equals(newSelection))
                {
                    // de-select any rows that should not be selected
                    foreach (DataGridViewRow row in _dataGridView.SelectedRows)
                    {
                        if (!CollectionUtils.Contains(newSelection.Items,
							item => Equals(item, row.DataBoundItem)))
                        {
                            row.Selected = false;
                        }
                    }

                    // select any rows that should be selected
                    foreach (var item in newSelection.Items)
                    {
                        var row = CollectionUtils.SelectFirst(_dataGridView.Rows,
							(DataGridViewRow r) => Equals(item, r.DataBoundItem));
                        if (row != null)
                            row.Selected = true;
                    }

                    ForceSelectionDisplay();

                    NotifySelectionChanged();
                }
            }
        }

        /// <summary>
        /// Whenever the table is refreshed/modified by a component it tends to jump the DataGridView display
        /// to the top of the list, this isn't desirable. The following method forces the given selection to
        /// be visible on the control.
        /// </summary>
        private void ForceSelectionDisplay()
        {
            // check if ALL the selected entries are not visible to the user
            if (CollectionUtils.TrueForAll(_dataGridView.SelectedRows, (DataGridViewRow row) => !row.Displayed)
				&& _table.Items.Count != 0)
            {
                // create an array to capture the indicies of the selection collection (lol)
                // indicies needed for index position calculation of viewable index
                var selectedRows = new int[_dataGridView.SelectedRows.Count];
                var i = 0;
                foreach (DataGridViewRow row in _dataGridView.SelectedRows)
                {
                    selectedRows[i] = row.Index;
                    i++;
                }

                // create variables for the index of the last row and the number of rows displayable
                // by the control without scrolling
                // row differential then becomes the index in which the all the last displayable rows starts at
                var lastRow = _dataGridView.Rows.GetLastRow(new DataGridViewElementStates());
                var displayedRows = _dataGridView.DisplayedRowCount(false) - 1;
                var rowDifferential = lastRow - displayedRows; // calculate the differential 

                // pre-existing tables
                if (selectedRows.Length != 0)
                {
                    // if the first selection is less than the boundary last range of displayable
                    // rows, then set the first viewable row to the first selection, if not, set it
                    // to the boundary
                    if (selectedRows[0] < rowDifferential)
                        FirstDisplayedScrollingRowIndex = selectedRows[0];
                    else if (selectedRows[0] > rowDifferential)
                        FirstDisplayedScrollingRowIndex = rowDifferential;
                }
                // new tables obviously will have no entries in selectedRows therefore
                // automatically set it to the row differential which will probably be 0
                else
                {
                    FirstDisplayedScrollingRowIndex = 0;
                }
            }
            // strange oddity, this part actually never gets activated for some strange reason
            // intended to preserve the current index if there are displayable items already on screen
            else
            {
                if (FirstDisplayedScrollingRowIndex > 0)
                    FirstDisplayedScrollingRowIndex = FirstDisplayedScrollingRowIndex;
            }
        }

        /// <summary>
        /// Exposes the KeyDown event of the underlying data grid view.
        /// </summary>
        public event KeyEventHandler DataGridKeyDown
        {
            add { _dataGridView.KeyDown += value; }
            remove { _dataGridView.KeyDown -= value; }
        }

		/// <summary>
		/// Begins editing.
		/// </summary>
		/// <param name="selectAll"></param>
		public bool BeginEdit(bool selectAll)
		{
			var firstSelRow = (DataGridViewRow)CollectionUtils.FirstElement(_dataGridView.SelectedRows);
			var col = CollectionUtils.SelectFirst(_table.Columns, (ITableColumn c) => c.GetCellEditor() != null);
			if (firstSelRow != null && col != null)
			{
				var rowIndex = firstSelRow.Index;
				var colIndex = _table.Columns.IndexOf(col);

				_dataGridView.CurrentCell = _dataGridView[colIndex, rowIndex];

				return _dataGridView.BeginEdit(selectAll);
			}
			return false;
		}


        #endregion

        protected ToolStrip ToolStrip
        {
            get { return _toolStrip; }
        }

        protected new ContextMenuStrip ContextMenuStrip
        {
            get { return _contextMenu; }
        }

        private void InitializeToolStrip()
        {
            ToolStripBuilder.Clear(_toolStrip.Items);
            if (_toolbarModel != null)
            {
                ToolStripBuilder.BuildToolbar(_toolStrip.Items, _toolbarModel.ChildNodes);
            }
        }

        private void InitializeMenu()
        {
            ToolStripBuilder.Clear(_contextMenu.Items);
            if (_menuModel != null)
            {
                ToolStripBuilder.BuildMenu(_contextMenu.Items, _menuModel.ChildNodes);
            }
        }

        private Selection GetSelectionHelper()
        {
            return new Selection(
                CollectionUtils.Map(_dataGridView.SelectedRows,
                                    (DataGridViewRow row) => row.DataBoundItem));
        }

        private void InitColumns()
        {
            // clear the old columns
            _dataGridView.Columns.Clear();

            if (_table != null)
            {
                var fontSize = this.Font.SizeInPoints;
                foreach (ITableColumn col in _table.Columns)
                {
                    // this is ugly but somebody's gotta do it
                    DataGridViewColumn dgcol;
                    if (col.ColumnType == typeof(bool))
                        dgcol = new DataGridViewCheckBoxColumn();
                    else if (col.ColumnType == typeof(Image) || col.ColumnType == typeof(IconSet))
                    {
                        dgcol = new DataGridViewImageColumn();

                        dgcol.SortMode = DataGridViewColumnSortMode.Automatic;

                        // Set the default to display nothing if not icons are provided.
                        // Otherwise WinForms will by default display an ugly icon with 'x'
                        dgcol.DefaultCellStyle.NullValue = null;
                    }
                    else if (col.HasClickableLink)
                    {
                        dgcol = new DataGridViewLinkColumn();
                        var linkColumn = (DataGridViewLinkColumn)dgcol;
                        linkColumn.LinkBehavior = LinkBehavior.SystemDefault;
                        linkColumn.TrackVisitedState = false;
                        linkColumn.SortMode = DataGridViewColumnSortMode.Automatic;
                    }
                    else
                    {
                        // assume any other type of column will be displayed as text
						// if it provides a custom editor, then we need to use a sub-class of the text box column
                        dgcol = (col.GetCellEditor() != null) ? 
							(DataGridViewColumn) new CustomEditableTableViewColumn(_table, col) : new DataGridViewTextBoxColumn();
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

                    col.VisibleChanged += OnColumnVisibilityChanged;

                    _dataGridView.Columns.Add(dgcol);
                }
            	_table.Columns.ItemsChanged += OnColumnsChanged;
            }
        }

        private void UnsubscribeFromOldTable()
        {
            if (_table != null)
            {
                foreach (ITableColumn column in _table.Columns)
                    column.VisibleChanged -= OnColumnVisibilityChanged;

				_dataGridView.ColumnHeaderMouseClick -= _dataGridView_ColumnHeaderMouseClick;
				_table.Columns.ItemsChanged -= OnColumnsChanged;
				_table.BeforeSorted -= _table_BeforeSortedEvent;
				_table.Sorted -= _table_SortedEvent;
            }
        }

    	private void OnColumnsChanged(object sender, ItemChangedEventArgs e)
    	{
    		this.Table = this.Table;
    	}

        private void OnColumnVisibilityChanged(object sender, EventArgs e)
        {
            // NY: Yes, I know, this is really cheap. The original plan was
            // to use anonymous delegates to "bind" the ITableColumn to the
            // DataGridViewColumn, but unsubscribing from ITableColumn.VisiblityChanged
            // was problematic.  This is the next best thing if we want
            // easy unsubscription.
            var column = (ITableColumn)sender;  //Invalid cast is a programming error, so let exception be thrown
            var dgcolumn = FindDataGridViewColumn(column);

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

        // Paints the custom background for each row
        private void SetCustomBackground(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if ((e.State & DataGridViewElementStates.Selected) ==
                        DataGridViewElementStates.Selected)
            {
                // do nothing?
                return;
            }

            if (_table != null)
            {
                var rowBounds = GetAdjustedRowBounds(e.RowBounds);

                // Color.FromName("Empty") does not return Color.Empty, so need to manually check for Empty
                var colorName = _table.GetItemBackgroundColor(_table.Items[e.RowIndex]);
                var backgroundColor = string.IsNullOrEmpty(colorName) || colorName.Equals("Empty") ? Color.Empty : Color.FromName(colorName);

                if (backgroundColor.Equals(Color.Empty))
                {
                    backgroundColor = e.InheritedRowStyle.BackColor;
                }

                // Paint the custom selection background.
                using (Brush backbrush =
                    new SolidBrush(backgroundColor))
                {
                    e.PaintParts &= ~DataGridViewPaintParts.Background;
                    e.Graphics.FillRectangle(backbrush, rowBounds);
                }
            }
        }

        // Paints the custom outline for each row
        private void OutlineCell(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var rowBounds = GetAdjustedRowBounds(e.RowBounds);

            if (_table != null)
            {
                const int penWidth = 2;
                var outline = new Rectangle(
                    rowBounds.Left + penWidth / 2,
                    rowBounds.Top + penWidth / 2 + 1,
                    rowBounds.Width - penWidth,
                    rowBounds.Height - penWidth - 2);

                var colorName = _table.GetItemOutlineColor(_table.Items[e.RowIndex]);
                var outlineColor = string.IsNullOrEmpty(colorName) || colorName.Equals("Empty") ? Color.Empty : Color.FromName(colorName);
                using (var outlinePen = new Pen(outlineColor, penWidth))
                {
                    e.Graphics.DrawRectangle(outlinePen, outline);
                }
            }
        }

        // Paints the content that spans multiple columns and the focus rectangle.
        private void DisplayCellSubRows(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var rowBounds = GetAdjustedRowBounds(e.RowBounds);

            SolidBrush forebrush = null;
            try
            {
                // Determine the foreground color.
                if ((e.State & DataGridViewElementStates.Selected) ==
                    DataGridViewElementStates.Selected)
                {
                    forebrush = new SolidBrush(e.InheritedRowStyle.SelectionForeColor);
                }
                else
                {
                    forebrush = new SolidBrush(e.InheritedRowStyle.ForeColor);
                }

                // Store text for each subrow
                var sb = new StringBuilder[_table.CellRowCount];
                for (var i = 0; i < _table.CellRowCount; i++)
                {
                    sb[i] = new StringBuilder();
                }

                for (var i = 0; i < _table.Columns.Count; i++)
                {
                    var col = _table.Columns[i] as ITableColumn;
                    if (col != null && col.CellRow > 0)
                    {
                        var row = this.DataGridView.Rows[e.RowIndex];
                        var recipe = row.Index != -1 ? row.Cells[i].Value : null;

                        if (recipe != null)
                        {
                            sb[col.CellRow].Append(recipe + " ");
                        }

                    }
                }

                // Draw text for each sub row (Rows 1 and higher in the Table)
                for (var i = 1; i < _table.CellRowCount; i++)
                {
                    var text = sb[i].ToString().Trim();

                    if (string.IsNullOrEmpty(text) == false)
                    {
                        // Calculate the bounds for the content that spans multiple 
                        // columns, adjusting for the horizontal scrolling position 
                        // and the current row height, and displaying only whole
                        // lines of text.
                        var textArea = rowBounds;
                        textArea.X -= this.DataGridView.HorizontalScrollingOffset;
                        textArea.Width += this.DataGridView.HorizontalScrollingOffset;
                        textArea.Y += _rowHeight + (i - 1) * CELL_SUBROW_HEIGHT;
                        textArea.Height = CELL_SUBROW_HEIGHT;

                        // Calculate the portion of the text area that needs painting.
                        RectangleF clip = textArea;
                        var startX = this.DataGridView.RowHeadersVisible ? this.DataGridView.RowHeadersWidth : 0;
                        clip.Width -= startX + 1 - clip.X;
                        clip.X = startX + 1;
                        var oldClip = e.Graphics.ClipBounds;
                        e.Graphics.SetClip(clip);

                        // Use a different font for subrows
                        // TODO: Make this a parameter of the Table
                        //Font subRowFont = new Font(e.InheritedRowStyle.Font, FontStyle.Italic);

                        var format = new StringFormat
                                     	{
                                     		FormatFlags = StringFormatFlags.NoWrap,
                                     		Trimming = StringTrimming.EllipsisWord
                                     	};

                    	// Draw the content that spans multiple columns.
                        e.Graphics.DrawString(text, e.InheritedRowStyle.Font, forebrush, textArea, format);

                        e.Graphics.SetClip(oldClip);
                    }
                }
            }
            finally
            {
                if (forebrush != null)
                    forebrush.Dispose();
            }
        }

        // Make all the link column the same color as the fore color
        private void SetLinkColor(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var row = _dataGridView.Rows[e.RowIndex];
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (cell is DataGridViewLinkCell)
                {
                    var linkCell = (DataGridViewLinkCell)cell;
                    linkCell.ActiveLinkColor = linkCell.LinkColor = linkCell.VisitedLinkColor
                        = row.Selected ? cell.InheritedStyle.SelectionForeColor : cell.InheritedStyle.ForeColor;
                }
            }
        }

        private Rectangle GetAdjustedRowBounds(Rectangle rowBounds)
        {
            return new Rectangle(
                    (this.DataGridView.RowHeadersVisible ? this.DataGridView.RowHeadersWidth : 0) + rowBounds.Left,
                    rowBounds.Top,
                    this.DataGridView.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - this.DataGridView.HorizontalScrollingOffset,
                    rowBounds.Height);
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
            var pt = _dataGridView.PointToClient(MousePosition);
            var info = _dataGridView.HitTest(pt.X, pt.Y);


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
            if (_surpressSelectionChangedEvent)
                return;

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

            var args = new ItemDragEventArgs(e.Button, this.GetSelectionHelper());
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
            var pending = _selectionChangeTimer.Enabled;
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
            if (_surpressSelectionChangedEvent)
                return;

            // notify clients of this class of a *real* selection change
            EventsHelper.Fire(_selectionChanged, this, EventArgs.Empty);
        }


        protected DataGridView DataGridView
        {
            get { return _dataGridView; }
        }

        private void TableView_Load(object sender, EventArgs e)
        {
            InitializeMenu();
            InitializeToolStrip();

            _isLoaded = true;
        }

        private void InitializeSortButton()
        {
            if (_table == null || _table.Columns.Count == 0)
            {
                _sortButton.Enabled = false;
            }
            else
            {
                // Rebuild dropdown menu
                _sortButton.Enabled = true;
                _sortButton.DropDownItems.Clear();
                _sortButton.DropDownItems.Add(_sortAscendingButton);
                _sortButton.DropDownItems.Add(_sortDescendingButton);
                _sortButton.DropDownItems.Add(_sortSeparator);

				foreach (ITableColumn column in _table.Columns)
            	{
					ToolStripItem item = new ToolStripMenuItem(column.Name, null, _sortButtonDropDownItem_Click, column.Name);
					if (_sortButton.DropDownItems.ContainsKey(column.Name) == false)
						_sortButton.DropDownItems.Add(item);
				}

                ResetSortButtonState();
            }
        }

        private void ResetSortButtonState()
        {
            if (_table == null || _table.SortParams == null)
                return;

			foreach (ToolStripItem item in _sortButton.DropDownItems)
        	{
				if (item == _sortAscendingButton)
					this.SortAscendingButtonCheck = _table.SortParams.Ascending;
				else if (item == _sortDescendingButton)
					this.SortDescendingButtonCheck = _table.SortParams.Ascending == false;
				else if (item == _sortSeparator)
					return;
				else
				{
					if (item.Name.Equals(_table.SortParams.Column.Name))
					{
						item.Image = SR.CheckSmall;
						_sortButton.ToolTipText = String.Format(SR.MessageSortBy, item.Name);
					}
					else
					{
						item.Image = null;
					}
				}
			}
        }

        private bool SortAscendingButtonCheck
        {
            get { return _sortAscendingButton.Image != null; }
            set { _sortAscendingButton.Image = value ? SR.CheckSmall : null; }
        }

        private bool SortDescendingButtonCheck
        {
            get { return _sortDescendingButton.Image != null; }
            set { _sortDescendingButton.Image = value ? SR.CheckSmall : null; }
        }

        private void _table_BeforeSortedEvent(object sender, EventArgs e)
        {
        	_selectionBeforeSort = this.Selection;
        }

		private void _table_SortedEvent(object sender, EventArgs e)
        {
			if (_selectionBeforeSort.Items.Length > 0)
				this.Selection = _selectionBeforeSort;

			ResetSortButtonState();
		}

		private void _dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			ForceSelectionDisplay();
		}
		
		private void sortAscendingButton_Click(object sender, EventArgs e)
        {
            if (_table == null || _table.SortParams == null)
                return;

            _table.SortParams.Ascending = true;
            _table.Sort(_table.SortParams);
        }

        private void sortDescendingButton_Click(object sender, EventArgs e)
        {
            if (_table == null || _table.SortParams == null)
                return;

            _table.SortParams.Ascending = false;
            _table.Sort(_table.SortParams);
        }

        private void _sortButtonDropDownItem_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            var sortColumn = CollectionUtils.SelectFirst(_table.Columns, (ITableColumn column) => column.Name.Equals(item.Name));

            if (sortColumn != null)
            {
                var sortParams = new TableSortParams(sortColumn, false);
                _table.Sort(sortParams);
            }
        }

        private void IntializeFilter()
        {
            _filterTextBox.Enabled = (_table != null);

            if (_table != null && _table.FilterParams != null && _table.FilterParams.Value is string)
            {
                _filterTextBox.Text = (string)_table.FilterParams.Value;
            }
            else
                _filterTextBox.Text = "";
        }

        private void _clearFilterButton_Click(object sender, EventArgs e)
        {
            _filterTextBox.Text = "";
        }

        private void _filterText_TextChanged(object sender, EventArgs e)
        {
            if (_table == null)
                return;


            if (String.IsNullOrEmpty(_filterTextBox.Text))
            {
                _filterTextBox.ToolTipText = SR.MessageEmptyFilter;
                _clearFilterButton.Enabled = false;
                _table.RemoveFilter();
            }
            else
            {
                _filterTextBox.ToolTipText = String.Format(SR.MessageFilterBy, _filterTextBox.Text);
                _clearFilterButton.Enabled = true;
                var filterParams = new TableFilterParams(null, _filterTextBox.Text);
                _table.Filter(filterParams);
            }

            // Refresh the current table
            this.Table = _table;
        }

        private void _dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore header cells
            if (e.RowIndex == -1)
                return;

            var dgCol = _dataGridView.Columns[e.ColumnIndex];
            if (dgCol is DataGridViewLinkColumn)
            {
                var col = (ITableColumn)dgCol.Tag;
                col.ClickLink(_table.Items[e.RowIndex]);
            }
            else if (dgCol is DataGridViewButtonColumn)
            {
                throw new NotImplementedException();
            }
        }

		private void _dataGridView_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
		{
            // Ignore header cells
            if (e.RowIndex == -1)
                return;

			var column = (ITableColumn)_dataGridView.Columns[e.ColumnIndex].Tag;
			e.ToolTipText = column.GetTooltipText(_table.Items[e.RowIndex]);
		}

		private void _dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			var column = (ITableColumn)_dataGridView.Columns[e.ColumnIndex].Tag;

			// Unless we know the type of e.Value can be handled by the DataGridView, we do not want to set e.FormattingApplied to true. Doing so will 
			// prevent the cell from formatting e.Value into type it can handle (eg. string), result in FormatException for value type like int, float, etc.			
			if (column.ColumnType == typeof(IconSet))
			{
				try
				{
					// try to create the icon
					var iconSet = (IconSet)e.Value;
					if (iconSet != null)
						e.Value = IconFactory.CreateIcon(iconSet.SmallIcon, column.ResourceResolver);
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex);
				}
			}
			else
			{
				e.Value = column.FormatValue(e.Value);
			}
		}
    }
}
