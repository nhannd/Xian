#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;
using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[Cloneable]
	public class ProtractorGraphic : PolylineGraphic
	{
		[CloneIgnore]
		private InvariantArcPrimitive _arc;
		private readonly int _arcRadius = 20;

		protected ProtractorGraphic (ProtractorGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		public ProtractorGraphic() : base()
		{
			base.Graphics.Add(_arc = new InvariantArcPrimitive());
			_arc.Visible = false;
		}

		protected override void OnColorChanged()
		{
			base.OnColorChanged();
			_arc.Color = base.Color;
		}

		public override void OnDrawing()
		{
			base.OnDrawing();
			
			if (this.Points.Count == 3)
			{
				_arc.Visible = IsArcVisible();

				if (_arc.Visible)
					CalculateArc();
			}
		}

		public override bool HitTest(Point point)
		{
			if (_arc.Visible)
			{
				return base.HitTest(point);
			}

			foreach (IGraphic graphic in this.Graphics)
			{
				if (graphic.Visible && graphic.HitTest(point))
					return true;
			}
			return false;
		}

		public override PointF GetClosestPoint(PointF point)
		{
			if (_arc.Visible)
			{
				return base.GetClosestPoint(point);
			}

			foreach (IGraphic graphic in this.Graphics)
			{
				if (graphic.Visible)
					return graphic.GetClosestPoint(point);
			}
			return PointF.Empty;
		}

		private void CalculateArc()
		{
			this.CoordinateSystem = CoordinateSystem.Destination;
			_arc.CoordinateSystem = CoordinateSystem.Destination;

			// The arc center is the vertex of the protractor
			_arc.Location = this.Points[1];

			_arc.InvariantTopLeft = new PointF(-_arcRadius, -_arcRadius);
			_arc.InvariantBottomRight = new PointF(_arcRadius, _arcRadius);

			float startAngle, sweepAngle;
			CalculateAngles(out startAngle, out sweepAngle);
			_arc.StartAngle = startAngle;
			_arc.SweepAngle = sweepAngle;
	
			_arc.ResetCoordinateSystem();
			this.ResetCoordinateSystem();
		}

		private void CalculateAngles(out float startAngle, out float sweepAngle)
		{
			this.CoordinateSystem = CoordinateSystem.Destination;

			sweepAngle = -(float)Vector.SubtendedAngle(
				this.Points[0],
				this.Points[1],
				this.Points[2]);


			// Define a horizontal ray
			PointF zeroDegreePoint = this.Points[1];
			zeroDegreePoint.X += 50;

			startAngle = (float) Vector.SubtendedAngle(
									this.Points[0],
									this.Points[1],
			                      	zeroDegreePoint);

			this.ResetCoordinateSystem();
		}

		private bool IsArcVisible()
		{
			// Arc should only be visible if the arc radius is smaller than both of the
			// two arms of the angle
			this.CoordinateSystem = CoordinateSystem.Destination;
			Vector3D vertexPositionVector = new Vector3D(this.Points[1].X, this.Points[1].Y, 0);
			Vector3D a = new Vector3D(this.Points[0].X, this.Points[0].Y, 0) - vertexPositionVector;
			Vector3D b = new Vector3D(this.Points[2].X, this.Points[2].Y, 0) - vertexPositionVector;
			this.ResetCoordinateSystem();

			return a.Magnitude > _arcRadius && b.Magnitude > _arcRadius;
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_arc = CollectionUtils.SelectFirst(base.Graphics,
				delegate(IGraphic test) { return test is InvariantArcPrimitive; }) as InvariantArcPrimitive;

			Platform.CheckForNullReference(_arc, "_arc");
		}

		public override Roi GetRoi()
		{
			return new ProtractorRoiInfo(this);
		}
	}
}
