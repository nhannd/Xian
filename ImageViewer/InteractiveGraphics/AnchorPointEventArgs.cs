using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Provides data for the
	/// <see cref="PolyLineAnchorPointsGraphic.AnchorPointChangedEvent"/>.
	/// </summary>
	public class AnchorPointEventArgs : CollectionEventArgs<PointF>
	{
		private int _anchorPointIndex;

		/// <summary>
		/// 
		/// </summary>
		public AnchorPointEventArgs()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="AnchorPointEventArgs"/>.
		/// </summary>
		/// <param name="anchorPointIndex"></param>
		/// <param name="anchorPoint"></param>
		public AnchorPointEventArgs(int anchorPointIndex, PointF anchorPoint)
		{
			Platform.CheckNonNegative(anchorPointIndex, "anchorPointIndex");

			_anchorPointIndex = anchorPointIndex;
			base.Item = anchorPoint;
		}

		/// <summary>
		/// Gets or sets the array index of the anchor point.
		/// </summary>
		public int AnchorPointIndex
		{
			get { return _anchorPointIndex; }
			set { _anchorPointIndex = value; }
		}

		/// <summary>
		/// Gets the anchor point.
		/// </summary>
		public PointF AnchorPoint { get { return base.Item; } }
	}
}
