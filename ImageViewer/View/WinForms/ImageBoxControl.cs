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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="TileComponent"/>
    /// </summary>
    public partial class ImageBoxControl : UserControl
    {
        private ImageBox _imageBox;
		private Rectangle _parentRectangle;
		private bool _imageScrollerVisible;

        /// <summary>
        /// Constructor
        /// </summary>
		internal ImageBoxControl(ImageBox imageBox, Rectangle parentRectangle)
        {
			_imageBox = imageBox;
			this.ParentRectangle = parentRectangle;

			InitializeComponent();

			_imageScrollerVisible = _imageScroller.Visible;
			_imageScroller.MouseDown += ImageScrollerClicked;
			_imageScroller.ValueChanged += ImageScrollerValueChanged;

			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.BackColor = Color.Black;
			this.Dock = DockStyle.None;
			this.Anchor = AnchorStyles.None;

			_imageBox.Drawing += OnDrawing;
			_imageBox.SelectionChanged += OnImageBoxSelectionChanged;
			_imageBox.LayoutCompleted += OnLayoutCompleted;
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

    	private IEnumerable<TileControl> TileControls
    	{
    		get
    		{
    			foreach (Control control in this.Controls)
    			{
    				if (control is TileControl)
    					yield return (TileControl) control;
    			}
    		}
    	}

		private TileControl GetTileControl(ITile tile)
		{
			foreach (TileControl tileControl in TileControls)
			{
				if (tileControl.Tile == tile)
					return tileControl;
			}

			return null;
		}

		internal void Draw()
		{
			foreach (TileControl control in this.TileControls)
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

			this.SuspendLayout();

			SetTileParentImageBoxRectangles(false);

			this.ResumeLayout(false);

			if (ImageScrollerVisible)
			{
				_imageScroller.Location = new Point(this.Width - _imageScroller.Width, 0);
				_imageScroller.Size = new Size(_imageScroller.Width, this.Height);
			}

			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
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

		private void OnTileControlDrawing(object sender, EventArgs e)
		{
			//Perfectly efficient when there's only 1 tile ... a little less so when there's more than 1.
			//However, doing this makes sure the scroll bar is *always* up to date, even for things like sorting of images,
			//which currently doesn't actually fire any events.
			UpdateImageScroller();
		}

		private void DisposeControls(IEnumerable<TileControl> controls)
		{
			foreach (TileControl control in controls)
			{
				this.Controls.Remove(control);
				control.Tile.SelectionChanged -= OnTileSelectionChanged;
				control.Drawing -= OnTileControlDrawing;
				control.Dispose();
			}
		}

		private void PerformDispose()
		{
			if (_imageBox != null)
			{
				_imageBox.Drawing -= OnDrawing;
				_imageBox.SelectionChanged -= OnImageBoxSelectionChanged;
				_imageBox.LayoutCompleted -= OnLayoutCompleted;
				_imageBox = null;
			}

			if (_imageScroller != null)
			{
				_imageScroller.ValueChanged -= ImageScrollerValueChanged;
				_imageScroller.MouseDown -= ImageScrollerClicked;
			}

			DisposeControls(new List<TileControl>(this.TileControls));
		}

    	private void OnLayoutCompleted(object sender, EventArgs e)
		{
			List<TileControl> oldControls = new List<TileControl>(this.TileControls);

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
				foreach (TileControl control in this.TileControls)
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

			// keep the image scroller at the forefront
			_imageScroller.BringToFront();

			this.ResumeLayout(false);
		}

		private void AddTileControl(Tile tile)
		{
			TileView view = ViewFactory.CreateAssociatedView(typeof(Tile)) as TileView;

			view.Tile = tile;
			view.ParentRectangle = this.AvailableClientRectangle;
			view.ParentImageBoxInsetWidth = ImageBox.InsetWidth;

			TileControl control = view.GuiElement as TileControl;
			control.Tile.SelectionChanged += OnTileSelectionChanged;
			control.Drawing += OnTileControlDrawing;

			control.SuspendLayout();
			this.Controls.Add(control);
			control.ResumeLayout(false);
		}

		private void SetTileParentImageBoxRectangles(bool suppressDraw)
		{
			foreach (TileControl control in this.TileControls)
				control.SetParentImageBoxRectangle(this.AvailableClientRectangle, ImageBox.InsetWidth, suppressDraw);
		}

		#endregion

		#region ImageBoxControl Scroll-stacking Support

    	private bool ImageScrollerVisible
    	{
			get { return _imageScrollerVisible; }
			set
			{
				//when we switch workspaces, _imageScroller.Visible changes to false.  But, for our calculations,
				//we don't care about whether or not it's really visible, just whether or not it should be visible.
				if (_imageScrollerVisible != value)
					_imageScroller.Visible = _imageScrollerVisible = value;
			}
    	}

		/// <summary>
		/// Gets the <see cref="Control.ClientRectangle"/> of this control, less any area dedicated to the ImageBoxControl scrollbar.
		/// </summary>
    	private Rectangle AvailableClientRectangle
    	{
    		get
    		{
				Rectangle clientRectangle = this.ClientRectangle;
				if (ImageScrollerVisible)
					clientRectangle.Width -= _imageScroller.Width - ImageBox.InsetWidth;
    			return clientRectangle;
    		}
    	}

    	private void ImageScrollerClicked(object sender, EventArgs e)
    	{
    		if(_imageBox != null)
    		{
				if (_imageBox.Tiles.Count > 0)
				{
					if (!_imageBox.Selected)
						_imageBox.SelectDefaultTile();

					TileControl tileControl = GetTileControl(_imageBox.SelectedTile);
					if (tileControl != null)
						tileControl.Focus();
				}
    		}
    	}

    	private void ImageScrollerValueChanged(object sender, TrackSlider.ValueChangedEventArgs e)
    	{
			if (e.UserAction == TrackSlider.UserAction.None)
			{
				//The change has occurred due to external forces ... so, drawing is up to the external force!
				_imageScroller.Update();
			}
			else
			{
				//we only draw the image box when focused because the user is actually dragging the scrollbar!
				_imageBox.TopLeftPresentationImageIndex = _imageScroller.Value;

				// make sure the scrollbar draws immediately!
				_imageScroller.Update();

				// this ordering of draw makes it look smoother for some reason.
				_imageBox.Draw();
			}
    	}

		private void UpdateImageScroller()
		{
			//This method can be called repeatedly and will essentially be a no-op if nothing needs to change.
			//In tiled mode, it could be a little inefficient to call repeatedly, but it's the lesser of the evils.
			//Otherwise, we're subscribing to a multitude of events and updating different things at different times.
			//Not to mention, that doesn't cover every case, like sorting images.  It's nothing compared to
			//the cost of updating the scroll control itself, anyway.

			CodeClock clock = new CodeClock();
			clock.Start();

			bool visibleBefore = ImageScrollerVisible;
			bool visibleNow = false;

			IDisplaySet displaySet = _imageBox.DisplaySet;
    		if (displaySet != null)
    		{
				int tileCount = _imageBox.Tiles.Count;
				int maximum = Math.Max(0, displaySet.PresentationImages.Count - tileCount);
				if (maximum > 0)
				{
					visibleNow = true;

					int topLeftIndex = _imageBox.TopLeftPresentationImageIndex;
					_imageScroller.SetValueAndRange(topLeftIndex, 0, maximum);
					_imageScroller.Increment = Math.Max(1, tileCount);
					_imageScroller.Value = topLeftIndex;
				}
    		}

			if (visibleBefore != visibleNow)
			{
				ImageScrollerVisible = visibleNow;
				//UpdateImageScroller is only called right before a Tile is drawn, so we suppress
				//the Tile drawing as a result of a size change here.
				SetTileParentImageBoxRectangles(true);
			}

			clock.Stop();
			//Trace.WriteLine(String.Format("UpdateScroller: {0}", clock));
		}

		#endregion
	}
}
