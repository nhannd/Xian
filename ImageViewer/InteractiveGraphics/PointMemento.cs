using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public class PointMemento : IMemento
	{
		PointF _point;

		public PointMemento()
		{
		}

		public PointMemento(PointF point)
		{
			_point = point;
		}

		public PointF Point
		{
			get { return _point; }
			set { _point = value; }
		}
	}
}
