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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;

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
			InitializeImageScroller();

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

		internal void Draw()
		{
			//Trace.Write("ImageBoxControl.Draw\n");

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

			//Trace.Write("ImageBoxControl.OnSizeChanged\n");

			this.SuspendLayout();

			foreach (TileControl control in this.TileControls)
				control.SetParentImageBoxRectangle(this.AvailableClientRectangle, ImageBox.InsetWidth);

			this.ResumeLayout(false);

			if (_imageScroller != null && _imageScroller.Visible)
			{
				_imageScroller.Location = new Point(this.Width - _imageScroller.Width, 0);
				_imageScroller.Size = new Size(_imageScroller.Width, this.Height);
			}

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

		private void DisposeControls(IEnumerable<TileControl> controls)
		{
			foreach (TileControl control in controls)
			{
				this.Controls.Remove(control);
				control.Tile.SelectionChanged -= OnTileSelectionChanged;
				control.Dispose();
			}
		}

		private void PerformDispose()
		{
			TerminateImageScroller();

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

			control.SuspendLayout();
			this.Controls.Add(control);
			control.ResumeLayout(false);
		}

		private void ImageScroller_VisibleChanged(object sender, EventArgs e)
		{
			foreach (TileControl control in this.TileControls)
				control.SetParentImageBoxRectangle(this.AvailableClientRectangle, ImageBox.InsetWidth);
		}

		#endregion

		#region ImageBoxControl Scroll-stacking Support

		private static readonly Dictionary<IImageBox, ImageBoxControl> _imageBoxControlIndex = new Dictionary<IImageBox, ImageBoxControl>();
    	private bool _isScrollToolInitiatedEvent = false;
    	private bool _isScrollBarInitiatedEvent = false;

		/// <summary>
		/// Gets the <see cref="Control.ClientRectangle"/> of this control, less any area dedicated to the ImageBoxControl scrollbar.
		/// </summary>
    	private Rectangle AvailableClientRectangle
    	{
    		get
    		{
				Rectangle clientRectangle = this.ClientRectangle;
				if (_imageScroller.Visible)
					clientRectangle.Width -= _imageScroller.Width - ImageBox.InsetWidth;
    			return clientRectangle;
    		}
    	}

		/// <summary>
		/// Initializes ImageBoxControl scroll-stacking support. Call on control construction.
		/// </summary>
    	private void InitializeImageScroller()
    	{
    		_imageBoxControlIndex.Add(this.ImageBox, this);

    		this.ImageBox.DisplaySetChanged += ImageBox_DisplaySetChanged;
    		this.ImageBox.LayoutCompleted += ImageBox_LayoutCompleted;

			if (this.ImageBox.DisplaySet != null)
				UpdateScrollerRange(this.ImageBox.DisplaySet);

			_imageScroller.GotFocus += ImageScroller_GotFocus;
    	}

		/// <summary>
		/// Terminates ImageBoxControl scroll-stacking support. Call on control disposal.
		/// </summary>
    	private void TerminateImageScroller()
    	{
			_imageScroller.GotFocus -= ImageScroller_GotFocus;

    		this.ImageBox.LayoutCompleted -= ImageBox_LayoutCompleted;
    		this.ImageBox.DisplaySetChanged -= ImageBox_DisplaySetChanged;

    		_imageBoxControlIndex.Remove(this.ImageBox);
    	}

    	private void ImageBox_LayoutCompleted(object sender, EventArgs e)
    	{
    		UpdateScrollerRange(this.ImageBox.DisplaySet);
    	}

    	private void ImageScroller_GotFocus(object sender, EventArgs e)
    	{
    		if(this.ImageBox != null)
    		{
				if (!this.ImageBox.Selected && this.ImageBox.Tiles.Count > 0)
    				this.ImageBox.Tiles[0].Select();
    		}
    	}

    	private void ImageBox_DisplaySetChanged(object sender, DisplaySetChangedEventArgs e)
    	{
			if (e.OldDisplaySet != null)
			{
				e.OldDisplaySet.PresentationImages.ItemAdded -= OnCurrentDisplaySetPresentationImagesChanged;
				e.OldDisplaySet.PresentationImages.ItemChanged -= OnCurrentDisplaySetPresentationImagesChanged;
				e.OldDisplaySet.PresentationImages.ItemRemoved -= OnCurrentDisplaySetPresentationImagesChanged;
			}

    		UpdateScrollerRange(e.NewDisplaySet);

			if (e.NewDisplaySet != null)
			{
				e.NewDisplaySet.PresentationImages.ItemAdded += OnCurrentDisplaySetPresentationImagesChanged;
				e.NewDisplaySet.PresentationImages.ItemChanged += OnCurrentDisplaySetPresentationImagesChanged;
				e.NewDisplaySet.PresentationImages.ItemRemoved += OnCurrentDisplaySetPresentationImagesChanged;
			}
    	}

    	private void OnCurrentDisplaySetPresentationImagesChanged(object sender, ListEventArgs<IPresentationImage> e)
    	{
    		UpdateScrollerRange(this.ImageBox.DisplaySet);
    	}

    	private void ImageScroller_ValueChanged(object sender, EventArgs e)
    	{
    		if (!_isScrollToolInitiatedEvent)
    		{
    			_isScrollBarInitiatedEvent = true;
    			try
    			{
    				this.ImageBox.TopLeftPresentationImageIndex = _imageScroller.Value;

					// make sure the scrollbar draws immediately!
    				_imageScroller.Update();

					// this ordering of draw makes it look smoother for some reason
    				this.ImageBox.Draw();
    			}
    			finally
    			{
    				_isScrollBarInitiatedEvent = false;
    			}
    		}
    	}

		/// <summary>
		/// Updates the display and range of the scrollbar
		/// </summary>
    	private void UpdateScrollerRange(IDisplaySet displaySet)
    	{
    		if (displaySet != null)
    		{
    			int tileCount = this.ImageBox.Tiles.Count;
				int maximum = Math.Max(0, displaySet.PresentationImages.Count - tileCount);
				if (maximum > 0)
				{
					_imageScroller.SetValueRange(this.ImageBox.TopLeftPresentationImageIndex, 0, maximum);
					_imageScroller.Increment = Math.Max(1, tileCount);
					_imageScroller.Visible = true;
				}
				else
				{
					_imageScroller.Visible = false;
				}
    		}
			else
    		{
    			_imageScroller.Visible = false;
    		}
    	}

		/// <summary>
		/// Tool that monitors the other stacking tools and updates the scrollbar accordingly.
		/// </summary>
    	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
    	private class ImageBoxControlScrollTool : ImageViewerTool
    	{
			protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
			{
				base.OnPresentationImageSelected(sender, e);

				try
				{
					IImageBox imageBox = FindImageBox(e.SelectedPresentationImage.ParentDisplaySet, this.ImageViewer);
					if (imageBox != null && _imageBoxControlIndex.ContainsKey(imageBox))
					{
						// get the control for the image
						ImageBoxControl imageBoxControl = _imageBoxControlIndex[imageBox];

						if (!imageBoxControl._isScrollBarInitiatedEvent)
						{
							imageBoxControl._isScrollToolInitiatedEvent = true;
							try
							{
								imageBoxControl._imageScroller.Value = imageBox.TopLeftPresentationImageIndex;

								// make sure the scrollbar draws immediately!
								imageBoxControl._imageScroller.Update();
							}
							finally
							{
								imageBoxControl._isScrollToolInitiatedEvent = false;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Warn, ex, "An unidentified exception has occured.");
#if DEBUG
					System.Diagnostics.Debug.Assert(false, ex.Message, ex.StackTrace);
#endif
				}
			}

			/// <summary>
			/// Finds the <see cref="IImageBox"/> containing the specified <paramref name="displaySet"/> in the specified <paramref name="viewer"/>.
			/// </summary>
    		private static IImageBox FindImageBox(IDisplaySet displaySet, IImageViewer viewer)
    		{
    			foreach (IImageBox box in viewer.PhysicalWorkspace.ImageBoxes)
    			{
    				if (box.DisplaySet == displaySet)
    					return box;
    			}
    			return null;
    		}
    	}

		#endregion
	}
}
