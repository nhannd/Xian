using System;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[Cloneable]
	internal class TextCalloutGraphic : AnnotationGraphic
	{
		public TextCalloutGraphic() : base(new PointOfInterestInteractiveGraphic())
		{
		}

		protected TextCalloutGraphic(TextCalloutGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		protected override CalloutGraphic CreateCalloutGraphic()
		{
			UserCalloutGraphic callout = new UserCalloutGraphic();
			callout.LineStyle = LineStyle.Solid;
			callout.ShowArrow = true;
			return callout;
		}

		private new PointOfInterestInteractiveGraphic Subject
		{
			get { return (PointOfInterestInteractiveGraphic) base.Subject; }
		}

		public new UserCalloutGraphic Callout
		{
			get { return (UserCalloutGraphic) base.Callout; }
		}

		public PointF PointOfInterest
		{
			get { return this.Subject.Location; }
			set { this.Subject.Location = value; }
		}

		public PointF TextLocation
		{
			get { return this.Callout.Location; }
			set { this.Callout.Location = value; }
		}

		public override GraphicState CreateFocussedSelectedState()
		{
			//return base.CreateFocussedSelectedState();
			return new FocussedSelectedTextCalloutGraphicState(this);
		}

		protected class FocussedSelectedTextCalloutGraphicState : FocussedSelectedAnnotationGraphicState
		{
			public FocussedSelectedTextCalloutGraphicState(TextCalloutGraphic annotationGraphic)
				: base(annotationGraphic) {}

			protected new TextCalloutGraphic StatefulGraphic
			{
				get { return (TextCalloutGraphic) base.StatefulGraphic; }
			}

			public override bool Start(IMouseInformation mouseInformation)
			{
				this.StatefulGraphic.CoordinateSystem = CoordinateSystem.Destination;
				try
				{
					PointOfInterestInteractiveGraphic poi = this.StatefulGraphic.Subject;
					UserCalloutGraphic callout = this.StatefulGraphic.Callout;
					RectangleF boundingBox = callout.BoundingBox;

					if ( mouseInformation.ClickCount == 2
						&& boundingBox.Contains(mouseInformation.Location))
					{
						// double click action on the callout text: send into edit text mode
						callout.StartEdit();
					}
					else if (mouseInformation.ClickCount == 1
						&& !poi.HitTest(mouseInformation.Location)
						&& !boundingBox.Contains(mouseInformation.Location)
						&& callout.HitTest(mouseInformation.Location))
					{
						// single click action on the callout line (that is, not the point of interest nor the callout text): move entire graphic
						this.StatefulGraphic.State = new MoveGraphicState(this.StatefulGraphic, this.StatefulGraphic);
						this.StatefulGraphic.State.Start(mouseInformation);

						return true;
					}
				}
				finally
				{
					this.StatefulGraphic.ResetCoordinateSystem();
				}

				if (base.Start(mouseInformation))
					return true;

				return false;
			}
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

			public override void Move(SizeF delta)
			{
				base.ControlPoints[0] += delta;
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