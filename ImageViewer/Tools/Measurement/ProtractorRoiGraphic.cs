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

		public ProtractorRoiGraphic()
			: base(new ProtractorInteractiveGraphic(), true)
		{
			_userMovedCallout = false;
			_autoCalculatingCalloutPosition = false;
		}

		private new ProtractorInteractiveGraphic Roi
		{
			get { return (ProtractorInteractiveGraphic) base.Roi; }
		}

		protected override void OnControlPointChanged()
		{
			if (Roi.PolyLine.Count < 3 || _userMovedCallout)
				return;

			base.CoordinateSystem = CoordinateSystem.Destination;

			_autoCalculatingCalloutPosition = true;

			PointF start = Roi.PolyLine[0];
			PointF vertex = Roi.PolyLine[1];
			PointF end = Roi.PolyLine[2];

			double vectorAngle = Formula.SubtendedAngle(start, vertex, end);
			vectorAngle = -vectorAngle / 2 + 180;

			Matrix rotation = new Matrix();
			rotation.Rotate((float)vectorAngle);
			rotation.Translate(-vertex.X, -vertex.Y);

			PointF[] points = new PointF[] {start, end};
			rotation.TransformPoints(points);

			Vector3D startVector = new Vector3D(points[0].X, points[0].Y, 0);
			startVector = startVector / startVector.Magnitude * 30;

			base.Callout.Location = new PointF(startVector.X + vertex.X, startVector.Y + vertex.Y);

			_autoCalculatingCalloutPosition = false;

			base.ResetCoordinateSystem();
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
	}
}