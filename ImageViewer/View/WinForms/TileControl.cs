using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Rendering;
using System.Diagnostics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="TileComponent"/>
    /// </summary>
    public partial class TileControl : UserControl
	{
		#region Private fields

		private Tile _tile;
		private IRenderingSurface _surface;
		private bool _maintainCapture;

		#endregion

		/// <summary>
        /// Constructor
        /// </summary>
        public TileControl(Tile tile)
        {
            InitializeComponent();

			_maintainCapture = false;

            _tile = tile;

			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.BackColor = Color.Black;

			_tile.Drawing += new EventHandler(OnDrawing);
			_tile.RendererChanged += new EventHandler(OnRendererChanged);
			_tile.NotifyCaptureChanging += new EventHandler<MouseCaptureChangingEventArgs>(this.OnCaptureChanging);
			_contextMenuStrip.Opening += new CancelEventHandler(OnContextMenuStripOpening);
        }

		public Tile Tile
		{
			get { return _tile; }
		}

		private IRenderingSurface Surface
		{
			get 
			{
				if (_surface == null)
				{
					// TileControl should *always* have an associated Tile
					if (this.Tile == null)
						throw new Exception("TileControl not associated with a Tile");

					// Legitimate case; Tile maybe empty
					if (this.Tile.PresentationImage == null)
						return null;

					IRenderer renderer = (this.Tile.PresentationImage as PresentationImage).ImageRenderer;

					// PresntationImage should *always* have a renderer
					if (renderer == null)
						throw new Exception("PresentationImage not associated with an IRenderer");

					_surface = renderer.GetRenderingSurface(this.Handle, this.Width, this.Height);
				}

				return _surface; 
			}
		}

		public void SetParentImageBoxRectangle(
			Rectangle parentImageBoxRectangle, 
			int parentImageBoxBorderWidth)
		{
			int insetImageBoxWidth = parentImageBoxRectangle.Width - 2 * parentImageBoxBorderWidth;
			int insetImageBoxHeight = parentImageBoxRectangle.Height - 2 * parentImageBoxBorderWidth;

			int left = (int)(_tile.NormalizedRectangle.Left * insetImageBoxWidth + _tile.InsetWidth);
			int top = (int)(_tile.NormalizedRectangle.Top * insetImageBoxHeight + _tile.InsetWidth);
			int right = (int)(_tile.NormalizedRectangle.Right * insetImageBoxWidth - _tile.InsetWidth);
			int bottom = (int)(_tile.NormalizedRectangle.Bottom * insetImageBoxHeight - _tile.InsetWidth);

			this.SuspendLayout();

			this.Left = left + parentImageBoxBorderWidth;
			this.Top = top + parentImageBoxBorderWidth;
			this.Width = right - left;
			this.Height = bottom - top;

			this.ResumeLayout();
		}

		public void Draw()
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			Render();

			Invalidate();
			Update();

			clock.Stop();
			string str = String.Format("OnImageDrawing: {0}\n", clock.ToString());
			Trace.Write(str);
		}

		private void Render()
		{
			DrawArgs args = new DrawArgs(this.Surface, this.ClientRectangle, this.ClientRectangle, ClearCanvas.ImageViewer.Rendering.DrawMode.Render);
			_tile.OnDraw(args);
		}

		private void OnDrawing(object sender, EventArgs e)
		{
			Draw();
		}

		private void OnRendererChanged(object sender, EventArgs e)
		{
			_surface = null;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.Surface == null)
			{
				e.Graphics.Clear(Color.Black);
			}
			else
			{
				this.Surface.ContextID = e.Graphics.GetHdc();
				DrawArgs args = new DrawArgs(this.Surface, this.ClientRectangle, e.ClipRectangle, ClearCanvas.ImageViewer.Rendering.DrawMode.Refresh);
				_tile.OnDraw(args);
			}

			base.OnPaint(e);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			// We're double buffering manually, so override this and do nothing
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			// Set the surface to null so when it's accessed, a new surface
			// will be created.
			_surface = null;
			Render();
			base.OnSizeChanged(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			_tile.OnMouseDown(ConvertToXMouseEventArgs(e));
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			_tile.OnMouseMove(ConvertToXMouseEventArgs(e));
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			_tile.OnMouseUp(ConvertToXMouseEventArgs(e));
			base.OnMouseUp(e);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			_tile.OnMouseWheel(ConvertToXMouseEventArgs(e));
			base.OnMouseWheel(e);
		}

		protected override void OnMouseCaptureChanged(EventArgs e)
		{
			base.OnMouseCaptureChanged(e);

			//This feels bad to me, but it's the only way to accomplish
			//keeping the capture when the mouse has come up.  .NET automatically handles
			//capture for you by turning it on on mouse down and off on mouse up, but
			//it does not allow you to keep capture when the mouse is not down.  Even
			// if you take out the calls to the base class OnMouseX handlers, it still
			// turns Capture back off although it has been turned on explicitly.
			if (this._maintainCapture)
				this.Capture = true;
		}

		private XMouseEventArgs ConvertToXMouseEventArgs(MouseEventArgs e)
		{
			XMouseEventArgs args = new XMouseEventArgs((XMouseButtons)e.Button, e.Clicks, e.X, e.Y, e.Delta, (XKeys)Control.ModifierKeys);
			return args;
		}

		private void OnCaptureChanging(object sender, MouseCaptureChangingEventArgs e)
		{
			_maintainCapture = (e.UIEventHandlerGainingCapture != null);
		}

		void OnContextMenuStripOpening(object sender, CancelEventArgs e)
		{
			ActionModelNode menuModel = (_tile.ImageViewer as ImageViewerComponent).ContextMenuModel;

			if (menuModel != null)
			{
				ToolStripBuilder.Clear(_contextMenuStrip.Items);
				ToolStripBuilder.BuildMenu(_contextMenuStrip.Items, menuModel.ChildNodes);
				e.Cancel = false;
			}
			else
				e.Cancel = true;
		}
    }
}
