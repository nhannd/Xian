using System;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[Cloneable]
	internal class TextCalloutGraphic : RoiGraphic
	{
		public TextCalloutGraphic() : base(new PointOfInterestInteractiveGraphic())
		{
			this.Callout.Text = "It's turtles all the way down!";
			this.Callout.ShowArrow = true;
		}

		protected TextCalloutGraphic(TextCalloutGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		private new PointOfInterestInteractiveGraphic Roi
		{
			get { return (PointOfInterestInteractiveGraphic) base.Roi; }
		}

		public PointF PointOfInterest
		{
			get { return this.Roi.Location; }
			set { this.Roi.Location = value; }
		}

		public PointF TextLocation
		{
			get { return this.Callout.Location; }
			set { this.Callout.Location = value; }
		}

		[Cloneable]
		private class PointOfInterestInteractiveGraphic : InteractiveGraphic
		{
			public PointOfInterestInteractiveGraphic() : base()
			{
				Initialize();
			}

			protected PointOfInterestInteractiveGraphic(PointOfInterestInteractiveGraphic source, ICloningContext context)
				: base(source, context)
			{
				context.CloneFields(source, this);
				Initialize();
			}

			private void Initialize()
			{
				base.ControlPoints.Add(new PointF(0, 0));
			}

			public PointF Location
			{
				get { return base.ControlPoints[0]; }
				set { base.ControlPoints[0] = value; }
			}

			public override RectangleF BoundingBox
			{
				get { return new RectangleF(this.Location, SizeF.Empty); }
			}

			public override object CreateMemento()
			{
				this.CoordinateSystem = CoordinateSystem.Source;
				try
				{
					return new LocationMemento(this.Location);
				}
				finally
				{
					this.ResetCoordinateSystem();
				}
			}

			public override void SetMemento(object memento)
			{
				LocationMemento pm = (LocationMemento) memento;
				this.CoordinateSystem = CoordinateSystem.Source;
				try
				{
					this.Location = pm.Point;
				}
				finally
				{
					this.ResetCoordinateSystem();
				}
			}

			public override PointF GetClosestPoint(PointF point)
			{
				return this.Location;
			}

			protected override void OnControlPointChanged(object sender, ListEventArgs<PointF> e) {}

			private class LocationMemento : IEquatable<LocationMemento>
			{
				public readonly PointF Point;

				public LocationMemento(PointF point)
				{
					this.Point = point;
				}

				public override int GetHashCode()
				{
					return this.Point.GetHashCode() ^ 0x0BE4AD82;
				}

				public override bool Equals(object obj)
				{
					if (obj is LocationMemento)
						return this.Equals((LocationMemento) obj);
					return false;
				}

				#region IEquatable<PointMemento> Members

				public bool Equals(LocationMemento other)
				{
					if (other == null)
						return false;
					return this.Point == other.Point;
				}

				#endregion
			}
		}
	}
}