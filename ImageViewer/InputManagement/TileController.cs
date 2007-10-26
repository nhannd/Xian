#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InputManagement
{
	/// <summary>
	/// This class controls the behaviour of objects in the <see cref="ITile"/>, namely the <see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/>s 
	/// in the current <see cref="IPresentationImage"/>'s SceneGraph (<see cref="PresentationImage.SceneGraph"/>) and <see cref="ClearCanvas.Desktop.Tools.ITool"/>s 
	/// belonging to the current <see cref="IPresentationImage"/>, specifically those that implement <see cref="IMouseButtonHandler"/> and/or <see cref="IMouseWheelHandler"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="TileController"/> receives messages from the view layer, interprets them, and allows the appropriate domain objects to handle
	/// the messages in a prescribed manner.  Here is a (highly simplified) description of how it works:
	/// </para>
	/// <para>
	///   - When the mouse moves without any buttons down, all <see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/>s 
	///     that implement <see cref="IMouseButtonHandler"/> have their <see cref="IMouseButtonHandler.Track"/> method called.
	///   - When a mouse button is clicked, an <see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/> is searched for at the current mouse position
	///     that implements <see cref="IMouseButtonHandler"/>.  If one is found, it is given 'capture' until it releases capture or capture is canceled by the framework.
	///     If no <see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/> is found, then all of the current <see cref="IPresentationImage"/>'s 
	///     <see cref="ClearCanvas.Desktop.Tools.ITool"/>s are searched for an <see cref="IMouseButtonHandler"/> and the same rules are applied.
	///   - When the right mouse button is clicked, the same thing occurs as for the left mouse button, but when it is released, a context menu is shown
	///     provided the mouse didn't move more than a couple of pixels.
	///   - When the mouse wheel is used, a similar approach is taken as mentioned above for <see cref="IMouseButtonHandler"/>s, but for <see cref="IMouseWheelHandler"/>s.  
	///     However, only <see cref="ClearCanvas.Desktop.Tools.ITool"/>s are given the opportunity to handle the mouse wheel, not <see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/>s.
	/// </para>
	/// <para>
	/// Note that this object is instantiated from within the view layer and cannot be accessed from application or domain level code.  
	/// This is deliberate as it is intended for internal framework use only.
	/// </para>
	/// </remarks>
	/// <seealso cref="IMouseButtonHandler"/>
	/// <seealso cref="IMouseWheelHandler"/>
	/// <seealso cref="IMouseInformation"/>
	/// <seealso cref="ITile"/>
	/// <seealso cref="IPresentationImage"/>
	/// <seealso cref="PresentationImage"/>
	/// <seealso cref="PresentationImage.SceneGraph"/>
	/// <seealso cref="ClearCanvas.Desktop.Tools.ITool"/>
	/// <seealso cref="ClearCanvas.ImageViewer.BaseTools.ImageViewerTool"/>
	/// <seealso cref="ClearCanvas.ImageViewer.BaseTools.MouseImageViewerTool"/>
	public sealed partial class TileController : IMouseInformation
	{
		private delegate bool CallHandlerMethodDelegate(IMouseButtonHandler handler);

		#region Private Fields

		private readonly Tile _tile;
		private Point _startMousePoint;
		private Point _currentMousePoint;
		private Rectangle _tileClientRectangle;

		private IMouseButtonHandler _captureHandler;

		//The wheel can only act on one tile at a time, and a click in another tile should stop the timer, therefore use statics.
		private static IMouseWheelHandler _captureMouseWheelHandler;
		private static Timer _wheelHandlerTimer;
		private static DateTime _timeOfLastWheel;
		private static readonly uint WheelStopDelayMilliseconds = 500;

		private CursorToken _cursorToken;
		
		private bool _contextMenuEnabled; 
		private IContextMenuProvider _contextMenuProvider;

		private event EventHandler _cursorTokenChanged;
		private event EventHandler<CaptureChangingEventArgs> _captureChangingEvent;

		private XMouseButtons _activeButton;
		private uint _clickCount;

		private readonly IViewerShortcutManager _shortcutManager;

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public TileController(Tile tile, IViewerShortcutManager shortcutManager)
		{
			Platform.CheckForNullReference(tile, "tile");
			Platform.CheckForNullReference(shortcutManager, "shortcutManager");

			_tile = tile;
			_shortcutManager = shortcutManager;
		}

		#region Private Properties

		private IMouseButtonHandler CaptureHandler
		{
			get { return _captureHandler; }
			set
			{
				if (_captureHandler == value)
					return;

				CaptureChangingEventArgs args = new CaptureChangingEventArgs(value, _captureHandler);
				_captureHandler = value;
				EventsHelper.Fire(_captureChangingEvent, this, args);

				if (_tile.ImageViewer != null)
					_tile.ImageViewer.EventBroker.OnCaptureChanging(args);
			}
		}

		private IMouseWheelHandler CaptureMouseWheelHandler
		{
			get { return _captureMouseWheelHandler; }
			set 
			{
				if (_captureMouseWheelHandler == value)
					return;

				if (_captureMouseWheelHandler != null)
				{
					_captureMouseWheelHandler.Stop();
					_wheelHandlerTimer.Stop();
				}

				_captureMouseWheelHandler = value;
				
				if (_captureMouseWheelHandler != null)
				{
					if (_wheelHandlerTimer == null)
					{
						_wheelHandlerTimer = new Timer(OnTimer);
						_wheelHandlerTimer.Interval = 10;
					}

					_wheelHandlerTimer.Start();
					_captureMouseWheelHandler.Start();
				}
			}
		}

		#endregion

		#region Private Methods

		private void OnTimer(object nothing)
		{
			if (_captureMouseWheelHandler == null)
				return;

			TimeSpan elapsed = Platform.Time.Subtract(_timeOfLastWheel);
			if (elapsed.Milliseconds >= WheelStopDelayMilliseconds)
				this.CaptureMouseWheelHandler = null;
		}

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

		private void ReleaseCapture(bool cancel)
		{
			if (this.CaptureHandler != null && cancel)
				this.CaptureHandler.Cancel();

			this.CaptureHandler = null;
			this.CursorToken = null;
			this.ContextMenuProvider = null;
		}

		private bool ProcessKeyboardMessage(KeyboardButtonMessage keyboardMessage)
		{
			//keyboard up messages are just consumed.
			if (keyboardMessage.ButtonAction == KeyboardButtonMessage.ButtonActions.Up)
				return true;

			ReleaseCapture(true);
			this.CaptureMouseWheelHandler = null;

			IClickAction action = _shortcutManager.GetKeyboardAction(keyboardMessage.Shortcut);
			if (action != null)
			{
				action.Click();
				return true;
			}

			return false;
		}

		private bool ProcessMouseWheelMessage(MouseWheelMessage wheelMessage)
		{
			ReleaseCapture(true);

			IMouseWheelHandler handler = _shortcutManager.GetMouseWheelHandler(wheelMessage.Shortcut);
			if (handler != null)
			{
				this.CaptureMouseWheelHandler = handler;

				handler.Wheel(wheelMessage.WheelDelta);
				_timeOfLastWheel = Platform.Time;
				return true;
			}

			return false;
		}

		private bool ProcessMouseButtonDownMessage(MouseButtonMessage buttonMessage)
		{
			this.CaptureMouseWheelHandler = null;

			//don't allow multiple buttons, it's just cleaner and easier to manage behaviour.
			if (_activeButton != 0)
			{
				_contextMenuEnabled = false;
				return true;
			}

			_activeButton = buttonMessage.Shortcut.MouseButton;
			_clickCount = buttonMessage.ClickCount;

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
			
			if (_tile.PresentationImage == null)
				return true;

			//give unfocused graphics a chance to focus (in the case of going straight from context menu to a graphic).
			FindHandlingGraphic(TrackHandler);

			IMouseButtonHandler handler = FindHandlingGraphic(StartHandler);
			if (handler != null)
			{
				this.CaptureHandler = handler;

				if (handler.SuppressContextMenu)
					_contextMenuEnabled = false;

				SetCursorToken(handler, buttonMessage.Location);
				this.ContextMenuProvider = handler as IContextMenuProvider;

				return true;
			}

			handler = _shortcutManager.GetMouseButtonHandler(buttonMessage.Shortcut);
			if (handler != null)
			{
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
			if (_activeButton != buttonMessage.Shortcut.MouseButton)
				return true;

			_activeButton = 0;
			_clickCount = 0;

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
						
			if (Math.Abs(_startMousePoint.X - this.Location.X) > 2 ||
			    Math.Abs(_startMousePoint.Y - this.Location.Y) > 2)
				_contextMenuEnabled = false;

			if (this.CaptureHandler != null)
			{
				if (this.CaptureHandler.ConstrainToTile && !this.TileClientRectangle.Contains(this.Location))
				{
					SetCursorToken(null, trackMessage.Location);
					return true;
				}

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
			if (handler.ConstrainToTile && !this.TileClientRectangle.Contains(this.Location))
				return false;

			return handler.Track(this);
		}

		private bool StopHandler(IMouseButtonHandler handler)
		{
			return handler.Stop(this);
		}

		private IMouseButtonHandler FindHandlingGraphic(CallHandlerMethodDelegate handlerDelegate)
		{
			PresentationImage image = this.Tile.PresentationImage as PresentationImage;
			if (image == null)
				return null;

			return FindHandlingGraphic(image.SceneGraph, handlerDelegate);
		}

		private IMouseButtonHandler FindHandlingGraphic(CompositeGraphic compositeGraphic, CallHandlerMethodDelegate handlerDelegate)
		{
			for (int graphicIndex = compositeGraphic.Graphics.Count - 1; graphicIndex >= 0; --graphicIndex)
			{
				IGraphic graphic = compositeGraphic.Graphics[graphicIndex];

				if (!graphic.Visible)
					continue;

				if (graphic is IMouseButtonHandler)
				{
					IMouseButtonHandler handler = (IMouseButtonHandler)graphic;
					if (handlerDelegate(handler))
						return handler;
				}
				else if (graphic is CompositeGraphic)
				{
					IMouseButtonHandler handler = FindHandlingGraphic(graphic as CompositeGraphic, handlerDelegate);
					if (handler != null)
						return handler;
				}
			}

			return null;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Used by the view layer to tell this object what the <see cref="Tile"/>'s client rectangle is.
		/// </summary>
		public Rectangle TileClientRectangle
		{
			get { return _tileClientRectangle; }
			set { _tileClientRectangle = value; }
		}

		/// <summary>
		/// Used by the view layer to decide whether or not to show the context menu.
		/// </summary>
		public bool ContextMenuEnabled
		{
			get { return _contextMenuEnabled; }
		}

		/// <summary>
		/// Used by the view layer to retrieve the <see cref="ActionModelNode"/> in order to show a context menu.
		/// </summary>
		public IContextMenuProvider ContextMenuProvider
		{
			get
			{
				if (_contextMenuProvider == null)
					_contextMenuProvider = _tile.ImageViewer as IContextMenuProvider;

				return _contextMenuProvider;
			}
			set
			{
				_contextMenuProvider = value;
			}
		}

		/// <summary>
		/// Used by the view layer to determine the <see cref="CursorToken"/> to show.
		/// </summary>
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

		#endregion

		#region Public Methods

		/// <summary>
		/// Called by the view layer so that the <see cref="TileController"/> can process the <paramref name="message"/>.
		/// </summary>
		public bool ProcessMessage(object message)
		{
			if (message is KeyboardButtonDownPreview)
			{
				//Right now, we can't determine what these keystrokes are going to do, so we just release mouse wheel capture.
				KeyboardButtonDownPreview preview = message as KeyboardButtonDownPreview;
				if (preview.Shortcut.KeyData != XKeys.None)
					this.CaptureMouseWheelHandler = null;

				return false;
			}
			
			if (message is MouseButtonMessage)
			{
				return ProcessMouseButtonMessage(message as MouseButtonMessage);
			}
			
			if (message is TrackMousePositionMessage)
			{
				return ProcessTrackMessage(message as TrackMousePositionMessage);
			}

			if (_tile.PresentationImage != null)
			{
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

				if (message is MouseLeaveMessage)
					_tile.PresentationImage.FocussedGraphic = null;
			}

			return false;
		}

		#endregion

		#region Public Events

		/// <summary>
		/// For use by the view layer, so it can detect when the <see cref="CursorToken"/> has changed.
		/// </summary>
		public event EventHandler CursorTokenChanged
		{
			add { _cursorTokenChanged += value; }
			remove { _cursorTokenChanged -= value; }
		}

		/// <summary>
		/// For use by the view layer, so it can detect when capture is changing.
		/// </summary>
		/// <seealso cref="CaptureChangingEventArgs"/>
		public event EventHandler<CaptureChangingEventArgs> CaptureChanging
		{
			add { _captureChangingEvent += value; }
			remove { _captureChangingEvent -= value; }
		}

		#endregion

		#region IMouseInformation Members

		/// <summary>
		/// The <see cref="ITile"/> that is controlled by this <see cref="TileController"/>.
		/// </summary>
		/// <remarks>
		/// For use by consumers of <see cref="IMouseInformation"/>.
		/// </remarks>
		public ITile Tile
		{
			get { return _tile; }
		}

		/// <summary>
		/// The current mouse location, set by the view layer.
		/// </summary>
		/// <remarks>
		/// For use by consumers of <see cref="IMouseInformation"/>.
		/// </remarks>
		public Point Location
		{
			get { return _currentMousePoint; }
			private set { _currentMousePoint = value; }
		}

		/// <summary>
		/// Gets the currently depressed (<see cref="XMouseButtons"/>) mouse button, set internally by this class.
		/// </summary>
		/// <remarks>
		/// For use by consumers of <see cref="IMouseInformation"/>.
		/// </remarks>
		public XMouseButtons ActiveButton
		{
			get { return _activeButton; }
		}

		/// <summary>
		/// Gets the current click count.
		/// </summary>
		/// <remarks>
		/// For use by consumers of <see cref="IMouseInformation"/>.
		/// </remarks>
		public uint ClickCount
		{
			get { return _clickCount; }
		}

		#endregion
	}
}