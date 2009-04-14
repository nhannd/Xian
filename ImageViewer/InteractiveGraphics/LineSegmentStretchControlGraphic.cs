using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public sealed class LineSegmentStretchControlGraphic : ControlPointsGraphic, IMemorable
	{
		public LineSegmentStretchControlGraphic(IGraphic subject)
			: base(subject)
		{
			Platform.CheckExpectedType(base.Subject, typeof (ILineSegmentGraphic));

			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				base.ControlPoints.Add(this.Subject.Pt1);
				base.ControlPoints.Add(this.Subject.Pt2);
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			Initialize();
		}

		private LineSegmentStretchControlGraphic(LineSegmentStretchControlGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		public new ILineSegmentGraphic Subject
		{
			get { return base.Subject as ILineSegmentGraphic; }
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Initialize();
		}

		private void Initialize()
		{
			this.Subject.Pt1Changed += OnSubjectPt1Changed;
			this.Subject.Pt2Changed += OnSubjectPt2Changed;
		}

		protected override void Dispose(bool disposing)
		{
			this.Subject.Pt1Changed -= OnSubjectPt1Changed;
			this.Subject.Pt2Changed -= OnSubjectPt2Changed;
			base.Dispose(disposing);
		}

		#region IMemorable Members

		public object CreateMemento()
		{
			PointsMemento pointsMemento = new PointsMemento();

			this.Subject.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				pointsMemento.Add(this.Subject.Pt1);
				pointsMemento.Add(this.Subject.Pt2);
			}
			finally
			{
				this.Subject.ResetCoordinateSystem();
			}

			return pointsMemento;
		}

		public void SetMemento(object memento)
		{
			PointsMemento pointsMemento = memento as PointsMemento;
			if (pointsMemento == null || pointsMemento.Count != 2)
				throw new ArgumentException("The provided memento is not the expected type.", "memento");

			this.Subject.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.Subject.Pt1 = pointsMemento[0];
				this.Subject.Pt2 = pointsMemento[1];
			}
			finally
			{
				this.Subject.ResetCoordinateSystem();
			}
		}

		#endregion

		private void OnSubjectPt1Changed(object sender, PointChangedEventArgs e)
		{
			this.SuspendControlPointEvents();
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.ControlPoints[0] = this.Subject.Pt1;
			}
			finally
			{
				this.ResetCoordinateSystem();
				this.ResumeControlPointEvents();
			}
		}

		private void OnSubjectPt2Changed(object sender, PointChangedEventArgs e)
		{
			this.SuspendControlPointEvents();
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.ControlPoints[1] = this.Subject.Pt2;
			}
			finally
			{
				this.ResetCoordinateSystem();
				this.ResumeControlPointEvents();
			}
		}

		protected override void OnControlPointChanged(int index, PointF point)
		{
			if (index == 0)
				this.Subject.Pt1 = point;
			else if (index == 1)
				this.Subject.Pt2 = point;
			base.OnControlPointChanged(index, point);
		}
	}
}