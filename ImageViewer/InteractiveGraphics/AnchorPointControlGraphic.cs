using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public class AnchorPointControlGraphic : ControlPointsGraphic, IMemorable
	{
		[CloneIgnore]
		private bool _suspendSubjectPointChangeEvents = false;

		public AnchorPointControlGraphic(IGraphic subject)
			: base(subject)
		{
			Platform.CheckExpectedType(base.Subject, typeof (IPointGraphic));

			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				base.ControlPoints.Add(this.Subject.Point);
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			Initialize();
		}

		protected AnchorPointControlGraphic(AnchorPointControlGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		public new IPointGraphic Subject
		{
			get { return base.Subject as IPointGraphic; }
		}

		public override string CommandName
		{
			get { return SR.CommandChange; }
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Initialize();
		}

		private void Initialize()
		{
			this.Subject.PointChanged += OnSubjectPointChanged;
		}

		protected override void Dispose(bool disposing)
		{
			this.Subject.PointChanged -= OnSubjectPointChanged;
			base.Dispose(disposing);
		}

		#region IMemorable Members

		public virtual object CreateMemento()
		{
			this.Subject.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				return new PointMemento(this.Subject.Point);
			}
			finally
			{
				this.Subject.ResetCoordinateSystem();
			}
		}

		public virtual void SetMemento(object memento)
		{
			PointMemento pointMemento = memento as PointMemento;
			if (pointMemento == null)
				throw new ArgumentException("The provided memento is not the expected type.", "memento");

			_suspendSubjectPointChangeEvents = true;
			this.Subject.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.Subject.Point = pointMemento.Point;
			}
			finally
			{
				this.Subject.ResetCoordinateSystem();
				_suspendSubjectPointChangeEvents = false;
				this.OnSubjectPointChanged(this, EventArgs.Empty);
			}
		}

		#endregion

		private void OnSubjectPointChanged(object sender, EventArgs e)
		{
			if (_suspendSubjectPointChangeEvents)
				return;

			this.SuspendControlPointEvents();
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.ControlPoints[0] = this.Subject.Point;
			}
			finally
			{
				this.ResetCoordinateSystem();
				this.ResumeControlPointEvents();
			}
		}

		protected override void OnControlPointChanged(int index, PointF point)
		{
			this.Subject.Point = point;
			base.OnControlPointChanged(index, point);
		}
	}
}