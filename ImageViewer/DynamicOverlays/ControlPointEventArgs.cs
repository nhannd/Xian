using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	/// <summary>
	/// Summary description for ControlPointEventArgs.
	/// </summary>
	public class ControlPointEventArgs : CollectionEventArgs<PointF>
	{
		private int _controlPointIndex;
		
		public ControlPointEventArgs()
		{

		}

		public ControlPointEventArgs(int controlPointIndex, PointF controlPoint)
		{
			Platform.CheckNonNegative(controlPointIndex, "controlPointIndex");
			Platform.CheckForNullReference(controlPoint, "controlPoint");

			_controlPointIndex = controlPointIndex;
			base.Item = controlPoint;
		}

		public int ControlPointIndex
		{
			get { return _controlPointIndex; }
			set { _controlPointIndex = value; }
		}

		public PointF ControlPoint
		{
			get { return base.Item; }
		}
	}
}
