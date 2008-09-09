using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms {
	[DefaultEvent("DimensionsSelected")]
	[DefaultProperty("Dimensions")]
	public class TableDimensionsPicker : Control {
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

		public TableDimensionsPicker() : this(DEFAULT_DIMS) { }
		public TableDimensionsPicker(int maxRows, int maxCols) : this(new Size(maxCols, maxRows)) { }

		public TableDimensionsPicker(Size maxDimensions) {
			_maxDims = maxDimensions;
			_hotDims = _dims = Size.Empty;
			ResetCellBorderWidth();
			ResetCellBorderColor();
			ResetActiveColor();
			ResetHighlightColor();
			base.Size = GetDefaultSize();
			base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		}

		#region Display Option Properties (and their Reset methods)

		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(1)]
		[Description("The pixel width of the cell borders.")]
		public int CellBorderWidth {
			get { return _cellBorderWidth; }
			set {
				if (_cellBorderWidth != value) {
					_cellBorderWidth = value;
					base.Invalidate();
				}
			}
		}

		public void ResetCellBorderWidth() {
			_cellBorderWidth = 1;
		}

		[Bindable(true)]
		[Category("Appearance")]
		[Description("The color of the cell borders.")]
		public Color CellBorderColor {
			get { return _cellBorderColor; }
			set {
				if (_cellBorderColor != value) {
					_cellBorderColor = value;
					base.Invalidate();
				}
			}
		}

		public void ResetCellBorderColor() {
			_cellBorderColor = Color.Black;
		}

		public bool ShouldSerializeCellBorderColor() {
			return !(this.CellBorderColor == Color.Black);
		}

		[Bindable(true)]
		[Category("Appearance")]
		[Description("The hot-tracking color that will highlight the dimensions over which the cursor is hovering.")]
		public Color HighlightColor {
			get { return _highlightColor; }
			set {
				if (_highlightColor != value) {
					_highlightColor = value;
					base.Invalidate();
				}
			}
		}

		public void ResetHighlightColor() {
			_highlightColor = SystemColors.HotTrack;
		}

		public bool ShouldSerializeHighlightColor() {
			return !(this.HighlightColor == SystemColors.HotTrack);
		}

		[Bindable(true)]
		[Category("Appearance")]
		[Description("The color that will highlight the currently selected dimensions.")]
		public Color ActiveColor {
			get { return _activeColor; }
			set {
				if (_activeColor != value) {
					_activeColor = value;
					base.Invalidate();
				}
			}
		}

		public void ResetActiveColor() {
			_activeColor = Color.Transparent;
		}

		public bool ShouldSerializeActiveColor() {
			return !(this.ActiveColor == Color.Transparent);
		}

		#endregion

		#region Designer Properties (and their Events and Reset methods)

		/// <summary>
		/// Fired when the <see cref="MaxDimensions"/> property changes.
		/// </summary>
		[Description("Fired when the maximum dimensions change.")]
		[Category("Property Changed")]
		public event EventHandler MaxDimensionsChanged {
			add { _maxDimensionsChanged += value; }
			remove { _maxDimensionsChanged -= value; }
		}

		protected virtual void NotifyMaxDimensionsChanged() {
			_hotDims = Size.Empty;
			base.Invalidate();

			if (_maxDimensionsChanged != null)
				_maxDimensionsChanged(this, new EventArgs());
		}

		[Bindable(true)]
		[Category("Behavior")]
		[Description("The maximum dimensions to show on the control.")]
		public Size MaxDimensions {
			get { return _maxDims; }
			set {
				if (_maxDims != value) {
					_maxDims = value;
					NotifyMaxDimensionsChanged();
				}
			}
		}

		public void ResetMaxDimensions() {
			_maxDims = DEFAULT_DIMS;
		}

		public bool ShouldSerializeMaxDimensions() {
			return !(_maxDims == DEFAULT_DIMS);
		}

		[Description("Fired when the user selects new dimensions.")]
		[Category("Action")]
		public event TableDimensionsEventHandler DimensionsSelected {
			add { _dimensionsSelected += value; }
			remove { _dimensionsSelected -= value; }
		}

		protected virtual void NotifyDimensionsSelected() {
			if (_dimensionsSelected != null)
				_dimensionsSelected(this, new TableDimensionsEventArgs(this.Dimensions));
		}

		[Description("Fired when the currently selected dimensions change.")]
		[Category("Property Changed")]
		public event EventHandler DimensionsChanged {
			add { _dimensionsChanged += value; }
			remove { _dimensionsChanged -= value; }
		}

		protected virtual void NotifyDimensionsChanged(Size newDims, Size oldDims) {
			base.Invalidate(GetInvalidRegion(newDims, oldDims));

			if (_dimensionsChanged != null)
				_dimensionsChanged(this, new EventArgs());
		}

		[Category("Behavior")]
		[Description("The currently selected dimensions.")]
		public Size Dimensions {
			get { return _dims; }
			set {
				if (_dims != value) {
					Platform.CheckArgumentRange(value.Height, 0, MaxDimensions.Height, "value");
					Platform.CheckArgumentRange(value.Width, 0, MaxDimensions.Width, "value");

					Size old = _dims;
					_dims = value;
					NotifyDimensionsChanged(old, value);
				}
			}
		}

		public void ResetDimensions() {
			_dims = Size.Empty;
		}

		public bool ShouldSerializeDimensions() {
			return !(_dims == Size.Empty);
		}

		/// <summary>
		/// Fired when the <see cref="HotTrackingDimensions"/> property changes.
		/// </summary>
		[Description("Fired when the hot-tracked dimensions change.")]
		[Category("Property Changed")]
		public event EventHandler HotTrackingDimensionsChanged {
			add { _hotDimensionsChanged += value; }
			remove { _hotDimensionsChanged -= value; }
		}

		protected virtual void NotifyHotTrackingDimensionsChanged(Size newDims, Size oldDims) {
			base.Invalidate(GetInvalidRegion(newDims, oldDims));

			if (_hotDimensionsChanged != null)
				_hotDimensionsChanged(this, new EventArgs());
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Size HotTrackingDimensions {
			get { return _hotDims; }
			private set {
				if (_hotDims != value) {
					Size old = _hotDims;
					_hotDims = value;
					NotifyHotTrackingDimensionsChanged(old, value);
				}
			}
		}

		#endregion

		#region Misc/Helper Properties

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int MaxRows {
			get { return this.MaxDimensions.Height; }
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int MaxColumns {
			get { return this.MaxDimensions.Width; }
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int Rows {
			get { return this.Dimensions.Height; }
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int Columns {
			get { return this.Dimensions.Width; }
		}

		#endregion

		#region System.Windows.Forms.Control Overrides

		protected override bool CanEnableIme {
			get { return false; }
		}

		protected override Size DefaultSize {
			get {
				return GetDefaultSize();
			}
		}

		protected override bool DoubleBuffered {
			get { return false; }
			set { }
		}

		/// <summary>
		/// This property is not relevant for this control.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Color ForeColor {
			get { return base.ForeColor; }
			set { }
		}

		/// <summary>
		/// This property is not relevant for this control.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Font Font {
			get { return base.Font; }
			set { }
		}

		/// <summary>
		/// This property is not relevant for this control.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override RightToLeft RightToLeft {
			get { return base.RightToLeft; }
			set { }
		}

		/// <summary>
		/// This property is not relevant for this control.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text {
			get { return string.Empty; }
			set { }
		}

		public override Size GetPreferredSize(Size proposedSize) {
			return this.DefaultSize;
		}

		#endregion

		#region Event Overrides

		protected override void OnMouseMove(MouseEventArgs e) {
			this.HotTrackingDimensions = new Size(GetCellAtPoint(e.X, e.Y));
			base.OnMouseMove(e);
		}

		protected override void OnMouseLeave(EventArgs e) {
			this.HotTrackingDimensions = Size.Empty;
			base.OnMouseLeave(e);
		}

		protected override void OnMouseClick(MouseEventArgs e) {
			this.Dimensions = new Size(GetCellAtPoint(e.X, e.Y));
			base.OnMouseClick(e);
			NotifyDimensionsSelected();
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);

			using (Bitmap bmp = new Bitmap(base.Width, base.Height)) {
				using (Graphics g = Graphics.FromImage(bmp)) {
					if (base.BackColor != Color.Transparent) {
						SolidBrush bkgBrush = new SolidBrush(base.BackColor);
						g.FillRectangle(bkgBrush, 0, 0, bmp.Width, bmp.Height);
						bkgBrush.Dispose();
					}

					if (this.ActiveColor != Color.Transparent) {
						SolidBrush selBrush = new SolidBrush(this.ActiveColor);
						g.FillRectangle(selBrush, 0, 0, GetColumnBoundary(_dims.Width), GetRowBoundary(_dims.Height));
						selBrush.Dispose();
					}

					if (this.HighlightColor != Color.Transparent) {
						SolidBrush hotBrush = new SolidBrush(this.HighlightColor);
						g.FillRectangle(hotBrush, 0, 0, GetColumnBoundary(_hotDims.Width), GetRowBoundary(_hotDims.Height));
						hotBrush.Dispose();
					}

					if (this.CellBorderColor != Color.Transparent) {
						Pen bdrPen = new Pen(this.CellBorderColor, this.CellBorderWidth);
						for (int n = 0; n <= _maxDims.Height; n++) {
							int y = GetRowBoundary(n);
							g.DrawLine(bdrPen, 0, y, bmp.Width - 1, y);
						}
						for (int n = 0; n <= _maxDims.Width; n++) {
							int x = GetColumnBoundary(n);
							g.DrawLine(bdrPen, x, 0, x, bmp.Height - 1);
						}
						bdrPen.Dispose();
					}
				}

				e.Graphics.DrawImageUnscaledAndClipped(bmp, e.ClipRectangle);
			}
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			base.Invalidate();
		}

		#endregion

		#region Helper Methods

		private Size GetDefaultSize() {
			const int DEFAULT_CELL_LENGTH = 48;
			return new Size(DEFAULT_CELL_LENGTH * MaxDimensions.Width, DEFAULT_CELL_LENGTH * MaxDimensions.Height);
		}

		protected Region GetInvalidRegion(Size newSize, Size oldSize) {
			Region r = new Region(new Rectangle(0, 0, GetColumnBoundary(oldSize.Width), GetRowBoundary(oldSize.Height)));
			r.Union(new Rectangle(0, 0, GetColumnBoundary(newSize.Width), GetRowBoundary(newSize.Height)));
			return r;
		}

		protected Point GetCellAtPoint(int x, int y) {
			int rows = Math.Min(MaxDimensions.Height, 1 + (int)(1.0 * MaxDimensions.Height * y / base.Height));
			int cols = Math.Min(MaxDimensions.Width, 1 + (int)(1.0 * MaxDimensions.Width * x / base.Width));
			return new Point(cols, rows);
		}

		protected int GetColumnBoundary(int col) {
			return Math.Min(base.Width - 1, (int)(1.0 * base.Width * col / _maxDims.Width));
		}

		protected int GetRowBoundary(int row) {
			return Math.Min(base.Height - 1, (int)(1.0 * base.Height * row / _maxDims.Height));
		}

		#endregion
	}

	#region TableDimensionsEventArgs/Handler

	public delegate void TableDimensionsEventHandler(object sender, TableDimensionsEventArgs e);

	public class TableDimensionsEventArgs : EventArgs {
		private Size _dimensions;

		public TableDimensionsEventArgs(Size dimensions) {
			_dimensions = dimensions;
		}

		public Size Dimensions {
			get { return _dimensions; }
		}

		public int Rows {
			get { return _dimensions.Height; }
		}

		public int Columns {
			get { return _dimensions.Width; }
		}
	}

	#endregion
}