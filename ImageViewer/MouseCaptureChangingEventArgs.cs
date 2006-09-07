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
		private XMouseEventArgs _losingMouseEventArgs;
		private XMouseEventArgs _gainingMouseEventArgs;

		public XMouseEventArgs LosingMouseEventArgs
		{
			get { return _losingMouseEventArgs; }
		}

		public XMouseEventArgs GainingMouseEventArgs
		{
			get { return _gainingMouseEventArgs; }
		}

		public IUIEventHandler UIEventHandlerLosingCapture
		{
			get { return _eventHandlerLosingCapture; }
		}

		public IUIEventHandler UIEventHandlerGainingCapture
		{
			get { return _eventHandlerGainingCapture; }
		}

		public MouseCaptureChangingEventArgs(
			IUIEventHandler eventHandlerLosingCapture,
			IUIEventHandler eventHandlerGainingCapture,
			XMouseEventArgs losingMouseEventArgs,
			XMouseEventArgs gainingMouseEventArgs)
		{
			_eventHandlerLosingCapture = eventHandlerLosingCapture;
			_eventHandlerGainingCapture = eventHandlerGainingCapture;

			_losingMouseEventArgs = losingMouseEventArgs;
			_gainingMouseEventArgs = gainingMouseEventArgs;
		}
	}
}
