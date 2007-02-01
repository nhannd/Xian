using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class InvariantTextPrimitive : InvariantPrimitive
	{
		private string _text;
		private float _sizeInPoints;
		private string _font;
		private SizeF _dimensions;
		private event EventHandler<RectangleChangedEventArgs> _boundingBoxChangedEvent;

		public InvariantTextPrimitive()
		{
			this.SizeInPoints = 10;
			this.Font = "Arial";
		}

		public InvariantTextPrimitive(string text) : this()
		{
			_text = text;
		}

		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}

		public float SizeInPoints
		{
			get { return _sizeInPoints; }
			set { _sizeInPoints = value; }
		}

		public string Font
		{
			get { return _font; }
			set { _font = value; }
		}

		public SizeF Dimensions
		{
			get 
			{
				if (this.CoordinateSystem == CoordinateSystem.Source)
				{
					return _dimensions;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					return this.SpatialTransform.ConvertToDestination(_dimensions);
				}
			}
			set 
			{
				//if (this.Dimensions == value)
				//	return;

				if (this.CoordinateSystem == CoordinateSystem.Source)
				{
					_dimensions = value;
				}
				else
				{
					Platform.CheckMemberIsSet(base.SpatialTransform, "SpatialTransform");
					_dimensions = this.SpatialTransform.ConvertToSource(value);
				}

				EventsHelper.Fire(_boundingBoxChangedEvent, this, new RectangleChangedEventArgs(this.BoundingBox));
			}
		}

		public RectangleF BoundingBox
		{
			get
			{
				return new RectangleF(this.AnchorPoint, this.Dimensions);
			}
		}

		public event EventHandler<RectangleChangedEventArgs> BoundingBoxChanged
		{
			add { _boundingBoxChangedEvent += value; }
			remove { _boundingBoxChangedEvent -= value; }
		}

		public override bool HitTest(Point point)
		{
			PointF pointF = new PointF(point.X, point.Y);

			this.CoordinateSystem = CoordinateSystem.Destination;

			bool hit = this.BoundingBox.Contains(pointF);

			this.ResetCoordinateSystem();

			return hit;
		}
	}
}
