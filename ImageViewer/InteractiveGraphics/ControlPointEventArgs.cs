using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Provides data for control point related events.
	/// </summary>
	public class ControlPointEventArgs : CollectionEventArgs<PointF>
	{
		private int _controlPointIndex;

		/// <summary>
		/// 
		/// </summary>
		public ControlPointEventArgs()
		{

		}

		/// <summary>
		/// Initializes a new instance of <see cref="ControlPointEventArgs"/>.
		/// </summary>
		/// <param name="controlPointIndex"></param>
		/// <param name="controlPointLocation"></param>
		public ControlPointEventArgs(int controlPointIndex, PointF controlPointLocation)
		{
			Platform.CheckNonNegative(controlPointIndex, "controlPointIndex");
			Platform.CheckForNullReference(controlPointLocation, "controlPointLocation");

			_controlPointIndex = controlPointIndex;
			base.Item = controlPointLocation;
		}

		/// <summary>
		/// Gets the index of <see cref="ControlPoint"/> in 
		/// <see cref="ControlPointGroup"/>.
		/// </summary>
		public int ControlPointIndex
		{
			get { return _controlPointIndex; }
		}

		/// <summary>
		/// Gets the location of the control point.
		/// </summary>
		public PointF ControlPointLocation
		{
			get { return base.Item; }
		}
	}
}
