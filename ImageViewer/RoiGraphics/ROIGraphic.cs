using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	[Cloneable]
	public class RoiGraphic : AnnotationGraphic 
	{
		private event EventHandler _roiChanged;

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

		private void Initialize() {
			SetTransformValidationPolicy(this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Initialize();
		}

		protected override CalloutGraphic CreateCalloutGraphic() {
			return new RoiCalloutGraphic();
		}

		public new RoiCalloutGraphic Callout
		{
			get { return (RoiCalloutGraphic) base.Callout; }
		}

		/// <summary>
		/// Gets the <see cref="InteractiveGraphic"> defining the region of interest.
		/// </summary>
		/// <remarks>The value of this property is the same as the underlying <see cref="AnnotationGraphic.Subject"/>.</remarks>
		public InteractiveGraphic Roi
		{
			get { return base.Subject; }
		}

		public event EventHandler RoiChanged
		{
			add { _roiChanged += value; }
			remove { _roiChanged -= value; }
		}

		protected sealed override void OnSubjectChanged() {
			this.OnRoiChanged();
			base.OnSubjectChanged();
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
		public void SuspendRoiChangedEvent() {
			_raiseRoiChangedEvent = false;
		}

		/// <summary>
		/// Resumes the raising of the <see cref="RoiChanged"/> event.
		/// </summary>
		/// <param name="raiseEventNow">If <b>true</b>, the <see cref="RoiChanged"/>
		/// event is raised immediately.
		/// </param>
		public void ResumeRoiChangedEvent(bool raiseEventNow) {
			_raiseRoiChangedEvent = true;

			if (raiseEventNow)
				EventsHelper.Fire(_roiChanged, this, EventArgs.Empty);
		}

		private static void SetTransformValidationPolicy(CompositeGraphic compositeGraphic) {
			foreach (IGraphic graphic in compositeGraphic.Graphics) {
				if (graphic is CompositeGraphic)
					SetTransformValidationPolicy(graphic as CompositeGraphic);

				if (!(compositeGraphic.SpatialTransform.ValidationPolicy is RoiTransformPolicy))
					compositeGraphic.SpatialTransform.ValidationPolicy = new RoiTransformPolicy();
			}
		}
	}

	[Cloneable]
	public class RoiCalloutGraphic : CalloutGraphic
	{
		public RoiCalloutGraphic() : base() {}

		protected RoiCalloutGraphic(RoiCalloutGraphic source, ICloningContext context) : base(source,context)
		{
			context.CloneFields(source,this);
		}

		public new string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}
	}
}