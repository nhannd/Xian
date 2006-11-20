using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public sealed class TileController : IInputController, IMouseInformation
	{
		private delegate bool CallHandlerMethodDelegate(IMouseButtonHandler handler);

		#region Private Fields

		private Tile _tile;
		private Point _startMousePoint;
		private Point _currentMousePoint;

		private IMouseButtonHandler _captureHandler;
 
		private CursorToken _cursorToken;
		
		private bool _contextMenuEnabled; 
		private ActionModelNode _contextMenuModel;

		private event EventHandler _cursorTokenChanged;
		private event EventHandler<CaptureChangingEventArgs> _captureChangingEvent;

		private XMouseButtons _buttonDown;

		#endregion

		public TileController(Tile tile)
		{
			Platform.CheckForNullReference(tile, "tile");
			_tile = tile;
		}

		private TileController()
		{
		}

		#region Public Properties

		public bool ContextMenuEnabled
		{
			get { return _contextMenuEnabled; }
		}

		public ActionModelNode ContextMenuModel
		{
			get
			{
				if (_contextMenuModel == null)
					_contextMenuModel = (_tile.ImageViewer as ImageViewerComponent).ContextMenuModel;

				return _contextMenuModel;
			}
			set
			{
				_contextMenuModel = value;
			}
		}

		public CursorToken CursorToken
		{
			get
			{
				return _cursorToken;
			}
			set
			{
				if (_cursorToken == value)
					return;

				_cursorToken = value;
				EventsHelper.Fire(_cursorTokenChanged, this, new EventArgs());
			}
		}

		public IMouseButtonHandler CaptureHandler
		{
			get { return _captureHandler; }
			set
			{
				if (_captureHandler == value)
					return;

				CaptureChangingEventArgs args = new CaptureChangingEventArgs(value, _captureHandler);
				_captureHandler = value;
				EventsHelper.Fire(_captureChangingEvent, this, args);
			}
		}

		#endregion

		#region Public Events

		public event EventHandler CursorTokenChanged
		{
			add { _cursorTokenChanged += value; }
			remove { _cursorTokenChanged -= value; }
		}

		public event EventHandler<CaptureChangingEventArgs> CaptureChanging
		{
			add { _captureChangingEvent += value; }
			remove { _captureChangingEvent -= value; }
		}

		#endregion

		#region Private Methods

		private void SetCursorToken(IMouseButtonHandler handler, Point location)
		{
			if (handler is ICursorTokenProvider)
			{
				this.CursorToken = (handler as ICursorTokenProvider).GetCursorToken(location);
			}
			else
			{
				this.CursorToken = null;
			}
		}

		private void SetContextMenu(IMouseButtonHandler handler, Point location)
		{
			if (handler is IContextMenuProvider)
			{
				this.ContextMenuModel = (handler as IContextMenuProvider).GetContextMenuModel(location);
			}
			else
			{
				this.ContextMenuModel = null;
			}
		}

		private void ReleaseCapture(bool cancel)
		{
			if (this.CaptureHandler != null && cancel)
				this.CaptureHandler.Cancel();

			this.CaptureHandler = null;
			this.CursorToken = null;
			this.ContextMenuModel = null;
		}

		private bool ProcessKeyboardMessage(KeyboardButtonMessage keyboardMessage)
		{
			//keyboard up messages are just consumed.
			if (keyboardMessage.ButtonAction == KeyboardButtonMessage.ButtonActions.Up)
				return true;

			ReleaseCapture(true);

			ImageViewerComponent viewer = _tile.ImageViewer as ImageViewerComponent;
			object handler = viewer.ShortcutManager[keyboardMessage];
			if (handler is IClickAction)
			{
				((IClickAction)handler).Click();
				return true;
			}

			return false;
		}

		private bool ProcessMouseWheelMessage(MouseWheelMessage wheelMessage)
		{
			ReleaseCapture(true);

			ImageViewerComponent viewer = _tile.ImageViewer as ImageViewerComponent;
			object handler = viewer.ShortcutManager[wheelMessage];

			if (handler is MouseWheelDelegate)
			{
				(handler as MouseWheelDelegate)();
				return true;
			}

			return false;
		}

		private bool ProcessMouseButtonDownMessage(MouseButtonMessage buttonMessage)
		{
			//don't allow multiple buttons, it's just cleaner and easier to manage behaviour.
			if (_buttonDown != 0)
			{
				_contextMenuEnabled = false;
				return true;
			}

			_buttonDown = buttonMessage.Shortcut.MouseButton;

			if (this.CaptureHandler != null)
			{
				if (this.CaptureHandler.SuppressContextMenu)
					_contextMenuEnabled = false;

				StartHandler(this.CaptureHandler);
				SetCursorToken(this.CaptureHandler, buttonMessage.Location);

				//we only release capture on button up, so just consume.
				return true;
			}

			_tile.Select();
			_contextMenuEnabled = (buttonMessage.Shortcut.MouseButton == XMouseButtons.Right);
			_startMousePoint = buttonMessage.Location;

			//give unfocused graphics a chance to focus (in the case of going straight from context menu to a graphic).
			FindHandlingGraphic(TrackHandler);

			IMouseButtonHandler handler = FindHandlingGraphic(StartHandler);
			if (handler != null)
			{
				this.CaptureHandler = handler;

				if (handler.SuppressContextMenu)
					_contextMenuEnabled = false;

				SetCursorToken(handler, buttonMessage.Location); 
				SetContextMenu(handler, buttonMessage.Location);

				return true;
			}

			ImageViewerComponent viewer = _tile.ImageViewer as ImageViewerComponent;
			object shortcutobject = viewer.ShortcutManager[buttonMessage];

			if (shortcutobject is IMouseButtonHandler)
			{
				handler = shortcutobject as IMouseButtonHandler;
				if (StartHandler(handler))
				{
					this.CaptureHandler = handler;

					if (handler.SuppressContextMenu)
						_contextMenuEnabled = false;

					SetCursorToken(handler, buttonMessage.Location); 

					return true;
				}
			}

			return false;
		}

		private bool ProcessMouseButtonUpMessage(MouseButtonMessage buttonMessage)
		{
			if (_buttonDown != buttonMessage.Shortcut.MouseButton)
				return true;

			_buttonDown = 0;

			if (this.CaptureHandler != null)
			{
				if (StopHandler(this.CaptureHandler))
					return true;

				ReleaseCapture(false);
				return true;
			}

			return false;
		}

		private bool ProcessMouseButtonMessage(MouseButtonMessage buttonMessage)
		{
			this.Location = buttonMessage.Location;

			if (buttonMessage.ButtonAction == MouseButtonMessage.ButtonActions.Down)
			{
				return ProcessMouseButtonDownMessage(buttonMessage);
			}

			return ProcessMouseButtonUpMessage(buttonMessage);
		}

		private void TrackCurrentPosition()
		{
			ProcessTrackMessage(new TrackMousePositionMessage(this.Location));
		}

		private bool ProcessTrackMessage(TrackMousePositionMessage trackMessage)
		{
			this.Location = trackMessage.Location;

			if (_startMousePoint != trackMessage.Location)
				_contextMenuEnabled = false;

			if (this.CaptureHandler != null)
			{
				if (this.CaptureHandler.Track(this))
				{
					SetCursorToken(this.CaptureHandler, trackMessage.Location);
					return true;
				}
			}

			IMouseButtonHandler handler = FindHandlingGraphic(TrackHandler);
			SetCursorToken(handler, trackMessage.Location);
			return (handler != null);
		}

		private bool StartHandler(IMouseButtonHandler handler)
		{
			return handler.Start(this);
		}

		private bool TrackHandler(IMouseButtonHandler handler)
		{
			return handler.Track(this);
		}

		private bool StopHandler(IMouseButtonHandler handler)
		{
			return handler.Stop(this);
		}

		private IMouseButtonHandler FindHandlingGraphic(CallHandlerMethodDelegate handlerDelegate)
		{
			GraphicLayer selectedGraphicLayer = _tile.PresentationImage.LayerManager.SelectedGraphicLayer;
			if (selectedGraphicLayer == null)
				return null;

			//Traverse the graphics in reverse order, that way the last one drawn will be the first focus candidate.
			for (int i = selectedGraphicLayer.Graphics.Count - 1; i >= 0; --i)
			{
				Layer layer = selectedGraphicLayer.Graphics[i];
				
				if (!layer.Visible)
					continue;

				if (layer is IMouseButtonHandler)
				{
					IMouseButtonHandler handler = (IMouseButtonHandler)layer;
					if (handlerDelegate(handler))
						return handler;
				}
			}

			return null;
		}

		#endregion

		#region IController Members

		public bool ProcessMessage(IInputMessage message)
		{
			if (_tile.PresentationImage != null)
			{
				if (message is MouseButtonMessage)
				{
					return ProcessMouseButtonMessage(message as MouseButtonMessage);
				}

				if (message is TrackMousePositionMessage)
				{
					return ProcessTrackMessage(message as TrackMousePositionMessage);
				}

				if (message is MouseWheelMessage)
				{
					bool returnValue = ProcessMouseWheelMessage(message as MouseWheelMessage);
					TrackCurrentPosition();
					return returnValue;
				}

				if (message is KeyboardButtonMessage)
				{
					bool returnValue = ProcessKeyboardMessage(message as KeyboardButtonMessage);
					TrackCurrentPosition();
					return returnValue;
				}

				if (message is ReleaseCaptureMessage)
				{
					ReleaseCapture(true);
					TrackCurrentPosition();
					return true;
				}
			}

			return false;
		}
		
		#endregion

		#region IMouseInformation Members

		public ITile Tile
		{
			get { return _tile; }
		}

		public Point Location
		{
			get { return _currentMousePoint; }
			private set { _currentMousePoint = value; }
		}

		public XMouseButtons MouseButtonDown
		{
			get { return _buttonDown; }
		}

		#endregion
	}
}
