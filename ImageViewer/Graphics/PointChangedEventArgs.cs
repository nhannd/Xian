using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Provides data for "point changed" events.
	/// </summary>
	public class PointChangedEventArgs : EventArgs
	{
		private PointF _point;

		/// <summary>
		/// Initializes a new instance of <see cref="PointChangedEventArgs"/>
		/// with the specified point.
		/// </summary>
		/// <param name="point"></param>
		public PointChangedEventArgs(PointF point)
		{
			_point = point;
		}

		/// <summary>
		/// Gets the point.
		/// </summary>
		public PointF Point
		{
			get { return _point; }
		}
	}
}
