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
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="TileComponent"/>
    /// </summary>
    public partial class TileControl : UserControl
	{
		#region Private fields

		private Tile _tile;
		private InputTranslator _inputTranslator;
		private TileInputController _tileController;

		private InformationBox _currentInformationBox;

		private IRenderingSurface _surface;
		private IMouseButtonHandler _currentMouseButtonHandler;
		private CursorWrapper _currentCursorWrapper;

		#endregion

		/// <summary>
        /// Constructor
        /// </summary>
        public TileControl(Tile tile, Rectangle parentRectangle, int parentImageBoxInsetWidth)
        {
			_tile = tile;
			SetParentImageBoxRectangle(parentRectangle, parentImageBoxInsetWidth);

			_inputTranslator = new InputTranslator(this.GetModifiers);
			_tileController = new TileInputController(_tile);
			
			InitializeComponent();

			this.BackColor = Color.Black;
			this.Dock = DockStyle.None;
			this.Anchor = AnchorStyles.None;

			_tile.Drawing += new EventHandler(OnDrawing);
			_tile.RendererChanged += new EventHandler(OnRendererChanged);
			_tile.InformationBoxChanged += new EventHandler<InformationBoxChangedEventArgs>(OnInformationBoxChanged);
			_tile.CursorTokenChanged += new EventHandler(OnCursorTokenChanged);

			_contextMenuStrip.Opening += new CancelEventHandler(OnContextMenuStripOpening);

			_tileController.NotifyCaptureChanging += new EventHandler<CaptureChangingEventArgs>(OnCaptureChanging);
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

		private Keys GetModifiers()
		{
			return Control.ModifierKeys;
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

			this.Location = new Point(left + parentImageBoxBorderWidth, top + parentImageBoxBorderWidth);
			this.Size = new Size(right - left, bottom - top);

			this.ResumeLayout(false);
		}
		
		public void Draw()
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			DrawArgs args = new DrawArgs(this.Surface, this.ClientRectangle, this.ClientRectangle, ClearCanvas.ImageViewer.Rendering.DrawMode.Render);
			_tile.OnDraw(args);
			Invalidate();
			Update();

			clock.Stop();
			string str = String.Format("TileControl.Draw: {0}, {1}\n", clock.ToString(), this.Size.ToString());
			Trace.Write(str);
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
			// Make sure tile gets blacked out if there's
			// no presentation image in it
			if (_tile.PresentationImage == null)
				_surface = null;

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

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);

		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			Trace.Write("TileControl.OnSizeChanged()\n");

			// Set the surface to null so when it's accessed, a new surface
			// will be created.
			_surface = null;
			Draw();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.Focus();
			_tileController.ProcessMessage(_inputTranslator.OnMouseDown(e));
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			_tileController.ProcessMessage(_inputTranslator.OnMouseMove(e));
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			_tileController.ProcessMessage(_inputTranslator.OnMouseUp(e));
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			_tileController.ProcessMessage(_inputTranslator.OnMouseWheel(e));
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (_tileController.ProcessMessage(_inputTranslator.OnKeyDown(e)))
				e.Handled = true;

			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (_tileController.ProcessMessage(_inputTranslator.OnKeyUp(e)))
				e.Handled = true;

			base.OnKeyUp(e);
		}

		protected override bool IsInputKey(Keys keyData)
		{
			//We want the tile control to receive keydown messages for *all* keys.
			return true;
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
			if (this._currentMouseButtonHandler != null)
				this.Capture = true;
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			// Notify the surface that the tile control's window handle is
			// about to be destroyed so that any objects using the handle have
			// a chance to deal with it
			if (_surface != null)
				_surface.WindowID = IntPtr.Zero;

			base.OnHandleDestroyed(e);
		}

		private void OnCaptureChanging(object sender, CaptureChangingEventArgs e)
		{
			if (_currentMouseButtonHandler == e.GainingCapture)
				return;

			bool setToken = false;
			CursorToken token = null;

			//This code has been provided for 2 reasons:
			//  1. Generally, all tools must obtain capture so that other objects don't interfere with them (like ROIs) as the mouse moves.
			//  2. Most of the time, it is desirable for the tool to set the cursor while it is active.
			// This faciliates an easy way to set the cursor for the general Tool case.  Also, any IUIEventHandler that captures the mouse
			// can use a CursorTokenAttribute and the cursor will be set automatically.  NOTE: The CursorTokenAttribute, although it is
			// applied to tools in much the same ways as the ActionAttributes, it is not an ActionAttribute.

			//The existing 'capturer' set the cursor before?
			if (_currentMouseButtonHandler != null)
			{
				object[] cursor = _currentMouseButtonHandler.GetType().GetCustomAttributes(typeof(CursorTokenAttribute), true);
				if (cursor != null && cursor.Length > 0)
					setToken = true;
			}

			//New 'capturer' sets the cursor?
			_currentMouseButtonHandler = e.GainingCapture;
			if (_currentMouseButtonHandler != null)
			{
				object[] cursor = _currentMouseButtonHandler.GetType().GetCustomAttributes(typeof(CursorTokenAttribute), true);
				if (cursor != null && cursor.Length > 0)
				{
					setToken = true;
					token = ((CursorTokenAttribute)cursor[0]).CursorToken;
				}
			}
			
			if (setToken)
				_tile.CursorToken = token;
		}

		void OnCursorTokenChanged(object sender, EventArgs e)
		{
			if (_tile.CursorToken == null)
			{
				this.Cursor = this.DefaultCursor;

				if (_currentCursorWrapper != null)
				{
					_currentCursorWrapper.Dispose();
					_currentCursorWrapper = null;
				}
			}
			else
			{
				try
				{
					CursorWrapper oldCursorWrapper = _currentCursorWrapper;
					_currentCursorWrapper = CursorFactory.CreateCursor(_tile.CursorToken);
					this.Cursor = _currentCursorWrapper.Cursor;

					if (oldCursorWrapper != null)
						oldCursorWrapper.Dispose();
				}
				catch (Exception exception)
				{
					Platform.Log(exception);
					this.Cursor = this.DefaultCursor;
					_currentCursorWrapper = null;
				}
			}
		}

		void OnContextMenuStripOpening(object sender, CancelEventArgs e)
		{
			ActionModelNode menuModel = (_tile.ImageViewer as ImageViewerComponent).ContextMenuModel;

			if (_tileController.ContextMenuEnabled)
			{
				ToolStripBuilder.Clear(_contextMenuStrip.Items);
				ToolStripBuilder.BuildMenu(_contextMenuStrip.Items, menuModel.ChildNodes);
				e.Cancel = false;
			}
			else
				e.Cancel = true;
		}

		void OnInformationBoxChanged(object sender, InformationBoxChangedEventArgs e)
		{
			if (_currentInformationBox != null)
				_currentInformationBox.Updated -= new EventHandler(OnUpdateInformationBox);

			_currentInformationBox = e.InformationBox;
			
			_toolTip.Active = false;
			_toolTip.Hide(this);

			if (e.InformationBox != null)
				_currentInformationBox.Updated += new EventHandler(OnUpdateInformationBox);
		}

		void OnUpdateInformationBox(object sender, EventArgs e)
		{
			if (!_currentInformationBox.Visible)
			{
				_toolTip.Active = false;
				_toolTip.Hide(this);
			}
			else
			{
				_toolTip.Active = true;
				Point point = new Point(_currentInformationBox.DestinationPoint.X, _currentInformationBox.DestinationPoint.Y);
				point.Offset(5, 5);
				_toolTip.Show(_currentInformationBox.Data, this, point);
			}
		}
    }
}
