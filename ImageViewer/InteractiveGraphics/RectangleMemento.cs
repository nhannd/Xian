using System.Drawing;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	internal class RectangleMemento : IMemento
	{
		PointF _topLeft;
		PointF _bottomRight;

		public RectangleMemento()
		{
		}

		public PointF TopLeft
		{
			get { return _topLeft; }
			set { _topLeft = value; }
		}
		
		public PointF BottomRight
		{
			get { return _bottomRight; }
			set { _bottomRight = value; }
		}
	}
}
