using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	internal class ProtractorInteractiveGraphic : PolyLineInteractiveGraphic
	{
		private ArcPrimitive _arc;
		private readonly int _arcRadius = 20;

		public ProtractorInteractiveGraphic() : base(true, 3)
		{

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
			if (this.PolyLine.Count == 3)
			{
				AddArc();

				_arc.Visible = IsArcVisible();
				
				if (_arc.Visible)
					CalculateArc();
			}

			base.OnControlPointChanged(sender, e);
		}

		private void CalculateArc()
		{
			this.PolyLine.CoordinateSystem = CoordinateSystem.Destination;

			// The arc center is the vertex of the protractor
			PointF arcCenter = this.PolyLine[1];
			_arc.CoordinateSystem = CoordinateSystem.Destination;

			_arc.TopLeft = new PointF(arcCenter.X - _arcRadius, arcCenter.Y - _arcRadius);
			_arc.BottomRight = new PointF(arcCenter.X + _arcRadius, arcCenter.Y + _arcRadius);

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

			this.PolyLine.ResetCoordinateSystem();

			// Define a horizontal ray
			PointF zeroDegreePoint = this.PolyLine[1];
			zeroDegreePoint.X += 50;

			startAngle = -(float)Formula.SubtendedAngle(
				this.PolyLine[0],
				this.PolyLine[1],
				zeroDegreePoint);
		}

		private void AddArc()
		{
			if (_arc != null)
				return;

			_arc = new ArcPrimitive();
			base.Graphics.Add(_arc);
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
