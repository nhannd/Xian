using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	internal class ProtractorInteractiveGraphic : PolyLineInteractiveGraphic
	{
		private InvariantArcPrimitive _arc;
		private readonly int _arcRadius = 20;

		public ProtractorInteractiveGraphic() : base(true, 3)
		{
			base.Graphics.Add(_arc = new InvariantArcPrimitive());
			_arc.Visible = false;
		}

		public override Color Color
		{
			get
			{
				return base.Color;
			}
			set
			{
				_arc.Color = value;
				base.Color = value;
			}
		}
		protected override void OnControlPointChanged(object sender, ListEventArgs<PointF> e)
		{
			base.OnControlPointChanged(sender, e);

			if (this.PolyLine.Count == 3)
			{
				_arc.Visible = IsArcVisible();
				
				if (_arc.Visible)
					CalculateArc();
			}
		}

		public override bool HitTest(Point point)
		{
			if (_arc.Visible)
				return _arc.HitTest(point) || base.HitTest(point);
			else
				return base.HitTest(point);
		}

		private void CalculateArc()
		{
			this.PolyLine.CoordinateSystem = CoordinateSystem.Destination;
			_arc.CoordinateSystem = CoordinateSystem.Destination;

			// The arc center is the vertex of the protractor
			_arc.AnchorPoint = this.PolyLine[1];

			_arc.InvariantTopLeft = new PointF(-_arcRadius, -_arcRadius);
			_arc.InvariantBottomRight = new PointF(_arcRadius, _arcRadius);

			float startAngle, sweepAngle;
			CalculateAngles(out startAngle, out sweepAngle);
			_arc.StartAngle = startAngle;
			_arc.SweepAngle = sweepAngle;
	
			_arc.ResetCoordinateSystem();
			this.PolyLine.ResetCoordinateSystem();
		}

		private void CalculateAngles(out float startAngle, out float sweepAngle)
		{
			this.PolyLine.CoordinateSystem = CoordinateSystem.Destination;

			sweepAngle = (float)Formula.SubtendedAngle(
				this.PolyLine[0],
				this.PolyLine[1],
				this.PolyLine[2]);


			// Define a horizontal ray
			PointF zeroDegreePoint = this.PolyLine[1];
			zeroDegreePoint.X += 50;

			startAngle = (float) Formula.SubtendedAngle(
			                      	this.PolyLine[0],
			                      	this.PolyLine[1],
			                      	zeroDegreePoint);

			this.PolyLine.ResetCoordinateSystem();
		}

		private bool IsArcVisible()
		{
			// Arc should only be visible if the arc radius is smaller than both of the
			// two arms of the angle
			this.PolyLine.CoordinateSystem = CoordinateSystem.Destination;
			Vector3D vertexPositionVector = new Vector3D(this.PolyLine[1].X, this.PolyLine[1].Y, 0);
			Vector3D a = new Vector3D(this.PolyLine[0].X, this.PolyLine[0].Y, 0) - vertexPositionVector;
			Vector3D b = new Vector3D(this.PolyLine[2].X, this.PolyLine[2].Y, 0) - vertexPositionVector;
			this.PolyLine.ResetCoordinateSystem();

			return a.Magnitude > _arcRadius && b.Magnitude > _arcRadius;
		}
	}
}
