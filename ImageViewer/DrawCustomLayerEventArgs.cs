using System;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public class DrawCustomLayerEventArgs : EventArgs
	{
		private IntPtr _hDC = IntPtr.Zero;
		private Rectangle _clientRectangle = new Rectangle(0, 0, 0, 0);

		public DrawCustomLayerEventArgs(IntPtr hDC, Rectangle clientRectangle)
		{
			Platform.CheckForNullReference(hDC, "hDC");

			_hDC = hDC;
			_clientRectangle = clientRectangle;
		}

		public IntPtr HDC
		{
			get { return _hDC; }
		}

		public Rectangle ClientRectangle
		{
			get { return _clientRectangle; }
		}
	}
}
