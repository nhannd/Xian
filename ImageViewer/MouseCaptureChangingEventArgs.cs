using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Contains data about UIEventHandlers that are losing and/or gaining capture.
	/// </summary>
	public class MouseCaptureChangingEventArgs : EventArgs
	{
		IUIEventHandler _eventHandlerLosingCapture;
		IUIEventHandler _eventHandlerGainingCapture;

		public IUIEventHandler UIEventHandlerLosingCapture
		{
			get { return _eventHandlerLosingCapture; }
		}

		public IUIEventHandler UIEventHandlerGainingCapture
		{
			get { return _eventHandlerGainingCapture; }
		}

		public MouseCaptureChangingEventArgs(IUIEventHandler eventHandlerLosingCapture, IUIEventHandler eventHandlerGainingCapture)
		{
			_eventHandlerLosingCapture = eventHandlerLosingCapture;
			_eventHandlerGainingCapture = eventHandlerGainingCapture;
		}
	}
}
