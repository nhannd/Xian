using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines an <see cref="IVectorGraphic"/> that can be described by a
	/// rectangular bounding box.
	/// </summary>
	/// <remarks>
	/// Rectangles and ellipses are examples of graphics that can be
	/// described by a rectangular bounding box.
	/// </remarks>
	public interface IBoundableGraphic : IVectorGraphic
	{
		/// <summary>
		/// Gets the top left corner of the bounding box in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF TopLeft { get; }

		/// <summary>
		/// Gets the bottom right corner of the bounding box in either source or destination coordinates.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		PointF BottomRight { get; }

		/// <summary>
		/// Gets the width of the bounding box in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination pixels.
		/// </remarks>
		float Width { get; }

		/// <summary>
		/// Gets the height of the bounding box in either source or destination pixels.
		/// </summary>
		/// <remarks>
		/// <see cref="IGraphic.CoordinateSystem"/> determines whether this
		/// property is in source or destination coordinates.
		/// </remarks>
		float Height { get; }
	}
}