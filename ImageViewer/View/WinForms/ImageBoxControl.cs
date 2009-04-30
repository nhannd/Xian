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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="TileComponent"/>
    /// </summary>
    public partial class ImageBoxControl : UserControl
    {
        private ImageBox _imageBox;
		private Rectangle _parentRectangle;

        /// <summary>
        /// Constructor
        /// </summary>
		internal ImageBoxControl(ImageBox imageBox, Rectangle parentRectangle)
        {
			_imageBox = imageBox;
			this.ParentRectangle = parentRectangle;

			InitializeComponent();

			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.BackColor = Color.Black;
			this.Dock = DockStyle.None;
			this.Anchor = AnchorStyles.None;

			_imageBox.Drawing += new EventHandler(OnDrawing);
			_imageBox.SelectionChanged += new EventHandler<ItemEventArgs<IImageBox>>(OnImageBoxSelectionChanged);
			_imageBox.LayoutCompleted += new EventHandler(OnLayoutCompleted);
        }

		internal ImageBox ImageBox
		{
			get { return _imageBox; }
		}

		internal Rectangle ParentRectangle
		{
			get { return _parentRectangle; }
			set 
			{
				_parentRectangle = value;

				int left = (int)(_imageBox.NormalizedRectangle.Left * _parentRectangle.Width);
				int top = (int)(_imageBox.NormalizedRectangle.Top * _parentRectangle.Height);
				int right = (int)(_imageBox.NormalizedRectangle.Right * _parentRectangle.Width);
				int bottom = (int)(_imageBox.NormalizedRectangle.Bottom * _parentRectangle.Height);

				this.SuspendLayout();

				this.Location = new Point(left, top);
				this.Size = new Size(right - left, bottom - top);

				this.ResumeLayout(false);
			}
		}

		internal void Draw()
		{
			//Trace.Write("ImageBoxControl.Draw\n");

			foreach (TileControl control in this.Controls)
				control.Draw();
			
			Invalidate();
		}

		#region Protected methods

		protected override void OnLoad(EventArgs e)
		{
			AddTileControls(_imageBox);

			base.OnLoad(e);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			//Trace.Write("ImageBoxControl.OnSizeChanged\n");

			this.SuspendLayout();

			foreach (TileControl control in this.Controls)
				control.SetParentImageBoxRectangle(this.ClientRectangle, ImageBox.InsetWidth);

			this.ResumeLayout(false);

			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			//Trace.Write("ImageBoxControl.OnPaint\n");

			e.Graphics.Clear(Color.Black);

			DrawImageBoxBorder(e);
			DrawTileBorders(e);

			base.OnPaint(e);
		}

		#endregion

		#region Private methods

		private void OnImageBoxSelectionChanged(object sender, ItemEventArgs<IImageBox> e)
		{
			Invalidate();
			Update();
		}

		private void OnTileSelectionChanged(object sender, ItemEventArgs<ITile> e)
		{
			Invalidate();
			Update();
		}

		private void OnDrawing(object sender, EventArgs e)
		{
			Draw();
		}

		private List<TileControl> GetTileControls()
		{
			List<TileControl> controls = new List<TileControl>();

			foreach (TileControl control in this.Controls)
				controls.Add(control);

			return controls;
		}

		private void DisposeControls(IEnumerable<TileControl> controls)
		{
			foreach (TileControl control in controls)
			{
				this.Controls.Remove(control);
				control.Tile.SelectionChanged -= new EventHandler<ItemEventArgs<ITile>>(OnTileSelectionChanged);
				control.Dispose();
			}
		}

		private void DisposeControls()
		{
			DisposeControls(GetTileControls());
		}

    	private void OnLayoutCompleted(object sender, EventArgs e)
		{
    		List<TileControl> oldControls = GetTileControls();

			this.SuspendLayout();

			// We add all the new tile controls to the image box control first,
			// then we remove the old ones. Removing them first then adding them
			// results in flickering, which we don't want.
			AddTileControls(_imageBox);

    		DisposeControls(oldControls);

			this.ResumeLayout(true);
		}

		private void DrawImageBoxBorder(PaintEventArgs e)
		{
			// Draw image box border
			DrawBorder(
				e.Graphics,
				this.ClientRectangle,
				_imageBox.BorderColor,
				ImageBox.BorderWidth,
				ImageBox.InsetWidth);
		}

		private void DrawTileBorders(PaintEventArgs e)
		{
			// Draw tile border, provided there's more than one tile
			if (this.Controls.Count > 1)
			{
				foreach (TileControl control in this.Controls)
				{
					Rectangle rectangle = GetTileRectangle(control);

					DrawBorder(
						e.Graphics,
						rectangle,
						control.Tile.BorderColor,
						Tile.BorderWidth,
						Tile.InsetWidth);
				}
			}
		}

		private Rectangle GetTileRectangle(TileControl control)
		{
			Rectangle tileRectangle = new Rectangle(control.Location, control.Size);

			Rectangle borderRectangle =
				Rectangle.Inflate(
					tileRectangle,
					Tile.InsetWidth,
					Tile.InsetWidth);

			return borderRectangle;
		}

		private void DrawBorder(System.Drawing.Graphics graphics, Rectangle rectangle, Color borderColor, int borderWidth, int insetWidth)
		{
			int offset = insetWidth / 2;
			Rectangle borderRectangle = Rectangle.Inflate(rectangle, -offset, -offset);

			using (Pen pen = new Pen(borderColor, borderWidth))
			{
				graphics.DrawRectangle(pen, borderRectangle);
			}
		}

		private void AddTileControls(ImageBox imageBox)
		{
			this.SuspendLayout();

			foreach (Tile tile in imageBox.Tiles)
				AddTileControl(tile);

			this.ResumeLayout(false);
		}

		private void AddTileControl(Tile tile)
		{
			TileView view = ViewFactory.CreateAssociatedView(typeof(Tile)) as TileView;

			view.Tile = tile;
			view.ParentRectangle = this.ClientRectangle;
			view.ParentImageBoxInsetWidth = ImageBox.InsetWidth;

			TileControl control = view.GuiElement as TileControl;
			control.Tile.SelectionChanged += new EventHandler<ItemEventArgs<ITile>>(OnTileSelectionChanged);

			control.SuspendLayout();
			this.Controls.Add(control);
			control.ResumeLayout(false);
		}

		#endregion
	}
}
