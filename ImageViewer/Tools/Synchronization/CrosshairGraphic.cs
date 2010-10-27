#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Drawing;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	internal class CrosshairGraphic : CompositeGraphic
	{
		private static readonly int _lineLength = 10;

		private readonly LinePrimitive _line1;
		private readonly LinePrimitive _line2;
		private readonly LinePrimitive _line3;
		private readonly LinePrimitive _line4;

		public PointF _anchor;

		public CrosshairGraphic()
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
			SetCrossHairLines();

			base.OnDrawing();
		}

		private void SetCrossHairLines()
		{
			base.CoordinateSystem = CoordinateSystem.Destination;

			SizeF offset1 = new SizeF(_lineLength + 5F, 0);
			SizeF offset2 = new SizeF(5F, 0);

			PointF anchor = Anchor;

			_line1.Point1 = PointF.Subtract(anchor, offset1);
			_line1.Point2 = PointF.Subtract(anchor, offset2);

			_line2.Point1 = PointF.Add(anchor, offset1);
			_line2.Point2 = PointF.Add(anchor, offset2);

			offset1 = new SizeF(0, _lineLength + 5F);
			offset2 = new SizeF(0, 5F);

			_line3.Point1 = PointF.Subtract(anchor, offset1);
			_line3.Point2 = PointF.Subtract(anchor, offset2);

			_line4.Point1 = PointF.Add(anchor, offset1);
			_line4.Point2 = PointF.Add(anchor, offset2);

			base.ResetCoordinateSystem();
		}
	}
}
