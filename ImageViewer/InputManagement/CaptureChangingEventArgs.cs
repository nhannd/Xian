using System;

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

		public IMouseButtonHandler GainingCapture
		{
			get { return _gainingCapture; }
		}
		public IMouseButtonHandler LosingCapture
		{
			get { return _losingCapture; }
		}
	}
}
