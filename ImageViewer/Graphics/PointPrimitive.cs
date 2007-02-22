using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class PointPrimitive : VectorGraphic
	{
		private PointF _point;

		public PointPrimitive(PointF point)
		{
			this.Point = point;
		}

		public PointF Point
		{
			get 
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _point;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return base.SpatialTransform.ConvertToDestination(_point);
				}
			}
			set 
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					_point = value;
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					_point = base.SpatialTransform.ConvertToSource(value);
				}
			}
		}

		public override bool HitTest(Point point)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void Move(SizeF delta)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
