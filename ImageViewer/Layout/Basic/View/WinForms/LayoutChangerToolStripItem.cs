using System;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
	/// <summary>
	/// A custom <see cref="ToolStripItem"/> control that hosts a <see cref="TableDimensionsPicker"/>.
	/// </summary>
	internal class LayoutChangerToolStripItem : ToolStripControlHost
	{
		private readonly TableDimensionsPicker _picker;
		private readonly LayoutChangerAction _action;
		private readonly Label _label;
		private readonly Panel _panel;
		private readonly Size _defaultSize;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="action">The action to which this view is bound.</param>
		public LayoutChangerToolStripItem(LayoutChangerAction action) : base(new Panel())
		{
			_action = action;
			_picker = new TableDimensionsPicker(_action.MaxRows, _action.MaxColumns);
			_picker.BackColor = Color.Transparent;
			_picker.DimensionsSelected += OnDimensionsSelected;
			_picker.HotTrackingDimensionsChanged += OnHotTrackingDimensionsChanged;
			_picker.Dock = DockStyle.Fill;
			_label = new Label();
			_label.AutoSize = false;
			_label.BackColor = Color.Transparent;
			_label.Click += OnCancel;
			_label.Dock = DockStyle.Bottom;
			_label.Size = new Size(base.Width, 21);
			_label.Text = SR.LabelCancel;
			_label.TextAlign = ContentAlignment.MiddleCenter;
			_panel = (Panel) base.Control;
			_panel.Controls.Add(_picker);
			_panel.Controls.Add(_label);

			base.AutoSize = false;
			base.BackColor = Color.Transparent;

			// this computes a nice size for the control that fits in the dropdown and has aethetically pleasant square cells
			base.Size = _defaultSize = new Size(base.Width, (int) (base.Width*1.0*_action.MaxRows/_action.MaxColumns) + _label.Height);
		}

		protected override void Dispose(bool disposing)
		{
			_picker.DimensionsSelected -= OnDimensionsSelected;
			_picker.HotTrackingDimensionsChanged -= OnHotTrackingDimensionsChanged;
			_label.Click -= OnCancel;

			base.Dispose(disposing);
		}

		protected override Size DefaultSize
		{
			get { return _defaultSize; }
		}

		public override Size GetPreferredSize(Size constrainingSize)
		{
			return _defaultSize;
		}

		protected override bool DismissWhenClicked
		{
			get { return true; }
		}

		/// <summary>
		/// Fired when the hot-tracked cell changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnHotTrackingDimensionsChanged(object sender, EventArgs e)
		{
			if (_picker.HotTrackingDimensions.IsEmpty)
				_label.Text = SR.LabelCancel;
			else
				_label.Text = string.Format(SR.FormatRowsColumns, _picker.HotTrackingDimensions.Height, _picker.HotTrackingDimensions.Width);
		}

		/// <summary>
		/// Fired when the user selects a layout.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDimensionsSelected(object sender, TableDimensionsEventArgs e)
		{
			_action.SetLayout(e.Rows, e.Columns);
			CloseDropDown();
		}

		/// <summary>
		/// Fired when the user clicks on the cancel label bar.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCancel(object sender, EventArgs e)
		{
			CloseDropDown();
		}

		/// <summary>
		/// Closes the dropdown, if this ToolStripItem is on a dropdown.
		/// </summary>
		private void CloseDropDown()
		{
			if (base.IsOnDropDown)
			{
				base.PerformClick();
			}
		}
	}
}