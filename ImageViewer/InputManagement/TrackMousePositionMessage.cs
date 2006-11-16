using System;
using System.Collections.Generic;
using System.Text;
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

		private TrackMousePositionMessage()
		{ 
		}

		public Point Location
		{
			get { return _location; }
			set { _location = value; }
		}
	}
}
