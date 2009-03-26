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
using System.Drawing;
using System.Diagnostics;
using System.Threading;
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
	/// - The <see cref="TileController"/> receives messages from the view layer, interprets them, and allows the appropriate domain objects to handle
	/// the messages in a prescribed manner.  Here is a (highly simplified) description of how it works:
	/// <list>
	/// <item>
	/// <description>
	/// - When the mouse moves without any buttons down, all <see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/>s 
	/// that implement <see cref="IMouseButtonHandler"/> have their <see cref="IMouseButtonHandler.Track"/> method called.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// - When a mouse button is clicked, an <see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/> is searched for at the current mouse position
	/// that implements <see cref="IMouseButtonHandler"/>.  If one is found, it is given 'capture' until it releases capture or capture is canceled by the framework.
	/// If no <see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/> is found, then all of the current <see cref="IPresentationImage"/>'s 
	/// <see cref="ClearCanvas.Desktop.Tools.ITool"/>s are searched for an <see cref="IMouseButtonHandler"/> and the same rules are applied.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// - When the right mouse button is clicked, the same thing occurs as for the left mouse button, but when it is released, a context menu is shown
	/// provided the mouse didn't move more than a couple of pixels.
	/// </description>
	/// </item>
	/// <item>
	/// <description>
	/// When the mouse wheel is used, a similar approach is taken as mentioned above for <see cref="IMouseButtonHandler"/>s, but for <see cref="IMouseWheelHandler"/>s.  
	/// However, only <see cref="ClearCanvas.Desktop.Tools.ITool"/>s are given the opportunity to handle the mouse wheel, not <see cref="ClearCanvas.ImageViewer.Graphics.IGraphic"/>s.
	/// </description>
	/// </item>
	/// </list>
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
	public sealed class TileController : IMouseInformation
	{
		private delegate bool CallHandlerMethodDelegate(IMouseButtonHandler handler);

		#region MouseWheelManager class 

		private class MouseWheelManager
		{
			private readonly uint WheelStopDelayMilliseconds = 500;

			//One per UI Thread.
			[ThreadStatic]
			private static MouseWheelManager _instance;

			private IMouseWheelHandler _captureMouseWheelHandler;
			private DateTime _timeOfLastWheel;
			private volatile bool _stopTimer;

			private MouseWheelManager()
			{
				_captureMouseWheelHandler = null;
				_stopTimer = true;
			}

			public static MouseWheelManager Instance
			{
				get
				{
					if (_instance == null)
						_instance = new MouseWheelManager();

					return _instance;
				}
			}

			public void UpdateLastWheelTime()
			{
				_timeOfLastWheel = Platform.Time;
			}

			public void SetCaptureHandler(IMouseWheelHandler captureMouseWheelHandler)
			{
				if (_captureMouseWheelHandler == captureMouseWheelHandler)
					return;

				if (_captureMouseWheelHandler != null)
					_captureMouseWheelHandler.StopWheel();

				_captureMouseWheelHandler = captureMouseWheelHandler;

				if (_captureMouseWheelHandler == null)
				{
					_stopTimer = true;
					return;
				}

				_captureMouseWheelHandler.StartWheel();

				_stopTimer = false;
				SynchronizationContext context = SynchronizationContext.Current;
				WaitCallback del = delegate
				                   	{
										while (!_stopTimer)
										{
											Thread.Sleep(10);
											context.Post(delegate
												 	{
														if (_captureMouseWheelHandler == null)
															return;

														TimeSpan elapsed = Platform.Time.Subtract(_timeOfLastWheel);
														if (elapsed.Milliseconds >= WheelStopDelayMilliseconds)
															SetCaptureHandler(null);
												 	}
												, null);
										}
				                   	};

				ThreadPool.QueueUserWorkItem(del);
			}
		}

		#endregion
		
		#region Private Fields

		private readonly Tile _tile;
		private bool _selectedOnThisClick;
		private Point _startMousePoint;
		private Point _currentMousePoint;
		private Rectangle _tileClientRectangle;

		private MemorableUndoableCommand _command;
		private IMouseButtonHandler _captureHandler;
		private int _startCount = 0;
		private CursorToken _cursorToken;
		
		private bool _contextMenuEnabled; 
		private IContextMenuProvider _contextMenuProvider;

		private event EventHandler _cursorTokenChanged;
		private event EventHandler<ItemEventArgs<IMouseButtonHandler>> _captureChangingEvent;

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
			_selectedOnThisClick = false;
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

				// Developer's note: make sure your memento classes override and implement
				// object.Equals so that unnecessary commands don't get added to the command
				// history.

				// Also, note that the begin state will be captured after IMouseButtonHandler.Start
				// has been called, so if your originator object is sensitive to a change 
				// that occurs in the Start() method, store it as a variable and return it
				// from CreateMemento.  The end state will be captured after the last call
				// to IMouseButtonHandler.Stop (or Cancel), so always ensure the returned 
				// end state is accurate.

				if (_captureHandler != null && _captureHandler is IMemorable)
				{
					_command.EndState = ((IMemorable) _captureHandler).CreateMemento();
					if (_command.BeginState != null && !_command.BeginState.Equals(_command.EndState))
					{
						DrawableUndoableCommand drawableCommand = new DrawableUndoableCommand(_tile);
						drawableCommand.Enqueue(_command);
						_tile.ImageViewer.CommandHistory.AddCommand(drawableCommand);
					}
					
					_command = null;
				}

				_captureHandler = value;

				if (_captureHandler != null && _captureHandler is IMemorable)
				{
					IMemorable originator = (IMemorable) _captureHandler;
					_command = new MemorableUndoableCommand(originator);
					_command.BeginState = originator.CreateMemento();
				}
				
				//fire our own event first, so the view can release 'real' capture 
				// before other notifications go out through the event broker.
				EventsHelper.Fire(_captureChangingEvent, this, new ItemEventArgs<IMouseButtonHandler>(_captureHandler));

				_tile.ImageViewer.EventBroker.OnMouseCaptureChanged(new MouseCaptureChangedEventArgs(_tile, _captureHandler != null));
			}
		}

		private IMouseWheelHandler CaptureMouseWheelHandler
		{
			set 
			{
				MouseWheelManager.Instance.SetCaptureHandler(value);
			}
		}

		#endregion

		#region Private Methods

		private static bool CanStartOnDoubleClick(IMouseButtonHandler handler)
		{
			return false == CancelStartOnDoubleClick(handler);
		}

		private static bool IgnoreDoubleClicks(IMouseButtonHandler handler)
		{
			return (handler.Behaviour & MouseButtonHandlerBehaviour.IgnoreDoubleClicks) == MouseButtonHandlerBehaviour.IgnoreDoubleClicks;
		}

		private static bool CancelStartOnDoubleClick(IMouseButtonHandler handler)
		{
			return (handler.Behaviour & MouseButtonHandlerBehaviour.CancelStartOnDoubleClick) == MouseButtonHandlerBehaviour.CancelStartOnDoubleClick;
		}

		private static bool SuppressContextMenu(IMouseButtonHandler handler)
		{
			return (handler.Behaviour & MouseButtonHandlerBehaviour.SuppressContextMenu) == MouseButtonHandlerBehaviour.SuppressContextMenu;
		}

		private static bool ConstrainToTile(IMouseButtonHandler handler)
		{
			return (handler.Behaviour & MouseButtonHandlerBehaviour.ConstrainToTile) == MouseButtonHandlerBehaviour.ConstrainToTile;
		}

		private static bool SuppressOnTileActivate(IMouseButtonHandler handler)
		{
			return (handler.Behaviour & MouseButtonHandlerBehaviour.SuppressOnTileActivate) == MouseButtonHandlerBehaviour.SuppressOnTileActivate;
		}

		#region Mouse Message Processing

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

		private void SetCursorToken(IMouseButtonHandler handler)
		{
			SetCursorToken(handler, Location);
		}

		private void SetCursorToken()
		{
			SetCursorToken(CaptureHandler, Location);
		}

		private void SetCapture(IMouseButtonHandler handler)
		{
			Trace.WriteLine(String.Format("Setting capture: {0}", handler.GetType().FullName));

			this.CaptureHandler = handler;

			if (SuppressContextMenu(this.CaptureHandler))
				_contextMenuEnabled = false;

			SetCursorToken();
			//tools can't have context menus
			if (handler is IGraphic)
				this.ContextMenuProvider = handler as IContextMenuProvider;
		}

		private void ReleaseCapture(bool cancel)
		{
			if (this.CaptureHandler != null && cancel)
				this.CaptureHandler.Cancel();

			_startCount = 0;

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
			if (!_tile.Enabled)
				return true;

			ReleaseCapture(true);

			IMouseWheelHandler handler = _shortcutManager.GetMouseWheelHandler(wheelMessage.Shortcut);
			if (handler != null)
			{
				this.CaptureMouseWheelHandler = handler;

				handler.Wheel(wheelMessage.WheelDelta);
				MouseWheelManager.Instance.UpdateLastWheelTime();
				return true;
			}

			return false;
		}

		#region Mouse Button Down

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

			if (StartCaptureHandler(buttonMessage))
				return true;

			_tile.Select();
			_contextMenuEnabled = (buttonMessage.Shortcut.MouseButton == XMouseButtons.Right);

			_startMousePoint = buttonMessage.Location;

			if (_tile.PresentationImage == null || !_tile.Enabled)
				return true;

			//give unfocused graphics a chance to focus (in the case of going straight from context menu to a graphic).
			FindHandlingGraphic(TrackHandler);

			return StartNewHandler(buttonMessage);
		}

		private bool StartCaptureHandler(MouseButtonMessage buttonMessage)
		{
			if (this.CaptureHandler == null)
				return false;

			if (SuppressContextMenu(this.CaptureHandler))
				_contextMenuEnabled = false;

			if (CancelStartDueToDoubleClick())
			{
				Trace.WriteLine(String.Format("Double-click release {0}", this.CaptureHandler.GetType()));
				ReleaseCapture(true);

				StartNewHandler(buttonMessage);
			}
			else
			{
				Trace.WriteLine(String.Format("Start (Clicks: {0}, Count: {1})", _clickCount, _startCount));
				StartHandler(this.CaptureHandler);
				SetCursorToken();
			}

			//we only release capture on button up, so just consume.
			return true;
		}

		private bool StartNewHandler(MouseButtonMessage buttonMessage)
		{
			if (StartNewGraphicHandler())
				return true;
			else 
				return StartNewToolHandler(buttonMessage);
		}

		private bool StartNewGraphicHandler()
		{
			if (_tile.PresentationImage is PresentationImage)
			{
				CompositeGraphic sceneGraph = ((PresentationImage)_tile.PresentationImage).SceneGraph;
				foreach (IMouseButtonHandler handler in GetHandlerGraphics(sceneGraph))
				{
					if (CanStartNewHandler(handler) && StartHandler(handler))
					{
						SetCapture(handler);
						return true;
					}
				}
			}

			return false;
		}

		private bool StartNewToolHandler(MouseButtonMessage buttonMessage)
		{
			foreach (IMouseButtonHandler handler in _shortcutManager.GetMouseButtonHandlers(buttonMessage.Shortcut))
			{
				if (CanStartNewHandler(handler) && StartHandler(handler))
				{
					SetCapture(handler);
					return true;
				}
			}

			return false;
		}

		private bool CanStartNewHandler(IMouseButtonHandler handler)
		{
			if (_clickCount < 2)
				return true;
			else if (CanStartOnDoubleClick(handler))
				return true;
			else
				return false;
		}

		private bool CancelStartDueToDoubleClick()
		{
			return (_clickCount > 1 && _startCount == 1 && CancelStartOnDoubleClick(this.CaptureHandler));
		}

		#endregion

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

				Trace.WriteLine(String.Format("Release capture {0}", this.CaptureHandler.GetType()));

				ReleaseCapture(false);
				return true;
			}

			return false;
		}

		private bool ProcessMouseButtonMessage(MouseButtonMessage buttonMessage)
		{
			this.Location = buttonMessage.Location;

			bool returnValue;
			if (buttonMessage.ButtonAction == MouseButtonMessage.ButtonActions.Down)
			{
				_selectedOnThisClick = !_tile.Selected;
				returnValue = ProcessMouseButtonDownMessage(buttonMessage);
				_selectedOnThisClick = false;
			}
			else
			{
				returnValue = ProcessMouseButtonUpMessage(buttonMessage);
			}

			return returnValue;
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
				if (ConstrainToTile(this.CaptureHandler) && !this.TileClientRectangle.Contains(this.Location))
				{
					SetCursorToken(null);
					return true;
				}

				if (this.CaptureHandler.Track(this))
				{
					SetCursorToken();
					return true;
				}
			}

			if (!_tile.Enabled)
				return true;

			IMouseButtonHandler handler = FindHandlingGraphic(TrackHandler);
			SetCursorToken(handler);
			return (handler != null);
		}

		private bool StartHandler(IMouseButtonHandler handler)
		{
			if (_selectedOnThisClick && SuppressOnTileActivate(handler))
				return false;

			if (_clickCount > 1 && IgnoreDoubleClicks(handler))
				return false;

			bool start = handler.Start(this);
			if (start)
				++_startCount;

			return start;
		}

		private bool TrackHandler(IMouseButtonHandler handler)
		{
			if (!_tile.Selected && SuppressOnTileActivate(handler))
				return false;

			if (ConstrainToTile(handler) && !this.TileClientRectangle.Contains(this.Location))
				return false;

			return handler.Track(this);
		}

		private bool StopHandler(IMouseButtonHandler handler)
		{
			bool handled = handler.Stop(this);
			if (!handled)
				_startCount = 0;

			return handled;
		}

		private IMouseButtonHandler FindHandlingGraphic(CallHandlerMethodDelegate handlerDelegate)
		{
			if (_tile.PresentationImage is PresentationImage)
			{
				CompositeGraphic sceneGraph = ((PresentationImage) _tile.PresentationImage).SceneGraph;
				foreach (IMouseButtonHandler handler in GetHandlerGraphics(sceneGraph))
				{
					if (handlerDelegate(handler))
						return handler;
				}
			}

			return null;
		}

		private IEnumerable<IMouseButtonHandler> GetHandlerGraphics(CompositeGraphic compositeGraphic)
		{
			foreach (IGraphic graphic in compositeGraphic.EnumerateChildGraphics(true))
			{
				if (!graphic.Visible)
					continue;

				if (graphic is IMouseButtonHandler)
				{
					yield return graphic as IMouseButtonHandler;
				}
				else if (graphic is CompositeGraphic)
				{
					foreach (IMouseButtonHandler handler in GetHandlerGraphics(graphic as CompositeGraphic))
						yield return handler;
				}
			}
		}

		#endregion
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
			else if (message is MouseButtonMessage)
			{
				return ProcessMouseButtonMessage(message as MouseButtonMessage);
			}
			else if (message is TrackMousePositionMessage)
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
				else if (message is KeyboardButtonMessage)
				{
					bool returnValue = ProcessKeyboardMessage(message as KeyboardButtonMessage);
					TrackCurrentPosition();
					return returnValue;
				}
				else if (message is ReleaseCaptureMessage)
				{
					ReleaseCapture(true);
					TrackCurrentPosition();
					return true;
				}
				else if (message is MouseLeaveMessage)
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
		public event EventHandler<ItemEventArgs<IMouseButtonHandler>> CaptureChanging
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