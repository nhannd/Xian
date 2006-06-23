using System;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public class DrawCustomLayerEventArgs : EventArgs
	{
		private IntPtr m_hDC = IntPtr.Zero;
		private Rectangle m_ClientRectangle = new Rectangle(0, 0, 0, 0);

		public DrawCustomLayerEventArgs(IntPtr hDC, Rectangle clientRectangle)
		{
			Platform.CheckForNullReference(hDC, "hDC");

			m_hDC = hDC;
			m_ClientRectangle = clientRectangle;
		}

		public IntPtr HDC
		{
			get { return m_hDC; }
		}

		public Rectangle ClientRectangle
		{
			get { return m_ClientRectangle; }
		}
	}
}
