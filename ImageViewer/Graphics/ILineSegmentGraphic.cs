using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines an <see cref="IVectorGraphic"/> that can be described by two points.
	/// </summary>
	public interface ILineSegmentGraphic : IVectorGraphic
	{
		/// <summary>
		/// Gets or sets one endpoint of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF Pt1 { get; set; }

		/// <summary>
		/// Gets or sets the other endpoint of the line in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF Pt2 { get; set; }

		/// <summary>
		/// Occurs when the <see cref="Pt1"/> property changed.
		/// </summary>
		event EventHandler<PointChangedEventArgs> Pt1Changed;

		/// <summary>
		/// Occurs when the <see cref="Pt2"/> property changed.
		/// </summary>
		event EventHandler<PointChangedEventArgs> Pt2Changed;
	}
}