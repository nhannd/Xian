using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Windows.Forms.Design;

namespace ClearCanvas.Controls.WinForms
{
	[Designer(typeof(ParentControlDesigner))]
	public partial class OutlookSidebar : UserControl
	{
		private int _visibleCount = -1;
		private int _lastCount = -1;
		private int	_maxHeight;

		public OutlookSidebar()
		{
			// Set Defaults
			this.Dock = DockStyle.Fill;
			this.TabStop = false;

			// Initialize
			InitializeComponent();
		}

		public Panel UserPanel
		{
			get { return _userPanel; }
		}

		public StackStrip StackStrip
		{
			get { return _stackStrip; }
		}

		public string MainHeaderText
		{
			get { return _mainHeaderStripLabel.Text; }
			set { _mainHeaderStripLabel.Text = value; }
		}

		public string SubHeaderText
		{
			get { return _subHeaderStripLabel.Text; }
			set { _subHeaderStripLabel.Text = value; }
		}

		#region Setup
		private void StackBar_Load(object sender, EventArgs e)
		{
			// Setup
			this._stackStrip.ItemHeightChanged += new EventHandler(stackStrip1_ItemHeightChanged);

			// Add Overflow items
			AddOverflowItems();

			// Set Height
			InitializeSplitter();

			// Set parent padding
			this.Parent.Padding = new Padding(3, 3, 0, 3);
		}

		private void AddOverflowItems()
		{
			ToolStripButton item;
			Color			trans = Color.FromArgb(238,238,238);
			string			name;

			// Add items to overflow
			for (int idx = this._stackStrip.ItemCount - 1; idx >= 0; idx--)
			{
				name = this._stackStrip.Items[idx].Tag as string;

				if (null != name)
				{
					//item = new ToolStripButton(GetBitmapResource(name));
					item = new ToolStripButton(name);
					item.ImageTransparentColor = trans;
					item.Alignment = ToolStripItemAlignment.Right;
					item.Name = name;

					this._overflowStrip.Items.Add(item);
				}
			}
		}

		#endregion

		#region Event Handlers
		private void splitContainer1_Paint(object sender, PaintEventArgs e)
		{
			ProfessionalColorTable	pct = new ProfessionalColorTable();
			Rectangle				bounds = (sender as SplitContainer).SplitterRectangle;

			int			squares;
			int			maxSquares = 9;
			int			squareSize = 4;
			int			boxSize = 2;

			// Make sure we need to do work
			if ((bounds.Width > 0) && (bounds.Height > 0))
			{
				Graphics	g = e.Graphics;

				// Setup colors from the provided renderer
				Color		begin = pct.OverflowButtonGradientMiddle;
				Color		end = pct.OverflowButtonGradientEnd;

				// Make sure we need to do work
				using (Brush b = new LinearGradientBrush(bounds, begin, end, LinearGradientMode.Vertical))
				{
					g.FillRectangle(b, bounds);
				}

				// Calculate squares
				if ((bounds.Width > squareSize) && (bounds.Height > squareSize))
				{
					squares = Math.Min((bounds.Width / squareSize), maxSquares);

					// Calculate start
					int		start = (bounds.Width - (squares * squareSize)) / 2;
					int		Y = bounds.Y  + 1;

					// Get brushes
					Brush dark = new SolidBrush(pct.GripDark);
					Brush middle = new SolidBrush(pct.ToolStripBorder);
					Brush light = new SolidBrush(pct.ToolStripDropDownBackground);

					// Draw
					for (int idx = 0; idx < squares; idx++)
					{
						// Draw grips
						g.FillRectangle(dark, start, Y, boxSize, boxSize);
						g.FillRectangle(light, start + 1, Y+1, boxSize, boxSize);
						g.FillRectangle(middle, start + 1, Y+1, 1, 1);
						start += squareSize;
					}

					dark.Dispose();
					middle.Dispose();
					light.Dispose();
				}
			}
		}

		void stackStrip1_ItemHeightChanged(object sender, EventArgs e)
		{
			InitializeSplitter();
		}

		private void stackStripSplitter_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if ((_maxHeight > 0) && ((this._stackStripSplitter.Height - e.SplitY) > _maxHeight))
			{
				// Limit to max height
				this._stackStripSplitter.SplitterDistance = this._stackStripSplitter.Height - _maxHeight;

				// Set to max
				_visibleCount = this._stackStrip.ItemCount;
			}
			else if ((_visibleCount > 0) && (this._stackStrip.ItemCount > 0))
			{
				// Make sure overflow is correct
				_visibleCount = (this._stackStrip.Height / this._stackStrip.ItemHeight);

				// See if this changed
				if (_lastCount != _visibleCount)
				{
					int		count = this._overflowStrip.Items.Count;

					// Update overflow items
					for (int idx = 1; idx < count; idx++)
					{
						this._overflowStrip.Items[idx].Visible = (idx < (count - _visibleCount));
					}

					// Update last
					_lastCount = _visibleCount;
				}
			}
		}
		#endregion

		#region Private API
		private void InitializeSplitter()
		{
			// Set slider increment
			if (this._stackStrip.ItemHeight > 0)
			{
				this._stackStripSplitter.SplitterIncrement = this._stackStrip.ItemHeight;

				// Find visible count
				if (_visibleCount < 0)
				{
					_visibleCount = this._stackStrip.InitialDisplayCount;
				}

				// Setup BaseStackStrip
				this._overflowStrip.Height = this._stackStrip.ItemHeight;

				// Set splitter distance
				int min = this._stackStrip.ItemHeight + this._overflowStrip.Height;
				int distance = this._stackStripSplitter.Height - this._stackStripSplitter.SplitterWidth - ((_visibleCount * this._stackStrip.ItemHeight) + this._overflowStrip.Height);

				// Set Max
				_maxHeight = (this._stackStrip.ItemHeight * this._stackStrip.ItemCount) + this._overflowStrip.Height + this._stackStripSplitter.SplitterWidth;

				// In case it's sized too small on startup
				if (distance < 0)
				{
					distance = min;
				}

				// Set Min/Max
				this._stackStripSplitter.SplitterDistance = distance;
				this._stackStripSplitter.Panel1MinSize = min;
			}
		}
		#endregion
	}
}
