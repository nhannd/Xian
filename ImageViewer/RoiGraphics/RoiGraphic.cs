using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.RoiGraphics.Analyzers;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	[Cloneable]
	public class RoiGraphic : AnnotationGraphic
	{
		private event EventHandler _roiChanged;

		[CloneIgnore]
		private Roi _roi;

		[CloneIgnore]
		private DelayedEventPublisher _delayedEventPublisher;

		[CloneIgnore]
		private bool _raiseRoiChangedEvent = true;

		public RoiGraphic(InteractiveGraphic roi) : base(roi)
		{
			Initialize();
		}

		public RoiGraphic(InteractiveGraphic roi, IAnnotationCalloutLocationStrategy calloutLocationStrategy) : base(roi, calloutLocationStrategy)
		{
			Initialize();
		}

		protected RoiGraphic(RoiGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		private void Initialize()
		{
			_delayedEventPublisher = new DelayedEventPublisher(OnDelayedRoiChanged);
			SetTransformValidationPolicy(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_delayedEventPublisher != null)
				{
					_delayedEventPublisher.Cancel();
					_delayedEventPublisher.Dispose();
					_delayedEventPublisher = null;
				}
			}
			base.Dispose(disposing);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Initialize();
			_roi = base.Subject.CreateRoiInformation();
		}

		protected override CalloutGraphic CreateCalloutGraphic()
		{
			return new RoiCalloutGraphic();
		}

		public new RoiCalloutGraphic Callout
		{
			get { return (RoiCalloutGraphic) base.Callout; }
		}

		public Roi Roi
		{
			get { return _roi; }
		}

		public event EventHandler RoiChanged
		{
			add { _roiChanged += value; }
			remove { _roiChanged -= value; }
		}

		public override Roi CreateRoiInformation()
		{
			return base.Subject.CreateRoiInformation();
		}

		protected override void OnNameChanged()
		{
			//TODO: prevent draw before initialization if the next 2 lines are uncommented
			//Analyze(true);
			//this.OnRoiChanged();
			base.OnNameChanged();
		}

		protected override sealed void OnSubjectChanged()
		{
			bool active = this.State is MoveGraphicState ||
			              this.State is MoveControlPointGraphicState ||
			              this.State is CreateGraphicState;

			if (!active)
			{
				// the roi is inactive, focused or selected, but not actively
				// moving or stretching; just do the calculation immediately.
				Analyze(false);
			}
			else
			{
				_delayedEventPublisher.Publish(this.Subject, EventArgs.Empty);
				Analyze(true);
			}

			this.OnRoiChanged();
			base.OnSubjectChanged();
		}

		protected override void OnStateChanged(GraphicStateChangedEventArgs e)
		{
			base.OnStateChanged(e);

			_delayedEventPublisher.PublishNow();
		}

		protected virtual void OnRoiChanged()
		{
			if (_raiseRoiChangedEvent)
				EventsHelper.Fire(_roiChanged, this, EventArgs.Empty);
		}

		/// <summary>
		/// Suspends the raising of the <see cref="RoiChanged"/> event.
		/// </summary>
		/// <remarks>
		/// There are times when it is desirable to suspend the raising of the
		/// <see cref="RoiChanged"/> event, such as when initializing 
		/// control points.  To resume the raising of the event, call
		/// <see cref="ResumeRoiChangedEvent"/>.
		/// </remarks>
		public void SuspendRoiChangedEvent()
		{
			_raiseRoiChangedEvent = false;
		}

		/// <summary>
		/// Resumes the raising of the <see cref="RoiChanged"/> event.
		/// </summary>
		/// <param name="raiseEventNow">If <b>true</b>, the <see cref="RoiChanged"/>
		/// event is raised immediately.
		/// </param>
		public void ResumeRoiChangedEvent(bool raiseEventNow)
		{
			_raiseRoiChangedEvent = true;

			if (raiseEventNow)
				EventsHelper.Fire(_roiChanged, this, EventArgs.Empty);
		}

		private void OnDelayedRoiChanged(object sender, EventArgs e)
		{
			Analyze(false);
			this.Draw();
		}

		private void Analyze(bool responsive)
		{
			_roi = base.Subject.CreateRoiInformation();
			if (_roi != null)
			{
				this.Callout.Update(_roi, responsive ? RoiAnalysisMode.Responsive : RoiAnalysisMode.Normal);
			}
		}

		private static void SetTransformValidationPolicy(CompositeGraphic compositeGraphic)
		{
			foreach (IGraphic graphic in compositeGraphic.Graphics)
			{
				if (graphic is CompositeGraphic)
					SetTransformValidationPolicy(graphic as CompositeGraphic);

				if (!(compositeGraphic.SpatialTransform.ValidationPolicy is RoiTransformPolicy))
					compositeGraphic.SpatialTransform.ValidationPolicy = new RoiTransformPolicy();
			}
		}
	}
}