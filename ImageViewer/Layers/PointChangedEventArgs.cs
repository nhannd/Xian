using System;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model.Layers
{
	public class PointChangedEventArgs : EventArgs
	{
		private PointF _point;
		private CoordinateSystem _coordinateSystem;

		public PointChangedEventArgs()
		{
		}

		public PointChangedEventArgs(PointF point, CoordinateSystem coordinateSystem)
		{
			_point = point;
		}

		public PointF Point
		{
			get { return _point; }
			set { _point = value; }
		}

		public CoordinateSystem CoordinateSystem
		{
			get { return _coordinateSystem; }
			set { _coordinateSystem = value; }
		}
	}
}
