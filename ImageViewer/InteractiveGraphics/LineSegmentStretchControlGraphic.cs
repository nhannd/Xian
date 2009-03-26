using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public sealed class LineSegmentStretchControlGraphic : ControlPointsGraphic
	{
		[CloneIgnore]
		private bool _bypassControlPointChangedEvent = false;

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

		private void OnSubjectPt1Changed(object sender, PointChangedEventArgs e)
		{
			_bypassControlPointChangedEvent = true;
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.ControlPoints[0] = this.Subject.Pt1;
			}
			finally
			{
				this.ResetCoordinateSystem();
				_bypassControlPointChangedEvent = false;
			}
		}

		private void OnSubjectPt2Changed(object sender, PointChangedEventArgs e)
		{
			_bypassControlPointChangedEvent = true;
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.ControlPoints[1] = this.Subject.Pt2;
			}
			finally
			{
				this.ResetCoordinateSystem();
				_bypassControlPointChangedEvent = false;
			}
		}

		protected override void OnControlPointChanged(int index, PointF point)
		{
			if (!_bypassControlPointChangedEvent)
			{
				if (index == 0)
					this.Subject.Pt1 = point;
				else if (index == 1)
					this.Subject.Pt2 = point;
			}
			base.OnControlPointChanged(index, point);
		}
	}
}