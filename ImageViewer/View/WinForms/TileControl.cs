#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Rendering;
using DrawMode=ClearCanvas.ImageViewer.Rendering.DrawMode;
using MessageBox=ClearCanvas.Desktop.View.WinForms.MessageBox;
using Screen=System.Windows.Forms.Screen;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="TileComponent"/>
	/// </summary>
	public partial class TileControl : UserControl
	{
		#region Private fields

		private Tile _tile;
		private TileInputTranslator _inputTranslator;
		private TileController _tileController;

		private InformationBox _currentInformationBox;

		private EditBoxControl _editBoxControl;

		private IRenderingSurface _surface;
		private IMouseButtonHandler _currentMouseButtonHandler;
		private CursorWrapper _currentCursorWrapper;

		[ThreadStatic]
		private static bool _drawing = false;

		[ThreadStatic]
		private static bool _painting = false;

		[ThreadStatic]
		private static readonly List<TileControl> _tilesToRepaint = new List<TileControl>();

		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		public TileControl(Tile tile, Rectangle parentRectangle, int parentImageBoxInsetWidth)
		{
			_tile = tile;
			_tileController = new TileController(_tile, (_tile.ImageViewer as ImageViewerComponent).ShortcutManager);
			_inputTranslator = new TileInputTranslator(this);

			SetParentImageBoxRectangle(parentRectangle, parentImageBoxInsetWidth);
			InitializeComponent();

			this.BackColor = Color.Black;
			this.Dock = DockStyle.None;
			this.Anchor = AnchorStyles.None;
			this.AllowDrop = true;

			_tile.Drawing += new EventHandler(OnDrawing);
			_tile.RendererChanged += new EventHandler(OnRendererChanged);
			_tile.InformationBoxChanged += new EventHandler<InformationBoxChangedEventArgs>(OnInformationBoxChanged);
			_tile.EditBoxChanged += new EventHandler(OnEditBoxChanged);

			_contextMenuStrip.ImageScalingSize = new Size(24, 24);
			_contextMenuStrip.Opening += new CancelEventHandler(OnContextMenuStripOpening);

			_tileController.CursorTokenChanged += new EventHandler(OnCursorTokenChanged);
			_tileController.CaptureChanging += new EventHandler<ItemEventArgs<IMouseButtonHandler>>(OnCaptureChanging);

			_editBoxControl = new EditBoxControl();
			this.Controls.Add(_editBoxControl);

			this.DoubleBuffered = false;
			this.SetStyle(ControlStyles.DoubleBuffer, false);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
			this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
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
						throw new Exception(SR.ExceptionTileControlNoAssociatedTile);

					// Legitimate case; Tile maybe empty
					if (this.Tile.PresentationImage == null)
						return null;

					IRenderer renderer = ((PresentationImage) Tile.PresentationImage).ImageRenderer;

					// PresntationImage should *always* have a renderer
					if (renderer == null)
						throw new Exception(SR.ExceptionPresentationImageNotAssociatedWithARenderer);

					_surface = renderer.GetRenderingSurface(this.Handle, this.Width, this.Height);
				}

				return _surface;
			}
		}

		public void SetParentImageBoxRectangle(
			Rectangle parentImageBoxRectangle,
			int parentImageBoxBorderWidth)
		{
			int insetImageBoxWidth = parentImageBoxRectangle.Width - 2*parentImageBoxBorderWidth;
			int insetImageBoxHeight = parentImageBoxRectangle.Height - 2*parentImageBoxBorderWidth;

			int left = (int) (_tile.NormalizedRectangle.Left*insetImageBoxWidth + Tile.InsetWidth);
			int top = (int) (_tile.NormalizedRectangle.Top*insetImageBoxHeight + Tile.InsetWidth);
			int right = (int) (_tile.NormalizedRectangle.Right*insetImageBoxWidth - Tile.InsetWidth);
			int bottom = (int) (_tile.NormalizedRectangle.Bottom*insetImageBoxHeight - Tile.InsetWidth);

			this.SuspendLayout();

			this.Location = new Point(left + parentImageBoxBorderWidth, top + parentImageBoxBorderWidth);
			this.Size = new Size(right - left, bottom - top);
			this.ResumeLayout(false);
		}

		public void Draw()
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			if (this.Surface != null)
			{
				System.Drawing.Graphics graphics = this.CreateGraphics();

				this.Surface.WindowID = this.Handle;
				this.Surface.ContextID = graphics.GetHdc();
				this.Surface.ClientRectangle = this.ClientRectangle;
				this.Surface.ClipRectangle = this.ClientRectangle;

				DrawArgs args = new DrawArgs(this.Surface,
				                             new WinFormsScreenProxy(Screen.FromControl(this)),
				                             DrawMode.Render);

				string errorMessage = null;

				_drawing = true;

				try
				{
					_tile.Draw(args);
				}
				catch (Exception ex)
				{
					errorMessage = ex.Message;
				}
				finally
				{
					_drawing = false;

					graphics.ReleaseHdc(this.Surface.ContextID);
					graphics.Dispose();

					if (errorMessage != null)
					{
						MessageBox mb = new MessageBox();
						mb.Show(errorMessage);
					}
				}
			}

			//Cause the tile to paint/refresh.
			Invalidate();
			Update();

			clock.Stop();
			string str = String.Format("TileControl.Draw: {0}, {1}\n", clock.ToString(), this.Size.ToString());
			Trace.Write(str);
		}

		private void DisposeSurface()
		{
			try
			{
				if (_surface != null)
					_surface.Dispose();
			}
			finally
			{
				_surface = null;
			}
		}

		#region Overrides

		private void OnDrawing(object sender, EventArgs e)
		{
			Draw();
		}

		private void OnRendererChanged(object sender, EventArgs e)
		{
			DisposeSurface();
		}

		private bool IsVistaOrLater()
		{
			return Environment.OSVersion.Version.Major >= 6;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			//Assume anything Vista or later has the same issues.
			if (IsVistaOrLater())
			{
				//Windows Vista is opportunistic when it comes to wait conditions (e.g. locks, Mutexes, etc)
				//in that it will actually process WM_PAINT messages on the current thread, even though
				//it is supposed to be blocking in a WaitSleepJoin state.  This behaviour can actually
				//break rendering for a couple of reasons:
				//  1. We do custom double-buffering, and it's possible that we could process a paint message
				//     for an image that hasn't actually been rendered to the back buffer yet.
				//  2. The renderer itself accesses the pixel data of the ImageSops, which is a synchronized operation.
				//     In the case where 2 threads try to load the pixel data of an image simultaneously, the renderer can end up
				//     blocking execution on the main UI thread in the middle of a rendering operation.  If we
				//     allow another tile to paint in this situation, it actually causes some GDI errors because
				//     the previous rendering operation has not yet completed.
				if (_drawing || _painting)
				{
					e.Graphics.Clear(Color.Black);

					//Queue this tile up for deferred painting and return.
					if (!_tilesToRepaint.Contains(this))
						_tilesToRepaint.Add(this);

					return;
				}

				//We're about to paint this tile, so remove it from the queue.
				_tilesToRepaint.Remove(this);
			}

			if (_tile.PresentationImage == null)
				DisposeSurface();

			if (this.Surface == null)
			{
				// Make sure tile gets blacked out if there's
				// no presentation image in it
				e.Graphics.Clear(Color.Black);
			}
			else
			{
				this.Surface.WindowID = this.Handle;
				this.Surface.ContextID = e.Graphics.GetHdc();
				this.Surface.ClientRectangle = this.ClientRectangle;
				this.Surface.ClipRectangle = e.ClipRectangle;

				DrawArgs args = new DrawArgs(this.Surface,
				                             new WinFormsScreenProxy(Screen.FromControl(this)),
				                             DrawMode.Refresh);

				string errorMessage = null;

				_painting = true;

				try
				{
					_tile.Draw(args);
				}
				catch (Exception ex)
				{
					errorMessage = ex.Message;
				}
				finally
				{
					_painting = false;
					e.Graphics.ReleaseHdc(this.Surface.ContextID);

					if (errorMessage != null)
					{
						MessageBox mb = new MessageBox();
						mb.Show(errorMessage);
					}
				}
			}

			// Now that we've finished painting this tile, we can process the deferred paint jobs.
			// The code below is self-fulfilling, in that we remove one tile from the queue and
			// invalidate it, causing it to paint.  When it's done painting, it will remove and
			// invalidate the next one, and so on.
			if (IsVistaOrLater() && _tilesToRepaint.Count > 0)
			{
				TileControl tileToRepaint = _tilesToRepaint[0];
				_tilesToRepaint.RemoveAt(0);

				tileToRepaint.Invalidate();
				tileToRepaint.Update();
			}

			//base.OnPaint(e);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			// We're double buffering manually, so override this and do nothing
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);

			Trace.Write("TileControl.OnSizeChanged()\n");

			if (_tileController != null)
				_tileController.TileClientRectangle = this.ClientRectangle;

			Draw();
		}

		#region Mouse/Keyboard Overrides

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			object message = _inputTranslator.OnMouseLeave();
			if (message == null)
				return;

			if (_tileController != null)
				_tileController.ProcessMessage(message);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.Focus();

			object message = _inputTranslator.OnMouseDown(e);
			if (message == null)
				return;

			if (_tileController != null)
				_tileController.ProcessMessage(message);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			object message = _inputTranslator.OnMouseMove(e);
			if (message == null)
				return;

			if (_tileController != null)
				_tileController.ProcessMessage(message);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			object message = _inputTranslator.OnMouseUp(e);
			if (message == null)
				return;

			if (_tileController != null)
				_tileController.ProcessMessage(message);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			object message = _inputTranslator.OnMouseWheel(e);
			if (message == null)
				return;

			if (_tileController != null)
				_tileController.ProcessMessage(message);
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

		protected override void OnKeyDown(KeyEventArgs e)
		{
			object message = _inputTranslator.OnKeyDown(e);
			if (message == null)
				return;

			if (_tileController != null && _tileController.ProcessMessage(message))
				e.Handled = true;

			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			object message = _inputTranslator.OnKeyUp(e);
			if (message == null)
				return;

			if (_tileController != null && _tileController.ProcessMessage(message))
				e.Handled = true;

			base.OnKeyUp(e);
		}

		protected override bool IsInputKey(Keys keyData)
		{
			//We want the tile control to receive keydown messages for *all* keys.
			return true;
		}

		public override bool PreProcessMessage(ref Message msg)
		{
			object message = _inputTranslator.PreProcessMessage(msg);
			if (message != null && _tileController != null)
				_tileController.ProcessMessage(message);

			bool returnValue = base.PreProcessMessage(ref msg);

			message = _inputTranslator.PostProcessMessage(msg, returnValue);
			if (message != null && _tileController != null)
				_tileController.ProcessMessage(message);

			return returnValue;
		}

		#endregion

		#region Drag/Drop Overrides

		protected override void OnDragOver(DragEventArgs drgevent)
		{
			if (drgevent.Data.GetDataPresent(typeof (DisplaySet)))
				drgevent.Effect = DragDropEffects.Move;
			else
				drgevent.Effect = DragDropEffects.None;

			base.OnDragOver(drgevent);
		}

		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			_tile.Select();

			MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(_tile.ParentImageBox);
			memorableCommand.BeginState = _tile.ParentImageBox.CreateMemento();

			IDisplaySet displaySet = (IDisplaySet) drgevent.Data.GetData(typeof (DisplaySet));
			_tile.ParentImageBox.DisplaySet = displaySet.CreateFreshCopy();
			_tile.ParentImageBox.Draw();

			memorableCommand.EndState = _tile.ParentImageBox.CreateMemento();

			DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(_tile.ParentImageBox);
			historyCommand.Enqueue(memorableCommand);
			_tile.ImageViewer.CommandHistory.AddCommand(historyCommand);

			base.OnDragDrop(drgevent);
		}

		#endregion

		protected override void OnHandleDestroyed(EventArgs e)
		{
			// Notify the surface that the tile control's window handle is
			// about to be destroyed so that any objects using the handle have
			// a chance to deal with it
			if (_surface != null)
				_surface.WindowID = IntPtr.Zero;

			base.OnHandleDestroyed(e);
		}

		#endregion

		private void OnCaptureChanging(object sender, ItemEventArgs<IMouseButtonHandler> e)
		{
			if (_currentMouseButtonHandler == e.Item)
				return;

			_currentMouseButtonHandler = e.Item;
			this.Capture = (_currentMouseButtonHandler != null);
		}

		private void OnCursorTokenChanged(object sender, EventArgs e)
		{
			if (_tileController == null)
				return;

			if (_tileController.CursorToken == null)
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
					_currentCursorWrapper = CursorFactory.CreateCursor(_tileController.CursorToken);
					this.Cursor = _currentCursorWrapper.Cursor;

					if (oldCursorWrapper != null)
						oldCursorWrapper.Dispose();
				}
				catch (Exception exception)
				{
					Platform.Log(LogLevel.Error, exception);
					this.Cursor = this.DefaultCursor;
					_currentCursorWrapper = null;
				}
			}
		}

		private void OnContextMenuStripOpening(object sender, CancelEventArgs e)
		{
			if (_tileController == null || _tileController.ContextMenuProvider == null)
			{
				e.Cancel = true;
				return;
			}

			if (_tileController.ContextMenuEnabled)
			{
				ActionModelNode menuModel = _tileController.ContextMenuProvider.GetContextMenuModel(_tileController);
				ToolStripBuilder.Clear(_contextMenuStrip.Items);
				ToolStripBuilder.BuildMenu(_contextMenuStrip.Items, menuModel.ChildNodes);
				e.Cancel = false;
			}
			else
				e.Cancel = true;
		}

		private void OnInformationBoxChanged(object sender, InformationBoxChangedEventArgs e)
		{
			if (_currentInformationBox != null)
				_currentInformationBox.Updated -= new EventHandler(OnUpdateInformationBox);

			_currentInformationBox = e.InformationBox;

			_toolTip.Active = false;
			_toolTip.Hide(this);

			if (e.InformationBox != null)
				_currentInformationBox.Updated += new EventHandler(OnUpdateInformationBox);
		}

		private void OnUpdateInformationBox(object sender, EventArgs e)
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

		private void OnEditBoxChanged(object sender, EventArgs e)
		{
			_editBoxControl.EditBox = _tile.EditBox;
		}
	}
}