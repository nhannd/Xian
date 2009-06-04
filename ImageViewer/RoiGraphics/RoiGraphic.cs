#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.RoiGraphics.Analyzers;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	[Cloneable]
	public class RoiGraphic : AnnotationGraphic
	{
		private event EventHandler _roiChanged;
		private event EventHandler _nameChanged;

		[CloneIgnore]
		private Roi _roi;

		[CloneIgnore]
		private DelayedEventPublisher _delayedEventPublisher;

		public RoiGraphic(IGraphic roi) : base(roi)
		{
			Initialize();
		}

		public RoiGraphic(IGraphic roi, IAnnotationCalloutLocationStrategy calloutLocationStrategy)
			: base(roi, calloutLocationStrategy)
		{
			Initialize();
		}

		protected RoiGraphic(RoiGraphic source, ICloningContext context)
			: base(source, context)
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
			get
			{
				if (_roi == null)
					_roi = base.Subject.CreateRoi();

				return _roi;
			}
		}

		public event EventHandler RoiChanged
		{
			add { _roiChanged += value; }
			remove { _roiChanged -= value; }
		}

		public event EventHandler NameChanged
		{
			add { _nameChanged += value; }
			remove { _nameChanged -= value; }
		}

		public override Roi CreateRoi()
		{
			return base.Subject.CreateRoi();
		}

		protected override void OnNameChanged()
		{
			//TODO: prevent draw before initialization if the next 2 lines are uncommented
			//Analyze(true);
			//this.OnRoiChanged();
			base.OnNameChanged();
			EventsHelper.Fire(_nameChanged, this, new EventArgs());
		}

		public override void Refresh()
		{
			this.OnSubjectChanged();
			base.Refresh();
		}

		protected override void OnParentPresentationImageChanged(IPresentationImage oldParentPresentationImage, IPresentationImage newParentPresentationImage)
		{
			IImageSopProvider sopProvider = oldParentPresentationImage as IImageSopProvider;
			if (sopProvider != null)
				sopProvider.Frame.NormalizedPixelSpacing.Calibrated -= OnNormalizedPixelSpacingCalibrated;

			base.OnParentPresentationImageChanged(oldParentPresentationImage, newParentPresentationImage);

			sopProvider = newParentPresentationImage as IImageSopProvider;
			if (sopProvider != null)
				sopProvider.Frame.NormalizedPixelSpacing.Calibrated += OnNormalizedPixelSpacingCalibrated;
		}

		/// <summary>
		/// Called when the underlying image's pixel spacing is recalibrated.
		/// </summary>
		protected virtual void OnImageCalibrated()
		{
			this.Refresh();
		}

		private void OnNormalizedPixelSpacingCalibrated(object sender, EventArgs e)
		{
			this.OnImageCalibrated();
		}

		protected override sealed void OnSubjectChanged()
		{
			bool active = false;
			if (this.DecoratedGraphic is IControlGraphic)
			{
				active = true; // ((IControlGraphic) this.DecoratedGraphic).CurrentHandler is IActiveControlGraphic;
			}

			//TODO (CR May09):need active?

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
			EventsHelper.Fire(_roiChanged, this, EventArgs.Empty);
		}

		private void OnDelayedRoiChanged(object sender, EventArgs e)
		{
			Analyze(false);
			this.Draw();
		}

		private void Analyze(bool responsive)
		{
			_roi = base.Subject.CreateRoi();
			this.Callout.Update(_roi, responsive ? RoiAnalysisMode.Responsive : RoiAnalysisMode.Normal);
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

		#region Static Helpers

		public static RoiGraphic CreateEllipse()
		{
			RoiGraphic roiGraphic = new RoiGraphic(new BoundableResizeControlGraphic(new BoundableStretchControlGraphic(new MoveControlGraphic(new EllipsePrimitive()))));
			roiGraphic.State = roiGraphic.CreateInactiveState();
			return roiGraphic;
		}

		public static RoiGraphic CreateRectangle()
		{
			RoiGraphic roiGraphic = new RoiGraphic(new BoundableResizeControlGraphic(new BoundableStretchControlGraphic(new MoveControlGraphic(new RectanglePrimitive()))));
			roiGraphic.State = roiGraphic.CreateInactiveState();
			return roiGraphic;
		}

		public static RoiGraphic CreatePolygon()
		{
			RoiGraphic roiGraphic = new RoiGraphic(new PolygonControlGraphic(true, new MoveControlGraphic(new PolylineGraphic(true))));
			roiGraphic.State = roiGraphic.CreateInactiveState();
			return roiGraphic;
		}

		#endregion
	}
}