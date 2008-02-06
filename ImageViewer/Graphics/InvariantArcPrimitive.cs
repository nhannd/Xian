using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An arc <see cref="InvariantPrimitive"/>.
	/// </summary>
	public class InvariantArcPrimitive : InvariantBoundablePrimitive, IArcGraphic
	{
		private float _startAngle;
		private float _sweepAngle;

		/// <summary>
		/// Initializes a new instance of <see cref="InvariantArcPrimitive"/>.
		/// </summary>
		public InvariantArcPrimitive()
		{

		}

		/// <summary>
		/// Gets or sets the angle at which the arc begins.
		/// </summary>
		public float StartAngle
		{
			get { return _startAngle; }
			set { _startAngle = value; }
		}

		/// <summary>
		/// Gets or sets the angle that the arc sweeps out.
		/// </summary>
		public float SweepAngle
		{
			get { return _sweepAngle; }
			set { _sweepAngle = value; }
		}

		/// <summary>
		/// Performs a hit test on the <see cref="InvariantArcPrimitive"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns>
		/// <b>True</b> if <paramref name="point"/> "hits" the <see cref="InvariantArcPrimitive"/>,
		/// <b>false</b> otherwise.
		/// </returns>
		/// <remarks>
		/// A "hit" is defined as when the mouse position is <see cref="VectorGraphic.HitTestDistance"/>
		/// screen pixels away from any point on the arc.
		/// </remarks>
		public override bool HitTest(Point point)
		{
			this.CoordinateSystem = CoordinateSystem.Destination;
			bool result = ArcPrimitive.HitTest(point, this.Rectangle, this.StartAngle, this.SweepAngle);
			this.ResetCoordinateSystem();

			return result;
		}
	}
}
