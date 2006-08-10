using System;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	/// <summary>
	/// Summary description for AnchorPointEventArgs.
	/// </summary>
	public class AnchorPointEventArgs : CollectionEventArgs<PointF>
	{
		private int _AnchorPointIndex;

		public AnchorPointEventArgs()
		{
		}

		public AnchorPointEventArgs(int anchorPointIndex, PointF anchorPoint)
		{
			Platform.CheckNonNegative(anchorPointIndex, "anchorPointIndex");

			_AnchorPointIndex = anchorPointIndex;
			base.Item = anchorPoint;
		}

		public int AnchorPointIndex
		{
			get { return _AnchorPointIndex; }
			set { _AnchorPointIndex = value; }
		}

		public PointF AnchorPoint { get { return base.Item; } }
	}
}
