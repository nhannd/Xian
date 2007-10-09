using System.Drawing;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public class PointMemento : IMemento
	{
		PointF _point;

		public PointMemento(PointF point)
		{
			_point = point;
		}

		public PointF Point
		{
			get { return _point; }
		}
	}
}
