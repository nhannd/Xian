using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	/// <summary>
	/// Summary description for WorkspaceForm.
	/// </summary>
	public class ImageViewerControl : UserControl
    {
        private IContainer components;
		private PhysicalWorkspace _physicalWorkspace;
        private ContextMenuStrip _contextMenu;
		private IRenderer _renderer;
        private ImageViewerComponent _component;

		public ImageViewerControl(ImageViewerComponent component)
		{
			Trace.Write("ImageWorkspaceControl()\n");

            _component = component;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			SetFormProperties();
			CreateRenderer();

            _physicalWorkspace = _component.PhysicalWorkspace;
			_physicalWorkspace.ImageDrawing += new EventHandler<ImageDrawingEventArgs>(OnImageDrawing);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (_renderer != null)
					_renderer.Dispose();

				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this._contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // _contextMenu
            // 
            this._contextMenu.Name = "_contextMenu";
            this._contextMenu.Size = new System.Drawing.Size(153, 26);
            this._contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this._contextMenu_Opening);
            // 
            // ImageViewerControl
            // 
            this.ContextMenuStrip = this._contextMenu;
            this.Name = "ImageViewerControl";
            this.Size = new System.Drawing.Size(283, 227);
            this.SizeChanged += new System.EventHandler(this.OnSizeChanged);
            this.ResumeLayout(false);

		}
		#endregion

		private void CreateRenderer()
		{
            RendererExtensionPoint xp = new RendererExtensionPoint();
            _renderer = (IRenderer)xp.CreateExtension();
		}

		private void SetFormProperties()
		{
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.BackColor = Color.Black;
		}

		private XMouseEventArgs ConvertToXMouseEventArgs(MouseEventArgs e)
		{
			XMouseEventArgs args = new XMouseEventArgs((XMouseButtons)e.Button, e.Clicks, e.X, e.Y, e.Delta, (XKeys)Control.ModifierKeys);
			return args;
		}

		private XKeyEventArgs ConvertToXKeyEventArgs(KeyEventArgs e)
		{
			XKeyEventArgs args = new XKeyEventArgs((XKeys)e.KeyData);
			return args;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			_physicalWorkspace.OnMouseDown(ConvertToXMouseEventArgs(e));
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			_physicalWorkspace.OnMouseMove(ConvertToXMouseEventArgs(e));
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			_physicalWorkspace.OnMouseUp(ConvertToXMouseEventArgs(e));
			base.OnMouseUp(e);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			_physicalWorkspace.OnMouseWheel(ConvertToXMouseEventArgs(e));
			base.OnMouseWheel(e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			_physicalWorkspace.OnKeyDown(ConvertToXKeyEventArgs(e));
			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			_physicalWorkspace.OnKeyUp(ConvertToXKeyEventArgs(e));
			base.OnKeyUp(e);
		}

		private void OnSizeChanged(object sender, EventArgs e)
		{
			string str = String.Format("OnSizeChanged: {0}\n", this.ClientRectangle.ToString());
			Trace.Write(str);

			_physicalWorkspace.ClientRectangle = this.ClientRectangle;
			_physicalWorkspace.Draw(false);
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
            CodeClock counter = new CodeClock();
			counter.Start();

			Region paintRegion = null;

			foreach (ImageBox imageBox in _physicalWorkspace.ImageBoxes)
			{
				foreach (Tile tile in imageBox.Tiles)
				{
					ICustomDrawable customImage = tile.PresentationImage as ICustomDrawable;

					if (customImage != null)
					{
						// Draw the custom drawable tile
						IntPtr hDC = e.Graphics.GetHdc();
						customImage.Draw(hDC, tile.DrawableClientRectangle);
						e.Graphics.ReleaseHdc(hDC);

						// Define a paint region that will eventually be used to paint
						// portions of the clip rectangle occupied by regular, non-custom 
						// drawable tiles.
						if (paintRegion == null)
							paintRegion = new Region(e.ClipRectangle);
	
						// Exclude any portion of the custom drawable tile from the
						// paint region, since we don't want to paint over
						// the tile that we just drew
						paintRegion.Exclude(tile.DrawableClientRectangle);
					}
				}
			}

			if (paintRegion == null)
			{
				// No custom drawable tiles; just paint normally
				_renderer.Paint(e.Graphics, e.ClipRectangle);
			}
			else
			{
				// We have custom drawable tiles.
				// Convert that region into an array of rectangles that we can then
				// paint (BitBlt) individually
				RectangleF[] paintRects = paintRegion.GetRegionScans(new Matrix());

				foreach (RectangleF rect in paintRects)
					_renderer.Paint(e.Graphics, Rectangle.Round(rect));

				paintRegion.Dispose();
			}

			counter.Stop();

			string str = String.Format("OnPaint: {0}, {1}\n", counter.ToString(), e.ClipRectangle.ToString());
			Trace.Write(str);

			base.OnPaint(e);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			// We're double buffering manually, so override this and do nothing
		}

		private void OnImageDrawing(object sender, ImageDrawingEventArgs e)
		{
			Trace.Write("OnImageDrawing\n");

			ICustomDrawable customImage = e.PresentationImage as ICustomDrawable;

			Graphics graphics = null;
			
			// If it's a custom image, create a Graphics object so that it can 
			// render itself directly to the screen
			if (customImage != null)
				graphics = CreateGraphics();

			_renderer.Draw(graphics, e);

			if (graphics != null)
			{
				graphics.Flush();
				graphics.Dispose();
			}

			// If it's a custom image, don't bother invalidating, since
			// the drawing is taken care of entirely by the custom routine.
			if (customImage != null)
				return;

			// When we call IRenderer.Draw, the normal (non-custom) meaning 
			// is that we're drawing to a memory buffer.  IRenderer.Paint
			// "flips" the buffer to the screen.  So, we need to invalidate here
			// so that OnPaint can call IRenderer.Paint

			// If the image box layout has changed, invalidate the
			// entire window; otherwise, just invalidate the tile
			if (e.ImageBoxLayoutChanged)
				Invalidate();
			else if (e.TileLayoutChanged)
				Invalidate(e.ImageBox.DrawableClientRectangle);
			else
				Invalidate(Rectangle.Inflate(e.ImageBox.DrawableClientRectangle, 1, 1));
			
			if (e.PaintNow)
				Update();
		}

        private void _contextMenu_Opening(object sender, CancelEventArgs e)
        {
            ActionModelNode menuModel = _component.ContextMenuModel;

			if (menuModel != null)
			{
				ToolStripBuilder.Clear(_contextMenu.Items);
				ToolStripBuilder.BuildMenu(_contextMenu.Items, menuModel.ChildNodes);
				e.Cancel = false;
			}
			else
				e.Cancel = true;
        }
	}
}
