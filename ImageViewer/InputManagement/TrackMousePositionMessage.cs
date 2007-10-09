using System.Drawing;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public sealed class TrackMousePositionMessage : IInputMessage
	{
		private Point _location;

		public TrackMousePositionMessage(Point location)
		{
			_location = location;
		}

		public Point Location
		{
			get { return _location; }
			set { _location = value; }
		}
	}
}
