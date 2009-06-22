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
using ClearCanvas.Common;
using System.Threading;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	/// <summary>
	/// An <see cref="AnnotationGraphic"/> specifically for scenarios where the
	/// subject of interest is an actual region of interest on an image.
	/// </summary>
	[Cloneable]
	public class RoiGraphic : AnnotationGraphic
	{
		private event EventHandler _roiChanged;
		private event EventHandler _nameChanged;

		[CloneIgnore]
		private Roi _roi;

		[CloneIgnore]
		private volatile SynchronizationContext _uiThreadContext;

		//need the lock because structs cannot be volatile.
		[CloneIgnore]
		private readonly object _syncLock = new object();
		[CloneIgnore]
		private DateTime? _lastChange;

		/// <summary>
		/// Constructs a new instance of <see cref="RoiGraphic"/>.
		/// </summary>
		/// <param name="roi">The graphic or control chain graphic representing the region of interest.</param>
		public RoiGraphic(IGraphic roi)
			: base(roi)
		{
			Initialize();
		}

		/// <summary>
		/// Constructs a new instance of <see cref="RoiGraphic"/>.
		/// </summary>
		/// <param name="roi">The graphic or control chain graphic representing the region of interest.</param>
		/// <param name="calloutLocationStrategy">An <see cref="IAnnotationCalloutLocationStrategy"/> to automatically place the callout.</param>
		public RoiGraphic(IGraphic roi, IAnnotationCalloutLocationStrategy calloutLocationStrategy)
			: base(roi, calloutLocationStrategy)
		{
			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected RoiGraphic(RoiGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		private void Initialize()
		{
			SetTransformValidationPolicy(this);
		}

		/// <summary>
		/// Releases all resources used by this <see cref="AnnotationGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_uiThreadContext = null;
				lock (_syncLock) { _lastChange = null; }
			}

			base.Dispose(disposing);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Initialize();
		}

		/// <summary>
		/// Called by <see cref="AnnotationGraphic"/> to create the <see cref="ICalloutGraphic"/> to be used by this annotation.
		/// </summary>
		/// <remarks>
		/// <para>This implementation creates a <see cref="RoiCalloutGraphic"/> which displays values analyzed from the region of interest by a set of <see cref="IRoiAnalyzer"/>s.</para>
		/// </remarks>
		/// <returns>The <see cref="ICalloutGraphic"/> to be used.</returns>
		protected override ICalloutGraphic CreateCalloutGraphic()
		{
			return new RoiCalloutGraphic();
		}

		/// <summary>
		/// Gets the <see cref="RoiCalloutGraphic"/> associated with the region of interest.
		/// </summary>
		public new RoiCalloutGraphic Callout
		{
			get { return (RoiCalloutGraphic)base.Callout; }
		}

		/// <summary>
		/// Gets an object representing the data of the current region of interest.
		/// </summary>
		public Roi Roi
		{
			get
			{
				if (_roi == null)
					_roi = base.Subject.GetRoi();

				return _roi;
			}
		}

		/// <summary>
		/// Occurs when the region of interest changes, thereby changing the data and invalidating any existing, computed statistics.
		/// </summary>
		public event EventHandler RoiChanged
		{
			add { _roiChanged += value; }
			remove { _roiChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="IGraphic.Name"/> of the graphic changes.
		/// </summary>
		public event EventHandler NameChanged
		{
			add { _nameChanged += value; }
			remove { _nameChanged -= value; }
		}

		/// <summary>
		/// Gets an object describing the region of interest on the <see cref="Graphic.ParentPresentationImage"/> selected by the <see cref="DecoratorCompositeGraphic.DecoratedGraphic"/>.
		/// </summary>
		/// <remarks>
		/// Decorated graphics that do not describe a region of interest may return null.
		/// </remarks>
		/// <returns>A <see cref="RoiGraphics.Roi"/> describing this region of interest, or null if the decorated graphic does not describe a region of interest.</returns>
		public override Roi GetRoi()
		{
			return base.Subject.GetRoi();
		}

		/// <summary>
		/// Called when the value of the <see cref="Graphic.Name"/> property changes.
		/// </summary>
		protected override void OnNameChanged()
		{
			//TODO: prevent draw before initialization if the next 2 lines are uncommented
			//Analyze(true);
			//this.OnRoiChanged();
			base.OnNameChanged();
			EventsHelper.Fire(_nameChanged, this, new EventArgs());
		}

		/// <summary>
		/// Refreshes the annotation graphic by recomputing the callout position and redrawing the graphic.
		/// </summary>
		public override void Refresh()
		{
			this.OnSubjectChanged();
			base.Refresh();
		}

		/// <summary>
		/// Called when the value of the <see cref="Graphic.ParentPresentationImage"/> property changes.
		/// </summary>
		/// <param name="oldParentPresentationImage">A reference to the old parent presentation image.</param>
		/// <param name="newParentPresentationImage">A reference to the new parent presentation image.</param>
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

		/// <summary>
		/// Called when properties on the <see cref="ControlGraphic.Subject"/> have changed.
		/// </summary>
		protected override sealed void OnSubjectChanged()
		{
			if (this.DecoratedGraphic is IControlGraphic && SynchronizationContext.Current != null)
			{
				//we can't use the DelayedEventPublisher because that relies on the sync context,
				//and we use graphics on worker threads for avi export ... so, we'll just do it custom.
				lock (_syncLock) { _lastChange = Platform.Time; }

				if (_uiThreadContext == null)
				{
					_uiThreadContext = SynchronizationContext.Current;
					ThreadPool.QueueUserWorkItem(DelayedEventThread);
				}

				Analyze(true);
			}
			else
			{
				// the roi is inactive, focused or selected, but not actively
				// moving or stretching; just do the calculation immediately.
				Analyze(false);
			}

			this.OnRoiChanged();
			base.OnSubjectChanged();
		}

		/// <summary>
		/// Called when the value of <see cref="StatefulCompositeGraphic.State"/> changes.
		/// </summary>
		/// <param name="e">An object containing data describing the specific state change.</param>
		protected override void OnStateChanged(GraphicStateChangedEventArgs e)
		{
			base.OnStateChanged(e);

			OnDelayedRoiChanged();
		}

		/// <summary>
		/// Called when the region of interest changes, thereby changing the data and invalidating any existing, computed statistics.
		/// </summary>
		protected virtual void OnRoiChanged()
		{
			EventsHelper.Fire(_roiChanged, this, EventArgs.Empty);
		}

		private void DelayedEventThread(object nothing)
		{
			SynchronizationContext uiThreadContext;
			DateTime? lastChange;

			while (null != (uiThreadContext = _uiThreadContext))
			{
				lock (_syncLock) { lastChange = _lastChange; }
				if (lastChange == null)
					break;

				TimeSpan timeDiff = Platform.Time.Subtract(lastChange.Value);
				if (timeDiff.Milliseconds >= 300)
				{
					uiThreadContext.Post(delegate { OnDelayedRoiChanged(); }, null);
					break;
				}

				Thread.Sleep(5);
			}
		}

		private void OnDelayedRoiChanged()
		{
			if (_uiThreadContext == null)
				return;

			_uiThreadContext = null;
			_lastChange = null;

			Analyze(false);
			this.Draw();
		}

		private void Analyze(bool responsive)
		{
			_roi = base.Subject.GetRoi();
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

		/// <summary>
		/// Convenience method to create a common elliptical, interactive region of interest.
		/// </summary>
		/// <returns>A new interactive region of interest graphic.</returns>
		public static RoiGraphic CreateEllipse()
		{
			RoiGraphic roiGraphic = new RoiGraphic(new BoundableResizeControlGraphic(new BoundableStretchControlGraphic(new MoveControlGraphic(new EllipsePrimitive()))));
			roiGraphic.State = roiGraphic.CreateInactiveState();
			return roiGraphic;
		}

		/// <summary>
		/// Convenience method to create a common rectangular, interactive region of interest.
		/// </summary>
		/// <returns>A new interactive region of interest graphic.</returns>
		public static RoiGraphic CreateRectangle()
		{
			RoiGraphic roiGraphic = new RoiGraphic(new BoundableResizeControlGraphic(new BoundableStretchControlGraphic(new MoveControlGraphic(new RectanglePrimitive()))));
			roiGraphic.State = roiGraphic.CreateInactiveState();
			return roiGraphic;
		}

		/// <summary>
		/// Convenience method to create a common polygonal, interactive region of interest.
		/// </summary>
		/// <returns>A new interactive region of interest graphic.</returns>
		public static RoiGraphic CreatePolygon()
		{
			RoiGraphic roiGraphic = new RoiGraphic(new PolygonControlGraphic(true, new MoveControlGraphic(new PolylineGraphic(true))));
			roiGraphic.State = roiGraphic.CreateInactiveState();
			return roiGraphic;
		}

		#endregion
	}
}