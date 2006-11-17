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

namespace ClearCanvas.ImageViewer.InputManagement
{
	public sealed class TileInputController : IInputController
	{
		private Tile _tile;
		private Point _lastMousePoint;
		private bool _contextMenuEnabled;

		private IMouseButtonHandler _currentPointerAction;
		private event EventHandler<CaptureChangingEventArgs> _captureChangingEvent;

		public TileInputController(Tile tile)
		{
			_tile = tile;
			_tile.CurrentPointerActionChanged += new EventHandler(OnCurrentPointerActionChanged);
		}

		private TileInputController()
		{ 
		}

		public event EventHandler<CaptureChangingEventArgs> NotifyCaptureChanging
		{
			add { _captureChangingEvent += value; }
			remove { _captureChangingEvent -= value; }
		}

		public bool ContextMenuEnabled
		{
			get { return _contextMenuEnabled; }
		}

		void OnCurrentPointerActionChanged(object sender, EventArgs e)
		{
			IMouseButtonHandler newAction = _tile.CurrentPointerAction;

			CaptureChangingEventArgs args = new CaptureChangingEventArgs(newAction, _currentPointerAction);
			_currentPointerAction = newAction;
			EventsHelper.Fire(_captureChangingEvent, this, args);
		}

		public bool ControlPointer(IMouseButtonHandler handler, IInputMessage message)
		{
			if (message is MouseButtonMessage)
			{
				MouseButtonMessage buttonMessage = (MouseButtonMessage)message;
				if (buttonMessage.ButtonAction == MouseButtonMessage.ButtonActions.Down)
				{
					return handler.Start(new MouseInformation(_tile, buttonMessage.Location));
				}
				else if (buttonMessage.ButtonAction == MouseButtonMessage.ButtonActions.Up)
				{
					return handler.Stop(new MouseInformation(_tile, buttonMessage.Location));
				}
			}
			else if (message is TrackMousePositionMessage)
			{
				TrackMousePositionMessage pointerMessage = message as TrackMousePositionMessage;
				return handler.Track(new MouseInformation(_tile, pointerMessage.Location));
			}

			return false;
		}

		#region IController Members

		public bool ProcessMessage(IInputMessage message)
		{
			if (_tile.PresentationImage == null)
				return true;

			if (message is MouseButtonMessage)
			{
				MouseButtonMessage buttonMessage = (MouseButtonMessage)message;

				if (buttonMessage.ButtonAction == MouseButtonMessage.ButtonActions.Down)
				{
					_tile.Select();
					_contextMenuEnabled = (buttonMessage.Shortcut.MouseButton == XMouseButtons.Right);
					_lastMousePoint = buttonMessage.Location;
				}
			}
			else if (message is TrackMousePositionMessage)
			{
				TrackMousePositionMessage pointerMessage = (TrackMousePositionMessage)message;
				if (_lastMousePoint != pointerMessage.Location)
					_contextMenuEnabled = false;
			}

			if (_tile.CurrentPointerAction != null)
			{
				if (ControlPointer(_tile.CurrentPointerAction, message))
					return true;
			}

			if (_tile.PresentationImage.LayerManager.SelectedGraphicLayer != null)
			{
				foreach (Layer layer in _tile.PresentationImage.LayerManager.SelectedGraphicLayer.Graphics)
				{
					if (layer is IMouseButtonHandler)
					{
						if (ControlPointer(layer as IMouseButtonHandler, message))
							return true;
					}
				}
			}

			ImageViewerComponent viewer = _tile.ImageViewer as ImageViewerComponent;
			object handler = viewer.ShortcutManager[message];

			if (handler != null)
			{
				if (handler is IMouseButtonHandler)
				{
					if (ControlPointer((IMouseButtonHandler)handler, message))
						return true;
				}
				else if (handler is MouseWheelDelegate)
				{
					MouseWheelDelegate wheelDelegate = (MouseWheelDelegate)handler;
					if (wheelDelegate != null)
					{
						wheelDelegate();
						return true;
					}
				}
				else if (handler is IClickAction)
				{
					((IClickAction)handler).Click();
					return true;
				}
			}

			return false;
		}
		
		#endregion
	}
}
