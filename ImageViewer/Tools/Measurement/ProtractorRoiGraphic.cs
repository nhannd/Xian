using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using Matrix=System.Drawing.Drawing2D.Matrix;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	internal class ProtractorRoiGraphic : RoiGraphic
	{
		private bool _userMovedCallout;
		private bool _autoCalculatingCalloutPosition;
		private bool _firstCalloutCalculation;
		private PointF _lastVertexLocationSource;

		public ProtractorRoiGraphic()
			: base(new ProtractorInteractiveGraphic(), true)
		{
			_userMovedCallout = false;
			_autoCalculatingCalloutPosition = false;
			_firstCalloutCalculation = true;
		}

		private new ProtractorInteractiveGraphic Roi
		{
			get { return (ProtractorInteractiveGraphic) base.Roi; }
		}

		public override void OnDrawing()
		{
			SetCalloutLocation();
			base.OnDrawing();
		}

		protected override void OnCalloutLocationChanged()
		{
			if (_autoCalculatingCalloutPosition || base.State is CreateGraphicState)
				return;

			_userMovedCallout = true;
		}

		protected override PointF CalculateCalloutEndPoint()
		{
			if (_userMovedCallout)
				return base.CalculateCalloutEndPoint();

			if (Roi.PolyLine.Count < 3)
			{
				base.Callout.Visible = false;
				return Roi.PolyLine[0];
			}

			base.Callout.Visible = true;
			return Roi.PolyLine[1];
		}

		private void SetCalloutLocation()
		{
			if (!base.Callout.Visible || _userMovedCallout)
				return;

			_autoCalculatingCalloutPosition = true;

			base.CoordinateSystem = CoordinateSystem.Source;

			SizeF calloutOffsetDestination = SizeF.Empty;
			if (!_firstCalloutCalculation && _lastVertexLocationSource != Roi.PolyLine[1])
			{
				PointF currentVertexLocationSource = Roi.PolyLine[1];

				PointF currentVertexLocationDestination = Roi.PolyLine.SpatialTransform.ConvertToDestination(currentVertexLocationSource);
				PointF lastVertexLocationDestination = Roi.PolyLine.SpatialTransform.ConvertToDestination(_lastVertexLocationSource);

				calloutOffsetDestination = new SizeF(
					currentVertexLocationDestination.X - lastVertexLocationDestination.X, 
					currentVertexLocationDestination.Y - lastVertexLocationDestination.Y);
			}

			_lastVertexLocationSource = Roi.PolyLine[1];

			base.ResetCoordinateSystem();

			base.CoordinateSystem = CoordinateSystem.Destination;

			// first, move the callout by the same amount the vertex moved
			base.Callout.Location += calloutOffsetDestination;

			PointF start = Roi.PolyLine[0];
			PointF vertex = Roi.PolyLine[1];
			PointF end = Roi.PolyLine[2];

			double vectorAngle = Formula.SubtendedAngle(start, vertex, end);
			vectorAngle = -vectorAngle / 2 + 180;

			Matrix rotation = new Matrix();
			rotation.Rotate((float)vectorAngle);
			rotation.Translate(-vertex.X, -vertex.Y);

			PointF[] points = new PointF[] { start, end };
			rotation.TransformPoints(points);

			float calloutMagnitude;
			if (_firstCalloutCalculation)
			{
				calloutMagnitude = 30F;
				_firstCalloutCalculation = false;
			}
			else
			{
				calloutMagnitude = new Vector3D(base.Callout.Location.X - vertex.X, 
					base.Callout.Location.Y - vertex.Y, 0).Magnitude;
			}

			Vector3D startVector = new Vector3D(points[0].X, points[0].Y, 0);
			if (FloatComparer.AreEqual(startVector.Magnitude, 0F, 0.01F))
				startVector = new Vector3D(-1, 0, 0);
			
			startVector = startVector / startVector.Magnitude * calloutMagnitude;

			base.Callout.Location = new PointF(startVector.X + vertex.X, startVector.Y + vertex.Y);

			_autoCalculatingCalloutPosition = false;

			base.ResetCoordinateSystem();
		}
	}
}