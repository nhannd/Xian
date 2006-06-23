using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ClearCanvas.Workstation.Model
{
	public class RectangleChangedEventArgs : EventArgs
	{
		private RectangleF _rectangle;

		public RectangleChangedEventArgs(RectangleF rectangle)
		{
			_rectangle = rectangle;
		}

		public RectangleF Rectangle
		{
			get { return _rectangle; }
		}
	}
}
