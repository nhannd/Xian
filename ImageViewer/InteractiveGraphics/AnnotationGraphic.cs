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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// An standard, stateful interactive graphic that consists of some subject of interest graphic
	/// and a <see cref="ICalloutGraphic">text callout</see> that describes the subject.
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
		private ICalloutGraphic _calloutGraphic;

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
		/// Gets the <see cref="ICalloutGraphic"/> associated with the subject of interest..
		/// </summary>
		public ICalloutGraphic Callout
		{
			get { return _calloutGraphic; }
		}

		#region Virtual Members

		/// <summary>
		/// Gets the namespace with which to qualify the action model site of any context menus on this graphic.
		/// </summary>
		/// <remarks>
		/// <para>The default implementation uses the fully qualified name of the <see cref="AnnotationGraphic"/> type as a namespace.</para>
		/// <para>An implementation of <see cref="AnnotationGraphic"/> can override this property to specify that an alternate action model be used instead.</para>
		/// </remarks>
		protected virtual string ContextMenuNamespace
		{
			get { return typeof (AnnotationGraphic).FullName; }
		}

		/// <summary>
		/// Refreshes the annotation graphic by recomputing the callout position and redrawing the graphic.
		/// </summary>
		public virtual void Refresh()
		{
			this.SetCalloutLocation();
			this.Draw();
		}

		/// <summary>
		/// Called by <see cref="AnnotationGraphic"/> to create the <see cref="ICalloutGraphic"/> to be used by this annotation.
		/// </summary>
		/// <remarks>
		/// <para>The default implementation creates a plain <see cref="CalloutGraphic"/> with no text and which is not user-modifiable.</para>
		/// <para>Subclasses can override this method to provide callouts with automatically computed text content or which is user-interactive.</para>
		/// </remarks>
		/// <returns>The <see cref="ICalloutGraphic"/> to be used.</returns>
		protected virtual ICalloutGraphic CreateCalloutGraphic()
		{
			return new CalloutGraphic(string.Empty);
		}

		/// <summary>
		/// Forces a recomputation of the callout line.
		/// </summary>
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

			_calloutGraphic.TextLocationChanged += OnCalloutLocationChanged;

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
														  delegate(IGraphic test) { return test is ICalloutGraphic; }) as ICalloutGraphic;
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
			_calloutGraphic.AnchorPoint = endPoint;
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
				_calloutGraphic.TextLocation = location;
				_calloutGraphic.ResetCoordinateSystem();

				_settingCalloutLocation = false;
			}

			this.ResetCoordinateSystem();

			SetCalloutEndPoint();
		}

		private void OnCalloutLocationChanged(object sender, EventArgs e)
		{
			if (!_settingCalloutLocation)
				_calloutLocationStrategy.OnCalloutLocationChangedExternally();
			SetCalloutEndPoint();
		}

		/// <summary>
		/// Gets a set of exported <see cref="IAction"/>s.
		/// </summary>
		/// <param name="site">The action model site at which the actions should reside.</param>
		/// <param name="mouseInformation">The mouse input when the action model was requested, such as in response to a context menu request.</param>
		/// <returns>A set of exported <see cref="IAction"/>s.</returns>
		public override IActionSet GetExportedActions(string site, IMouseInformation mouseInformation)
		{
			if (_toolSet == null)
				_toolSet = new ToolSet(new GraphicToolExtensionPoint(), new GraphicToolContext(this));
			return base.GetExportedActions(site, mouseInformation).Union(_toolSet.Actions);
		}

		#region IContextMenuProvider Members

		/// <summary>
		/// Gets the context menu <see cref="ActionModelNode"/> based on the current state of the mouse.
		/// </summary>
		public virtual ActionModelNode GetContextMenuModel(IMouseInformation mouseInformation)
		{
			IActionSet actions = this.GetExportedActions("basicgraphic-menu", mouseInformation);
			if (actions == null || actions.Count == 0)
				return null;
			return ActionModelRoot.CreateModel(this.ContextMenuNamespace, "basicgraphic-menu", actions);
		}

		#endregion
	}
}