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
		private int m_AnchorPointIndex;

		public AnchorPointEventArgs()
		{
		}

		public AnchorPointEventArgs(int anchorPointIndex, PointF anchorPoint)
		{
			Platform.CheckNonNegative(anchorPointIndex, "anchorPointIndex");

			m_AnchorPointIndex = anchorPointIndex;
			base.Item = anchorPoint;
		}

		public int AnchorPointIndex
		{
			get { return m_AnchorPointIndex; }
			set { m_AnchorPointIndex = value; }
		}

		public PointF AnchorPoint { get { return base.Item; } }
	}
}
