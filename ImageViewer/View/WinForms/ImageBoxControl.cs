using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Desktop;

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
		public ImageBoxControl(ImageBox imageBox)
        {
			_imageBox = imageBox;
			
			InitializeComponent();

			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.BackColor = Color.Black;

			_imageBox.Drawing += new EventHandler(OnDrawing);
			_imageBox.SelectionChanged += new EventHandler<ImageBoxEventArgs>(OnImageBoxSelectionChanged);
			_imageBox.TileAdded += new EventHandler<TileEventArgs>(OnTileAdded);
			_imageBox.TileRemoved += new EventHandler<TileEventArgs>(OnTileRemoved);

			AddTileControls(imageBox);
        }

		public ImageBox ImageBox
		{
			get { return _imageBox; }
		}

		public Rectangle ParentRectangle
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

				this.Left = left;
				this.Top = top;
				this.Width = right - left;
				this.Height = bottom - top;

				this.ResumeLayout();
			}
		}

		public void Draw()
		{
			if (this.ImageBox.LayoutRefreshRequired)
			{
				LayoutTiles();
			}
			else
			{
				foreach (TileControl control in this.Controls)
					control.Draw();
			}
		}

		void OnDrawing(object sender, EventArgs e)
		{
			Draw();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			LayoutTiles();

			base.OnSizeChanged(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			// Draw image box border
			DrawBorder(
				e.Graphics, 
				this.ClientRectangle,
				_imageBox.BorderColor,
				_imageBox.BorderWidth, 
				_imageBox.InsetWidth);

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
						control.Tile.BorderWidth,
						control.Tile.InsetWidth);
				}
			}

			base.OnPaint(e);
		}

		void OnImageBoxSelectionChanged(object sender, ImageBoxEventArgs e)
		{
			Invalidate();
			Update();
		}

		void OnTileSelectionChanged(object sender, TileEventArgs e)
		{
			Invalidate();
			Update();
		}

		void OnTileAdded(object sender, TileEventArgs e)
		{
			AddTileControl(e.Tile as Tile);
		}

		void OnTileRemoved(object sender, TileEventArgs e)
		{
			foreach (TileControl control in this.Controls)
			{
				if (e.Tile == control.Tile)
				{
					control.Dispose();
					this.Controls.Remove(control);
					return;
				}
			}
		}

		private void LayoutTiles()
		{
			this.SuspendLayout();

			foreach (TileControl control in this.Controls)
				control.SetParentImageBoxRectangle(this.ClientRectangle, _imageBox.InsetWidth);

			_imageBox.LayoutRefreshRequired = false;

			this.ResumeLayout();
		}

		private Rectangle GetTileRectangle(TileControl control)
		{
			Rectangle tileRectangle = new Rectangle(control.Location, control.Size);

			Rectangle borderRectangle =
				Rectangle.Inflate(
					tileRectangle,
					control.Tile.InsetWidth,
					control.Tile.InsetWidth);

			return borderRectangle;
		}

		private void DrawBorder(Graphics graphics, Rectangle rectangle, Color borderColor, int borderWidth, int insetWidth)
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
			foreach (Tile tile in imageBox.Tiles)
				AddTileControl(tile);
		}

		private void AddTileControl(Tile tile)
		{
			TileView view = ViewFactory.CreateAssociatedView(typeof(Tile)) as TileView;

			view.Tile = tile;

			TileControl control = view.GuiElement as TileControl;
			control.Tile.SelectionChanged += new EventHandler<TileEventArgs>(OnTileSelectionChanged);
			this.Controls.Add(control);
		}
    }
}
