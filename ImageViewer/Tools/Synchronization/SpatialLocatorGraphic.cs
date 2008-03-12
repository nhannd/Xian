using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	internal class SpatialLocatorGraphic : CompositeGraphic
	{
		private static readonly int _lineLength = 10;

		private readonly LinePrimitive _line1;
		private readonly LinePrimitive _line2;
		private readonly LinePrimitive _line3;
		private readonly LinePrimitive _line4;

		public PointF _anchor;

		public SpatialLocatorGraphic()
		{
			base.Graphics.Add(_line1 = new LinePrimitive());
			base.Graphics.Add(_line2 = new LinePrimitive());
			base.Graphics.Add(_line3 = new LinePrimitive());
			base.Graphics.Add(_line4 = new LinePrimitive());

			this.Color = Color.LimeGreen;
		}

		public Color Color
		{
			get { return _line1.Color; }
			set
			{
				_line1.Color = _line2.Color = _line3.Color = _line4.Color = value;
			}
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
			set
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

		public override void OnDrawing()
		{
			SetLinePositions();

			base.OnDrawing();
		}

		private void SetLinePositions()
		{
			base.CoordinateSystem = CoordinateSystem.Destination;

			SizeF shift1 = new SizeF(_lineLength + 5F, 0);
			SizeF shift2 = new SizeF(5F, 0);

			PointF anchor = Anchor;

			_line1.Pt1 = PointF.Subtract(anchor, shift1);
			_line1.Pt2 = PointF.Subtract(anchor, shift2);

			_line2.Pt1 = PointF.Add(anchor, shift1);
			_line2.Pt2 = PointF.Add(anchor, shift2);

			shift1 = new SizeF(0, _lineLength + 5F);
			shift2 = new SizeF(0, 5F);

			_line3.Pt1 = PointF.Subtract(anchor, shift1);
			_line3.Pt2 = PointF.Subtract(anchor, shift2);

			_line4.Pt1 = PointF.Add(anchor, shift1);
			_line4.Pt2 = PointF.Add(anchor, shift2);

			base.ResetCoordinateSystem();
		}
	}
}
