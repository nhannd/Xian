using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// The CaptureUIEventHandler is used to capture a UIEventHandler at the bottom level of the UIEventHandler
	/// chain and route messages directly to it until it releases the capture.  Other objects can release the capture
	/// as well, so the object that takes capture should also subscribe to the NotifyCaptureChanging event.
	/// </summary>
	internal class CaptureUIEventHandler : IUIEventHandler, IMouseCapture
	{
		private IUIEventHandler _captureEventHandler = null;
		private XMouseEventArgs _captureEventArgs = null;

		/// <summary>
		/// Set up the mouse event arguments that are passed in <see cref="XMouseEventArgs"/> so that
		/// its properties that would normally be set via the UIEventHandler chain (selectedPresentationImage, etc)
		/// are correct.
		/// </summary>
		/// <param name="e">The event args to set up.</param>
		private void SetupMouseEventArgs(XMouseEventArgs e)
		{
			e.MouseCapture = this;
			e.SelectedDisplaySet = _captureEventArgs.SelectedDisplaySet;
			e.SelectedImageBox = _captureEventArgs.SelectedImageBox;
			e.SelectedPresentationImage = _captureEventArgs.SelectedPresentationImage;
			e.SelectedTile = _captureEventArgs.SelectedTile;
		}

		#region IMouseCapture Members

		public event EventHandler<MouseCaptureChangingEventArgs> NotifyCaptureChanging;

		/// <summary>
		/// Sets the capture to a particular UIEventHandler.  Normally, that event handler is responsible for releasing
		/// the capture also, since all messages will be routed to it.  However, in the event that an object
		/// sets the capture while another still has the capture the <see cref="NotifyCaptureChanging"/> event will 
		/// get fired.  Any objects that need to know about capture being lost must subscribe to the <see cref="NotifyCaptureChanging"/> event.
		/// </summary>
		/// <param name="eventHandler">The eventhandler to give capture to.</param>
		/// <param name="e">The event args to mimic when routing messages directly to a handler.</param>
		public void SetCapture(IUIEventHandler eventHandler, XMouseEventArgs e)
		{
			IUIEventHandler oldCapture = _captureEventHandler;
			XMouseEventArgs oldMouseEventArgs = _captureEventArgs;

			_captureEventHandler = eventHandler;
			_captureEventArgs = e;

			if (oldCapture != _captureEventHandler)
				EventsHelper.Fire(NotifyCaptureChanging, this, new MouseCaptureChangingEventArgs(oldCapture, _captureEventHandler, oldMouseEventArgs, _captureEventArgs)); 

		}

		/// <summary>
		/// Releases the capture.  Any object can release the capture, so all UIEventHandlers that take capture should subscribe
		/// to the <see cref="NotifyCaptureChanging"/> event.
		/// </summary>
		public void ReleaseCapture()
		{
			EventsHelper.Fire(NotifyCaptureChanging, this, new MouseCaptureChangingEventArgs(_captureEventHandler, null, _captureEventArgs, null));

			_captureEventHandler = null;
			_captureEventArgs = null;
		}

		public IUIEventHandler GetCapture()
		{
			return _captureEventHandler;
		}

		#endregion

		/// <summary>
		/// The UIEventHandler implementation.  If capture is currently set, these handlers always return true,
		/// otherwise they return false.
		/// </summary>
		#region IUIEventHandler Members

		public bool OnMouseMove(XMouseEventArgs e)
		{
			e.MouseCapture = this;

			if (_captureEventHandler == null)
				return false;

			SetupMouseEventArgs(e);
			_captureEventHandler.OnMouseMove(e);

			return true;
		}

		public bool OnMouseDown(XMouseEventArgs e)
		{
			e.MouseCapture = this; 
			
			if (_captureEventHandler == null)
				return false;

			SetupMouseEventArgs(e);
			_captureEventHandler.OnMouseDown(e);

			return true;
		}

		public bool OnMouseUp(XMouseEventArgs e)
		{
			e.MouseCapture = this; 
			
			if (_captureEventHandler == null)
				return false;

			SetupMouseEventArgs(e);
			_captureEventHandler.OnMouseUp(e);

			return true;
		}

		public bool OnMouseWheel(XMouseEventArgs e)
		{
			e.MouseCapture = this; 
			
			if (_captureEventHandler == null)
				return false;

			SetupMouseEventArgs(e);
			_captureEventHandler.OnMouseWheel(e);

			return true;
		}

		public bool OnKeyDown(XKeyEventArgs e)
		{
			if (_captureEventHandler == null)
				return false;

			_captureEventHandler.OnKeyDown(e);

			return true;
		}

		public bool OnKeyUp(XKeyEventArgs e)
		{
			if (_captureEventHandler == null)
				return false;

			_captureEventHandler.OnKeyUp(e);

			return true;
		}

		#endregion
	}
}
