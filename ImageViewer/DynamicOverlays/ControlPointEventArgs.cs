using System;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	/// <summary>
	/// Summary description for ControlPointEventArgs.
	/// </summary>
	public class ControlPointEventArgs : CollectionEventArgs<PointF>
	{
		private int _ControlPointIndex;
		
		public ControlPointEventArgs()
		{

		}

		public ControlPointEventArgs(int controlPointIndex, PointF controlPoint)
		{
			Platform.CheckNonNegative(controlPointIndex, "controlPointIndex");
			Platform.CheckForNullReference(controlPoint, "controlPoint");

			_ControlPointIndex = controlPointIndex;
			base.Item = controlPoint;
		}

		public int ControlPointIndex
		{
			get { return _ControlPointIndex; }
			set { _ControlPointIndex = value; }
		}

		public PointF ControlPoint
		{
			get { return base.Item; }
		}
	}
}
