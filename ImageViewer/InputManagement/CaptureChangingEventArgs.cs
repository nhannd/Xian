using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public class CaptureChangingEventArgs : EventArgs
	{
		private IMouseButtonHandler _gainingCapture;
		private IMouseButtonHandler _losingCapture;

		public CaptureChangingEventArgs(IMouseButtonHandler gainingCapture, IMouseButtonHandler losingCapture)
		{
			_gainingCapture = gainingCapture;
			_losingCapture = losingCapture;
		}

		protected CaptureChangingEventArgs()
		{
		}

		public IMouseButtonHandler GainingCapture
		{
			get { return _gainingCapture; }
			set { _gainingCapture = value; }
		}
		public IMouseButtonHandler LosingCapture
		{
			get { return _losingCapture; }
			set { _losingCapture = value; }
		}
	}
}
