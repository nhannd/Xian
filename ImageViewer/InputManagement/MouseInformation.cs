using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ClearCanvas.ImageViewer.InputManagement
{
	public sealed class MouseInformation
	{
		private ITile _tile;
		private Point _point;

		public MouseInformation(ITile tile, Point point)
		{
			_tile = tile;
			_point = point;
		}

		private MouseInformation()
		{ 
		}

		public ITile Tile
		{
			get { return _tile; }
		}

		public Point Point
		{
			get { return _point; }
		}
	}
}
