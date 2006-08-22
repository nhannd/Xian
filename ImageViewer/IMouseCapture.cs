using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// The IMouseCapture interface allows for a given UIEventHandler to take ownership
	/// of all mouse messages until capture has been released.
	/// </summary>
	public interface IMouseCapture
	{
		void SetCapture(IUIEventHandler eventHandler, XMouseEventArgs e);
		IUIEventHandler GetCapture();
		void ReleaseCapture();
		event EventHandler<MouseCaptureChangingEventArgs> NotifyCaptureChanging;
	}
}
