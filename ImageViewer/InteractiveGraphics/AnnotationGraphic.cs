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
using System.ComponentModel;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// An stateful interactive graphic that consists of some subject of interest graphic
	/// and a <see cref="CalloutGraphic">text callout</see> that describes the subject.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="AnnotationGraphic"/> essentially acts as a template for any kind
	/// of interactive graphic defining some object of interest. The subject of interest
	/// can be any graphic primitive such as a <see cref="LinePrimitive">line</see>, a
	/// <see cref="RectanglePrimitive">rectangle</see>, an <see cref="EllipsePrimitive">ellipse</see>,
	/// etc., or some hierarchy of <see cref="ControlGraphic"/>s decorating a primitive graphic.
	/// </para>
	/// <para>
	/// By default, the callout line will snap to the nearest point on the <see cref="ControlGraphic.Subject"/>.
	/// </para>
	/// </remarks>
	[DicomSerializableGraphicAnnotation(typeof (StandardAnnotationGraphicSerializer))]
	[Cloneable]
	public class AnnotationGraphic : StandardStatefulGraphic, IContextMenuProvider
	{
		#region Private fields

		[CloneIgnore]
		private bool _notifyOnSubjectChanged = true;

		[CloneIgnore]
		private CalloutGraphic _calloutGraphic;

		[CloneIgnore]
		private IToolSet _toolSet;

		[CloneIgnore]
		private bool _settingCalloutLocation = false;

		private IAnnotationCalloutLocationStrategy _calloutLocationStrategy;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="AnnotationGraphic"/>.
		/// </summary>
		public AnnotationGraphic(IGraphic subjectGraphic)
			: this(subjectGraphic, null) {}

		/// <summary>
		/// Initializes a new instance of <see cref="AnnotationGraphic"/> with the given <see cref="IAnnotationCalloutLocationStrategy"/>.
		/// </summary>
		public AnnotationGraphic(IGraphic subjectGraphic, IAnnotationCalloutLocationStrategy calloutLocationStrategy)
			: base(subjectGraphic)
		{
			Initialize(calloutLocationStrategy);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected AnnotationGraphic(AnnotationGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		#endregion

		/// <summary>
		/// Gets the <see cref="CalloutGraphic"/>.
		/// </summary>
		public CalloutGraphic Callout
		{
			get { return _calloutGraphic; }
		}

		#region IMemorable Members

		/// <summary>
		/// Creates a memento that can be used to restore the current state.
		/// </summary>
		public virtual object CreateMemento()
		{
			//return new AnnotationGraphicMemento(_subjectGraphic.CreateMemento(), _calloutGraphic.CreateMemento());
			return null;
		}

		/// <summary>
		/// Restores the state of an object.
		/// </summary>
		/// <param name="memento">The object that was
		/// originally created with <see cref="IMemorable.CreateMemento"/>.</param>
		/// <remarks>
		/// The implementation of <see cref="IMemorable.SetMemento"/> should return the 
		/// object to the original state captured by <see cref="IMemorable.CreateMemento"/>.
		/// </remarks>
		public virtual void SetMemento(object memento)
		{
			//AnnotationGraphicMemento annotationMemento = (AnnotationGraphicMemento) memento;
			//_calloutGraphic.SetMemento(annotationMemento.CalloutMemento);
			//_subjectGraphic.SetMemento(annotationMemento.SubjectMemento);
		}

		#endregion

		#region Virtual Members

		protected virtual string ContextMenuNamespace
		{
			get { return typeof (AnnotationGraphic).FullName; }
		}

		public virtual void Refresh()
		{
			this.SetCalloutLocation();
			this.Draw();
		}

		protected virtual CalloutGraphic CreateCalloutGraphic()
		{
			return new CalloutGraphic("XCalloutGraphic");
		}

		protected void RecomputeCalloutLine()
		{
			this.SetCalloutEndPoint();
		}

		#endregion

		private void Initialize(IAnnotationCalloutLocationStrategy calloutLocationStrategy)
		{
			if (_calloutGraphic == null)
			{
				_calloutGraphic = this.CreateCalloutGraphic();
				base.Graphics.Add(_calloutGraphic);
			}

			_calloutGraphic.LocationChanged += new EventHandler<PointChangedEventArgs>(OnCalloutLocationChanged);

			if (_calloutLocationStrategy == null)
				_calloutLocationStrategy = calloutLocationStrategy ?? new AnnotationCalloutLocationStrategy();

			_calloutLocationStrategy.SetAnnotationGraphic(this);

			this.Subject.VisualStateChanged += OnSubjectVisualStateChanged;
		}

		/// <summary>
		/// Releases all resources used by this <see cref="AnnotationGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			this.Subject.VisualStateChanged -= OnSubjectVisualStateChanged;
			base.Dispose(disposing);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_calloutGraphic = CollectionUtils.SelectFirst(base.Graphics,
			                                              delegate(IGraphic test) { return test is CalloutGraphic; }) as CalloutGraphic;
			Platform.CheckForNullReference(_calloutGraphic, "_calloutGraphic");

			this.Initialize(null);

			//the roi and callout may have been selected, so we force a state change
			this.State = this.CreateInactiveState();
		}

		#region Annotation Subject Change Notification

		private void OnSubjectVisualStateChanged(object sender, VisualStateChangedEventArgs e)
		{
			if (_notifyOnSubjectChanged)
			{
				if(e.PropertyName != "Color" && e.PropertyName != "LineStyle")
					this.OnSubjectChanged();
			}
		}

		/// <summary>
		/// Temporarily suspends recomputation of the annotation in response to
		/// property change events on the <see cref="IControlGraphic.Subject"/>.
		/// </summary>
		public void Suspend()
		{
			_notifyOnSubjectChanged = false;
		}

		/// <summary>
		/// Resumes recomputation of the annotation in response to
		/// property change events on the <see cref="IControlGraphic.Subject"/>.
		/// </summary>
		/// <param name="notifyNow">True if the recomputation is to be carried out immediately.</param>
		public void Resume(bool notifyNow)
		{
			_notifyOnSubjectChanged = true;

			if (notifyNow)
				OnSubjectVisualStateChanged(this, new VisualStateChangedEventArgs(this, string.Empty));
		}

		/// <summary>
		/// Called when properties on the <see cref="ControlGraphic.Subject"/> have changed.
		/// </summary>
		protected virtual void OnSubjectChanged()
		{
			SetCalloutLocation();
		}

		#endregion

		private void SetCalloutEndPoint()
		{
			// We're attaching the callout to the ROI, so make sure the two
			// graphics are in the same coordinate system before we do that.
			// This sets all the graphics coordinate systems to be the same.
			this.CoordinateSystem = Subject.CoordinateSystem;

			PointF endPoint;
			CoordinateSystem coordinateSystem;
			_calloutLocationStrategy.CalculateCalloutEndPoint(out endPoint, out coordinateSystem);

			this.ResetCoordinateSystem();

			_calloutGraphic.CoordinateSystem = coordinateSystem;
			_calloutGraphic.EndPoint = endPoint;
			_calloutGraphic.ResetCoordinateSystem();
		}

		private void SetCalloutLocation()
		{
			this.CoordinateSystem = Subject.CoordinateSystem;

			PointF location;
			CoordinateSystem coordinateSystem;
			if (_calloutLocationStrategy.CalculateCalloutLocation(out location, out coordinateSystem))
			{
				_settingCalloutLocation = true;

				_calloutGraphic.CoordinateSystem = coordinateSystem;
				_calloutGraphic.Location = location;
				_calloutGraphic.ResetCoordinateSystem();

				_settingCalloutLocation = false;
			}

			this.ResetCoordinateSystem();

			SetCalloutEndPoint();
		}

		private void OnCalloutLocationChanged(object sender, PointChangedEventArgs e)
		{
			if (!_settingCalloutLocation)
				_calloutLocationStrategy.OnCalloutLocationChangedExternally();
			SetCalloutEndPoint();
		}

		public override IActionSet GetExportedActions(string site, IMouseInformation mouseInformation)
		{
			if (_toolSet == null)
				_toolSet = new ToolSet(new GraphicToolExtensionPoint(), new GraphicToolContext(this));
			return base.GetExportedActions(site, mouseInformation).Union(_toolSet.Actions);
		}

		#region IContextMenuProvider Members

		public ActionModelNode GetContextMenuModel(IMouseInformation mouseInformation)
		{
			IActionSet actions = this.GetExportedActions("basicgraphic-menu", mouseInformation);
			if (actions == null || actions.Count == 0)
				return null;
			return ActionModelRoot.CreateModel(this.ContextMenuNamespace, "basicgraphic-menu", actions);
		}

		#endregion
	}
}