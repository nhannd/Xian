using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// A control for picking the dimensions of a table.
	/// </summary>
	/// <remarks>
	/// <para>In this control, dimension width and height are equivalent to table columns and rows, respectively.</para>
	/// </remarks>
	[DefaultEvent("DimensionsSelected")]
	[DefaultProperty("Dimensions")]
	public class TableDimensionsPicker : Control
	{
		private static readonly Size DEFAULT_DIMS = new Size(4, 4);
		private event EventHandler _dimensionsChanged;
		private event EventHandler _maxDimensionsChanged;
		private event EventHandler _hotDimensionsChanged;
		private event TableDimensionsEventHandler _dimensionsSelected;

		private Size _hotDims;
		private Size _maxDims;
		private Size _dims;

		private Color _cellBorderColor;
		private int _cellBorderWidth;

		private Color _highlightColor;
		private Color _activeColor;

		#region Constructors

		/// <summary>
		/// Constructs a <see cref="TableDimensionsPicker"/> showing four rows and columns.
		/// </summary>
		public TableDimensionsPicker() : this(DEFAULT_DIMS) {}

		/// <summary>
		/// Constructs a <see cref="TableDimensionsPicker"/> showing the specified number of rows and columns.
		/// </summary>
		/// <remarks>
		/// <para>Dimension width and height are equivalent to table columns and rows, respectively.</para>
		/// </remarks>
		/// <param name="maxRows">The number of rows to show on the control, and hence the maximum the user can select.</param>
		/// <param name="maxCols">The number of columns to show on the control, and hence the maximum the user can select.</param>
		public TableDimensionsPicker(int maxRows, int maxCols) : this(new Size(maxCols, maxRows)) {}

		/// <summary>
		/// Constructs a <see cref="TableDimensionsPicker"/> showing the specified table size.
		/// </summary>
		/// <remarks>
		/// <para>Dimension width and height are equivalent to table columns and rows, respectively.</para>
		/// </remarks>
		/// <param name="maxDimensions">The table size show on the control, and hence the maximum size the user can select.</param>
		public TableDimensionsPicker(Size maxDimensions)
		{
			_maxDims = maxDimensions;
			_hotDims = _dims = Size.Empty;
			ResetCellBorderWidth();
			ResetCellBorderColor();
			ResetActiveColor();
			ResetHighlightColor();
			base.Size = GetDefaultSize();
			base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		}

		#endregion

		#region Display Option Properties (and their Reset methods)

		/// <summary>
		/// Gets or sets the pixel width of the cell borders.
		/// </summary>
		/// <remarks>
		/// The default border width is 1.
		/// </remarks>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(1)]
		[Description("The pixel width of the cell borders.")]
		public int CellBorderWidth
		{
			get { return _cellBorderWidth; }
			set
			{
				if (_cellBorderWidth != value)
				{
					_cellBorderWidth = value;
					base.Invalidate();
				}
			}
		}

		/// <summary>
		/// Resets the pixel width of the cell borders to the default value of 1.
		/// </summary>
		public void ResetCellBorderWidth()
		{
			_cellBorderWidth = 1;
		}

		/// <summary>
		/// Gets or sets the color of the cell borders.
		/// </summary>
		/// <remarks>
		/// The default border color is <see cref="Color.Black"/>.
		/// </remarks>
		[Bindable(true)]
		[Category("Appearance")]
		[Description("The color of the cell borders.")]
		public Color CellBorderColor
		{
			get { return _cellBorderColor; }
			set
			{
				if (_cellBorderColor != value)
				{
					_cellBorderColor = value;
					base.Invalidate();
				}
			}
		}

		/// <summary>
		/// Resets the color of the cell borders to the default value of <see cref="Color.Black"/>.
		/// </summary>
		public void ResetCellBorderColor()
		{
			_cellBorderColor = Color.Black;
		}

		/// <summary>
		/// Determines if the current color of the cell borders is the default value.
		/// </summary>
		/// <returns>False if it is the default, True otherwise.</returns>
		public bool ShouldSerializeCellBorderColor()
		{
			return !(this.CellBorderColor == Color.Black);
		}

		/// <summary>
		/// Gets or sets the hot-tracking color that will highlight the dimensions over which the cursor is hovering.
		/// </summary>
		/// <remarks>
		/// The default hot-tracking color is <see cref="SystemColors.HotTrack"/>.
		/// </remarks>
		[Bindable(true)]
		[Category("Appearance")]
		[Description("The hot-tracking color that will highlight the dimensions over which the cursor is hovering.")]
		public Color HighlightColor
		{
			get { return _highlightColor; }
			set
			{
				if (_highlightColor != value)
				{
					_highlightColor = value;
					base.Invalidate();
				}
			}
		}

		/// <summary>
		/// Resets the hot-tracking color that will highlight the dimensions over which the cursor is hovering to the default value of <see cref="SystemColors.HotTrack"/>.
		/// </summary>
		public void ResetHighlightColor()
		{
			_highlightColor = SystemColors.HotTrack;
		}

		/// <summary>
		/// Determines if the current hot-tracking color is the default value.
		/// </summary>
		/// <returns>False if it is the default, True otherwise.</returns>
		public bool ShouldSerializeHighlightColor()
		{
			return !(this.HighlightColor == SystemColors.HotTrack);
		}

		/// <summary>
		/// Gets or sets the color that will highlight the currently selected dimensions.
		/// </summary>
		/// <remarks>
		/// The default active color is <see cref="Color.Transparent"/>.
		/// </remarks>
		[Bindable(true)]
		[Category("Appearance")]
		[Description("The color that will highlight the currently selected dimensions.")]
		public Color ActiveColor
		{
			get { return _activeColor; }
			set
			{
				if (_activeColor != value)
				{
					_activeColor = value;
					base.Invalidate();
				}
			}
		}

		/// <summary>
		/// Resets the color that will highlight the currently selected dimensions to the default value of <see cref="Color.Transparent"/>.
		/// </summary>
		public void ResetActiveColor()
		{
			_activeColor = Color.Transparent;
		}

		/// <summary>
		/// Determined if the current selection color is the default value.
		/// </summary>
		/// <returns>False if it is the default, True otherwise.</returns>
		public bool ShouldSerializeActiveColor()
		{
			return !(this.ActiveColor == Color.Transparent);
		}

		#endregion

		#region Designer Properties (and their Events and Reset methods)

		/// <summary>
		/// Fired when the <see cref="MaxDimensions"/> property changes.
		/// </summary>
		[Description("Fired when the maximum dimensions change.")]
		[Category("Property Changed")]
		public event EventHandler MaxDimensionsChanged
		{
			add { _maxDimensionsChanged += value; }
			remove { _maxDimensionsChanged -= value; }
		}

		/// <summary>
		/// Notifies listeners that the <see cref="MaxDimensions"/> property has changed.
		/// </summary>
		protected virtual void NotifyMaxDimensionsChanged()
		{
			_hotDims = Size.Empty;
			base.Invalidate();

			if (_maxDimensionsChanged != null)
				_maxDimensionsChanged(this, new EventArgs());
		}

		/// <summary>
		/// Gets or sets the maximum dimensions to show on the control.
		/// </summary>
		/// <remarks>
		/// The default maximum dimensions is 4x4.
		/// </remarks>
		[Bindable(true)]
		[Category("Behavior")]
		[Description("The maximum dimensions to show on the control.")]
		public Size MaxDimensions
		{
			get { return _maxDims; }
			set
			{
				if (_maxDims != value)
				{
					_maxDims = value;
					NotifyMaxDimensionsChanged();
				}
			}
		}

		/// <summary>
		/// Resets the maximum dimensions to show on the control to the default value of 4x4.
		/// </summary>
		public void ResetMaxDimensions()
		{
			_maxDims = DEFAULT_DIMS;
		}

		/// <summary>
		/// Determines if the maximum dimensions to show on the control is the default value.
		/// </summary>
		/// <returns>False if it is the default, True otherwise.</returns>
		public bool ShouldSerializeMaxDimensions()
		{
			return !(_maxDims == DEFAULT_DIMS);
		}

		/// <summary>
		/// Fired when the user selects new dimensions.
		/// </summary>
		/// <remarks>
		/// This event is only fired when the <see cref="Dimensions"/> property changes as a result of user action.
		/// Programmatically changing the property does not fired this event. However, both methods will trigger the
		/// <see cref="DimensionsChanged"/> event.
		/// </remarks>
		[Description("Fired when the user selects new dimensions.")]
		[Category("Action")]
		public event TableDimensionsEventHandler DimensionsSelected
		{
			add { _dimensionsSelected += value; }
			remove { _dimensionsSelected -= value; }
		}

		/// <summary>
		/// Notifies listeners that the user has selected new dimensions.
		/// </summary>
		protected virtual void NotifyDimensionsSelected()
		{
			if (_dimensionsSelected != null)
				_dimensionsSelected(this, new TableDimensionsEventArgs(this.Dimensions));
		}

		/// <summary>
		/// Fired when the <see cref="Dimensions"/> property changes.
		/// </summary>
		[Description("Fired when the currently selected dimensions change.")]
		[Category("Property Changed")]
		public event EventHandler DimensionsChanged
		{
			add { _dimensionsChanged += value; }
			remove { _dimensionsChanged -= value; }
		}

		/// <summary>
		/// Notifies listeners that the <see cref="Dimensions"/> property has changed.
		/// </summary>
		/// <param name="newDims">The new value of the property.</param>
		/// <param name="oldDims">The old value of the property.</param>
		protected virtual void NotifyDimensionsChanged(Size newDims, Size oldDims)
		{
			base.Invalidate(GetInvalidRegion(newDims, oldDims));

			if (_dimensionsChanged != null)
				_dimensionsChanged(this, new EventArgs());
		}

		/// <summary>
		/// Gets or sets the currently selected dimensions.
		/// </summary>
		/// <remarks>
		/// The default selected dimensions is 0x0.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the dimensions are greater than the <see cref="MaxDimensions"/>.</exception>
		[Category("Behavior")]
		[Description("The currently selected dimensions.")]
		public Size Dimensions
		{
			get { return _dims; }
			set
			{
				if (_dims != value)
				{
					Platform.CheckArgumentRange(value.Height, 0, MaxDimensions.Height, "value");
					Platform.CheckArgumentRange(value.Width, 0, MaxDimensions.Width, "value");

					Size old = _dims;
					_dims = value;
					NotifyDimensionsChanged(old, value);
				}
			}
		}

		/// <summary>
		/// Resets the currently selected dimensions to the default value of 0x0.
		/// </summary>
		public void ResetDimensions()
		{
			_dims = Size.Empty;
		}

		/// <summary>
		/// Determines if the currently selected dimensions are the default values.
		/// </summary>
		/// <returns>False if it is the default, True otherwise.</returns>
		public bool ShouldSerializeDimensions()
		{
			return !(_dims == Size.Empty);
		}

		/// <summary>
		/// Fired when the <see cref="HotTrackingDimensions"/> property changes.
		/// </summary>
		[Description("Fired when the hot-tracked dimensions change.")]
		[Category("Property Changed")]
		public event EventHandler HotTrackingDimensionsChanged
		{
			add { _hotDimensionsChanged += value; }
			remove { _hotDimensionsChanged -= value; }
		}

		/// <summary>
		/// Notifies listeners that the <see cref="HotTrackingDimensions"/> property has changed.
		/// </summary>
		/// <param name="newDims">The new value of the property.</param>
		/// <param name="oldDims">The old value of the property.</param>
		protected virtual void NotifyHotTrackingDimensionsChanged(Size newDims, Size oldDims)
		{
			base.Invalidate(GetInvalidRegion(newDims, oldDims));

			if (_hotDimensionsChanged != null)
				_hotDimensionsChanged(this, new EventArgs());
		}

		/// <summary>
		/// Gets the current dimensions that the cursor is hovering over.
		/// </summary>
		/// <remarks>
		/// If the cursor is not over the control, this property is 0x0 (i.e. <see cref="Size.Empty"/>).
		/// </remarks>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Size HotTrackingDimensions
		{
			get { return _hotDims; }
			private set
			{
				if (_hotDims != value)
				{
					Size old = _hotDims;
					_hotDims = value;
					NotifyHotTrackingDimensionsChanged(old, value);
				}
			}
		}

		#endregion

		#region Misc/Helper Properties

		/// <summary>
		/// Gets or sets the maximum number of rows shown on the control.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int MaxRows
		{
			get { return this.MaxDimensions.Height; }
			set { this.MaxDimensions = new Size(value, this.MaxColumns); }
		}

		/// <summary>
		/// Gets or sets the maximum number of columns shown on the control.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int MaxColumns
		{
			get { return this.MaxDimensions.Width; }
			set { this.MaxDimensions = new Size(this.MaxRows, value); }
		}

		/// <summary>
		/// Gets the row that the cursor is hovering over.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int HotTrackingRows
		{
			get { return this.HotTrackingDimensions.Height; }
		}

		/// <summary>
		/// Gets the column that the cursor is hovering over.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int HotTrackingColumns
		{
			get { return this.HotTrackingDimensions.Width; }
		}

		/// <summary>
		/// Gets or sets the currently selected number of rows.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int Rows
		{
			get { return this.Dimensions.Height; }
			set { this.Dimensions = new Size(value, this.Columns); }
		}

		/// <summary>
		/// Gets or sets the current selected number of columns.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int Columns
		{
			get { return this.Dimensions.Width; }
			set { this.Dimensions = new Size(this.Rows, value); }
		}

		#endregion

		#region System.Windows.Forms.Control Overrides

		/// <summary>
		/// Gets the default size of the control.
		/// </summary>
		protected override Size DefaultSize
		{
			get { return GetDefaultSize(); }
		}

		/// <summary>
		/// This control is always drawn double buffered.
		/// </summary>
		protected override bool DoubleBuffered
		{
			get { return false; }
			set { }
		}

		/// <summary>
		/// This property is not relevant for this control.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Color ForeColor
		{
			get { return base.ForeColor; }
			set { }
		}

		/// <summary>
		/// This property is not relevant for this control.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Font Font
		{
			get { return base.Font; }
			set { }
		}

		/// <summary>
		/// This property is not relevant for this control.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override RightToLeft RightToLeft
		{
			get { return base.RightToLeft; }
			set { }
		}

		/// <summary>
		/// This property is not relevant for this control.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text
		{
			get { return string.Empty; }
			set { }
		}

		/// <summary>
		/// Retrieves the size of a rectangular area into which a control can be fitted.
		/// </summary>
		/// <returns>An ordered pair of type <see cref="T:System.Drawing.Size"></see> representing the width and height of a rectangle.</returns>
		/// <param name="proposedSize">The custom-sized area for a control.</param>
		public override Size GetPreferredSize(Size proposedSize)
		{
			return this.DefaultSize;
		}

		#endregion

		#region Event Overrides

		///<summary>
		///Raises the <see cref="E:System.Windows.Forms.Control.MouseMove"></see> event.
		///</summary>
		///
		///<param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data. </param>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			this.HotTrackingDimensions = new Size(GetCellAtPoint(e.X, e.Y));
			base.OnMouseMove(e);
		}

		///<summary>
		///Raises the <see cref="E:System.Windows.Forms.Control.MouseLeave"></see> event.
		///</summary>
		///
		///<param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data. </param>
		protected override void OnMouseLeave(EventArgs e)
		{
			this.HotTrackingDimensions = Size.Empty;
			base.OnMouseLeave(e);
		}

		///<summary>
		///Raises the <see cref="E:System.Windows.Forms.Control.MouseClick"></see> event.
		///</summary>
		///
		///<param name="e">An <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data. </param>
		protected override void OnMouseClick(MouseEventArgs e)
		{
			this.Dimensions = new Size(GetCellAtPoint(e.X, e.Y));
			base.OnMouseClick(e);
			NotifyDimensionsSelected();
		}

		///<summary>
		///Raises the <see cref="E:System.Windows.Forms.Control.Paint"></see> event.
		///</summary>
		///
		///<param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"></see> that contains the event data. </param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			using (Bitmap bmp = new Bitmap(base.Width, base.Height))
			{
				using (Graphics g = Graphics.FromImage(bmp))
				{
					// paint the background if it's not transparent
					if (base.BackColor != Color.Transparent)
					{
						SolidBrush bkgBrush = new SolidBrush(base.BackColor);
						g.FillRectangle(bkgBrush, 0, 0, bmp.Width, bmp.Height);
						bkgBrush.Dispose();
					}

					// paint the last selected dimensions if it's not transparent
					if (this.ActiveColor != Color.Transparent)
					{
						SolidBrush selBrush = new SolidBrush(this.ActiveColor);
						g.FillRectangle(selBrush, 0, 0, GetColumnBoundary(_dims.Width), GetRowBoundary(_dims.Height));
						selBrush.Dispose();
					}

					// paint the hot-tracked dimensions if it's not transparent
					if (this.HighlightColor != Color.Transparent)
					{
						SolidBrush hotBrush = new SolidBrush(this.HighlightColor);
						g.FillRectangle(hotBrush, 0, 0, GetColumnBoundary(_hotDims.Width), GetRowBoundary(_hotDims.Height));
						hotBrush.Dispose();
					}

					// paint the cell borders if it's not transparent
					if (this.CellBorderColor != Color.Transparent)
					{
						Pen bdrPen = new Pen(this.CellBorderColor, this.CellBorderWidth);
						for (int n = 0; n <= _maxDims.Height; n++)
						{
							int y = GetRowBoundary(n);
							g.DrawLine(bdrPen, 0, y, bmp.Width - 1, y);
						}
						for (int n = 0; n <= _maxDims.Width; n++)
						{
							int x = GetColumnBoundary(n);
							g.DrawLine(bdrPen, x, 0, x, bmp.Height - 1);
						}
						bdrPen.Dispose();
					}
				}

				// draw the invalid region to the control surface
				e.Graphics.DrawImageUnscaledAndClipped(bmp, e.ClipRectangle);
			}
		}

		///<summary>
		///Raises the <see cref="E:System.Windows.Forms.Control.Resize"></see> event.
		///</summary>
		///
		///<param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data. </param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			base.Invalidate();
		}

		#endregion

		#region Helper Methods

		private Size GetDefaultSize()
		{
			const int DEFAULT_CELL_LENGTH = 48;
			return new Size(DEFAULT_CELL_LENGTH*MaxDimensions.Width, DEFAULT_CELL_LENGTH*MaxDimensions.Height);
		}

		/// <summary>
		/// Gets the region invalidated by the changing of a dimension from one value to another.
		/// </summary>
		/// <param name="newSize">The new dimensions.</param>
		/// <param name="oldSize">The old dimensions.</param>
		/// <returns>The invalid region to repaint.</returns>
		protected Region GetInvalidRegion(Size newSize, Size oldSize)
		{
			Region r = new Region(new Rectangle(0, 0, GetColumnBoundary(oldSize.Width), GetRowBoundary(oldSize.Height)));
			r.Union(new Rectangle(0, 0, GetColumnBoundary(newSize.Width), GetRowBoundary(newSize.Height)));
			return r;
		}

		/// <summary>
		/// Computes the coordinates of the cell to which the cursor coordinates point.
		/// </summary>
		/// <param name="x">The X position of the cursor in client coordinates.</param>
		/// <param name="y">The Y position of the cursor in client coordinates.</param>
		/// <returns>The coordinates of the cell.</returns>
		protected Point GetCellAtPoint(int x, int y)
		{
			int rows = Math.Min(MaxDimensions.Height, 1 + (int) (1.0*MaxDimensions.Height*y/base.Height));
			int cols = Math.Min(MaxDimensions.Width, 1 + (int) (1.0*MaxDimensions.Width*x/base.Width));
			return new Point(cols, rows);
		}

		/// <summary>
		/// Computes the X coordinate of the ith column border.
		/// </summary>
		/// <param name="col">The index of the column border.</param>
		/// <returns>The X coordinate in client coordinates.</returns>
		protected int GetColumnBoundary(int col)
		{
			return Math.Min(base.Width - 1, (int) (1.0*base.Width*col/_maxDims.Width));
		}

		/// <summary>
		/// Computes the Y coordinate of the ith row border.
		/// </summary>
		/// <param name="row">The index of the row border.</param>
		/// <returns>The Y coordinate in client coordinates.</returns>
		protected int GetRowBoundary(int row)
		{
			return Math.Min(base.Height - 1, (int) (1.0*base.Height*row/_maxDims.Height));
		}

		#endregion
	}

	#region TableDimensionsEventArgs/Handler

	/// <summary>
	/// Represents the method that will handle the <see cref="TableDimensionsPicker.DimensionsSelected"/> event.
	/// </summary>
	/// <param name="sender">The <see cref="TableDimensionsPicker"/> that fired the event.</param>
	/// <param name="e">A <see cref="TableDimensionsEventArgs"/> that contains event data.</param>
	public delegate void TableDimensionsEventHandler(object sender, TableDimensionsEventArgs e);

	/// <summary>
	/// Provides data for the <see cref="TableDimensionsPicker.DimensionsSelected"/> event.
	/// </summary>
	public class TableDimensionsEventArgs : EventArgs
	{
		private Size _dimensions;

		/// <summary>
		/// Constructs a <see cref="TableDimensionsEventArgs"/>.
		/// </summary>
		/// <param name="dimensions">The selected dimensions.</param>
		public TableDimensionsEventArgs(Size dimensions)
		{
			_dimensions = dimensions;
		}

		/// <summary>
		/// Gets the selected dimensions.
		/// </summary>
		public Size Dimensions
		{
			get { return _dimensions; }
		}

		/// <summary>
		/// Gets the selected number of rows (height of the selected dimensions).
		/// </summary>
		public int Rows
		{
			get { return _dimensions.Height; }
		}

		/// <summary>
		/// Gets the selected number of columns (width of the selected dimensions).
		/// </summary>
		public int Columns
		{
			get { return _dimensions.Width; }
		}
	}

	#endregion
}