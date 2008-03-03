using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An ellipse <see cref="InvariantPrimitive"/>.
	/// </summary>
	public class InvariantEllipsePrimitive : InvariantBoundablePrimitive
	{
		/// <summary>
		/// Initializes a new instance of <see cref="InvariantEllipsePrimitive"/>.
		/// </summary>
		public InvariantEllipsePrimitive()
		{

		}

		/// <summary>
		/// Performs a hit test on the <see cref="InvariantEllipsePrimitive"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="InvariantEllipsePrimitive"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// A "hit" is defined as when the mouse position is <see cref="VectorGraphic.HitTestDistance"/>
		/// screen pixels away from any point on the arc.
		/// </remarks>
		public override bool HitTest(Point point)
		{
			this.CoordinateSystem = CoordinateSystem.Source;
			bool result = EllipsePrimitive.HitTest(
				this.SpatialTransform.ConvertToSource(point), 
				this.Rectangle,
				this.SpatialTransform);
			this.ResetCoordinateSystem();

			return result;
		}
	}
}
