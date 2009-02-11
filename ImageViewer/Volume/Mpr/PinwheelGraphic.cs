using ClearCanvas.ImageViewer.Graphics;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	internal class PinwheelGraphic : CompositeGraphic
	{
		private readonly InvariantLinePrimitive _line1;
		private readonly InvariantLinePrimitive _line2;
		private readonly InvariantEllipsePrimitive _ellipse;
		private PointF _anchor;

		public PinwheelGraphic()
		{
			base.Graphics.Add(_line1 = new InvariantLinePrimitive());
			base.Graphics.Add(_line2 = new InvariantLinePrimitive());
			base.Graphics.Add(_ellipse = new InvariantEllipsePrimitive());

			this.Color = Color.Yellow;
		}

		
		public Color Color
		{
			get { return _line1.Color; }
			set
			{
				_line1.Color = _line2.Color = value;
			}
		}

		public int Rotation
		{
			get { return base.SpatialTransform.RotationXY; }	
			set { base.SpatialTransform.RotationXY = value; }
		}

		public PointF Anchor
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					return _anchor;
				}
				else
				{
					return base.SpatialTransform.ConvertToDestination(_anchor);
				}
			}
			private set
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
				{
					_anchor = value;
				}
				else
				{
					_anchor = base.SpatialTransform.ConvertToSource(value);
				}
			}
		}

		public override bool HitTest(Point point)
		{
			_ellipse.CoordinateSystem = CoordinateSystem.Destination;
			bool hit = _ellipse.Contains(point);
			_ellipse.ResetCoordinateSystem();
			return hit;
		}

		public PointF RotationAnchor
		{
			get { return _ellipse.AnchorPoint; }	
		}

		public override void OnDrawing()
		{
			base.CoordinateSystem = CoordinateSystem.Destination;

			int x = base.ParentPresentationImage.ClientRectangle.Width/2;
			int y = base.ParentPresentationImage.ClientRectangle.Height/2;
			this.Anchor = new PointF(x, y);
			
			base.ResetCoordinateSystem();

			_line1.AnchorPoint = this.Anchor;
			_line1.InvariantTopLeft = new PointF(-7, 0);
			_line1.InvariantBottomRight = new PointF(20, 0);

			_line2.AnchorPoint = this.Anchor;
			_line2.InvariantTopLeft = new PointF(0, -7);
			_line2.InvariantBottomRight = new PointF(0, 7);

			_ellipse.AnchorPoint = _line1.BottomRight;
			_ellipse.InvariantTopLeft = new PointF(-3, -3);
			_ellipse.InvariantBottomRight = new PointF(3, 3);

			base.OnDrawing();
		}
	}
}
