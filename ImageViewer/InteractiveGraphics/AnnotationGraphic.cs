#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.PresentationStates;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// An stateful interactive graphic that consists of some <see cref="InteractiveGraphic">subject of interest</see>
	/// and a <see cref="CalloutGraphic">text callout</see> that describes the subject.
	/// </summary>
	/// <remarks>
	/// <see cref="AnnotationGraphic"/> essentially acts as a template for any kind
	/// of interactive graphic defining some object of interest.  The type of region of interest
	/// can be any <see cref="InteractiveGraphic"/> such as a line, a rectangle, 
	/// an ellipse, etc.; it is defined by the tool writer via the constructor.  
	/// By default, the callout line will snap to the
	/// nearest point on the <see cref="InteractiveGraphic"/>.
	/// </remarks>
	[DicomSerializableGraphicAnnotation(typeof (StandardAnnotationGraphicSerializer))]
	[Cloneable]
	public class AnnotationGraphic : StandardStatefulInteractiveGraphic, IContextMenuProvider
	{
		#region AnnotationGraphicMemento

		private class AnnotationGraphicMemento : IEquatable<AnnotationGraphicMemento>
		{
			public readonly object SubjectMemento;
			public readonly object CalloutMemento;

			public AnnotationGraphicMemento(object subjectMemento, object calloutMemento)
			{
				SubjectMemento = subjectMemento;
				CalloutMemento = calloutMemento;
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;

				return this.Equals(obj as AnnotationGraphicMemento);
			}

			#region IEquatable<AnnotationGraphicMemento> Members

			public bool Equals(AnnotationGraphicMemento other)
			{
				if (other == null)
					return false;

				return SubjectMemento.Equals(other.SubjectMemento) && CalloutMemento.Equals(other.CalloutMemento);
			}

			#endregion
		}

		#endregion

		#region Private fields

		[CloneIgnore]
		private InteractiveGraphic _subjectGraphic;

		[CloneIgnore]
		private CalloutGraphic _calloutGraphic;

		[CloneIgnore]
		private bool _settingCalloutLocation = false;

		private ToolSet _toolSet;
		private IAnnotationCalloutLocationStrategy _calloutLocationStrategy;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="AnnotationGraphic"/>.
		/// </summary>
		public AnnotationGraphic(InteractiveGraphic subjectGraphic)
			: this(subjectGraphic, null) {}

		/// <summary>
		/// Initializes a new instance of <see cref="AnnotationGraphic"/> with the given <see cref="IAnnotationCalloutLocationStrategy"/>.
		/// </summary>
		public AnnotationGraphic(InteractiveGraphic subjectGraphic, IAnnotationCalloutLocationStrategy calloutLocationStrategy)
			: base(subjectGraphic)
		{
			_subjectGraphic = subjectGraphic;
			Initialize(calloutLocationStrategy);
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected AnnotationGraphic(AnnotationGraphic source, ICloningContext context)
			: base(source, context) { }

		#endregion

		/// <summary>
		/// Gets the <see cref="CalloutGraphic"/>.
		/// </summary>
		public CalloutGraphic Callout
		{
			get { return _calloutGraphic; }
		}

		/// <summary>
		/// Gets the <see cref="InteractiveGraphic"/> that defines
		/// the subject.
		/// </summary>
		public InteractiveGraphic Subject
		{
			get { return _subjectGraphic; }
		}

		#region Overrides

		/// <summary>
		/// Gets the cursor token to be shown at the current mouse position.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override CursorToken GetCursorToken(Point point)
		{
			CursorToken returnToken = base.GetCursorToken(point);
			if (returnToken == null)
			{
				if (_calloutGraphic.HitTest(point))
					returnToken = MoveToken;
			}

			return returnToken;
		}

		/// <summary>
		/// Performs a hit test on both the subject and callout.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override bool HitTest(Point point)
		{
			return _subjectGraphic.HitTest(point) || _calloutGraphic.HitTest(point);
		}

		#endregion

		#region IContextMenuProvider Members

		/// <summary>
		/// Gets the context menu <see cref="ActionModelNode"/> based on the current state of the mouse.
		/// </summary>
		public virtual ActionModelNode GetContextMenuModel(IMouseInformation mouseInformation)
		{
			IGraphic hit;
			if (_subjectGraphic.HitTest(mouseInformation.Location))
				hit = _subjectGraphic;
			else if (_calloutGraphic.HitTest(mouseInformation.Location))
				hit = _calloutGraphic;
			else
				return null;

			if (_toolSet == null)
				_toolSet = new ToolSet(new GraphicToolExtensionPoint(), new GraphicToolContext(this, hit, this.ImageViewer.DesktopWindow));

			return ActionModelRoot.CreateModel(typeof (AnnotationGraphic).FullName, "basicgraphic-menu", _toolSet.Actions);
		}

		#endregion

		#region IMemorable Members

		/// <summary>
		/// Creates a memento that can be used to restore the current state.
		/// </summary>
		public override object CreateMemento()
		{
			return new AnnotationGraphicMemento(_subjectGraphic.CreateMemento(), _calloutGraphic.CreateMemento());
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
		public override void SetMemento(object memento)
		{
			AnnotationGraphicMemento annotationMemento = (AnnotationGraphicMemento) memento;
			_calloutGraphic.SetMemento(annotationMemento.CalloutMemento);
			_subjectGraphic.SetMemento(annotationMemento.SubjectMemento);
		}

		#endregion

		#region Create State Overrides

		/// <summary>
		/// Creates a focussed and selected <see cref="GraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public override GraphicState CreateFocussedSelectedState()
		{
			return new FocussedSelectedAnnotationGraphicState(this);
		}

		protected override void OnEnterCreateState(IMouseInformation mouseInformation)
		{
			_calloutGraphic.Color = FocusSelectedColor;
			SetCalloutControlPointVisibility(false);
			base.OnEnterCreateState(mouseInformation);
		}

		protected override void OnEnterFocusSelectedState(IMouseInformation mouseInformation)
		{
			_calloutGraphic.Color = FocusSelectedColor;
			SetCalloutControlPointVisibility(true);
			base.OnEnterFocusSelectedState(mouseInformation);
		}

		protected override void OnEnterFocusState(IMouseInformation mouseInformation)
		{
			_calloutGraphic.Color = FocusColor;
			SetCalloutControlPointVisibility(true);
			base.OnEnterFocusState(mouseInformation);
		}

		protected override void OnEnterInactiveState(IMouseInformation mouseInformation)
		{
			_calloutGraphic.Color = InactiveColor;
			SetCalloutControlPointVisibility(false);
			base.OnEnterInactiveState(mouseInformation);
		}

		protected override void OnEnterSelectedState(IMouseInformation mouseInformation)
		{
			_calloutGraphic.Color = SelectedColor;
			SetCalloutControlPointVisibility(false);
			base.OnEnterSelectedState(mouseInformation);
		}

		protected class FocussedSelectedAnnotationGraphicState : FocussedSelectedInteractiveGraphicState
		{
			public FocussedSelectedAnnotationGraphicState(AnnotationGraphic annotationGraphic)
				: base(annotationGraphic) {}

			protected AnnotationGraphic StatefulGraphic
			{
				get { return (AnnotationGraphic) base.StatefulGraphic; }
			}

			public override bool Start(IMouseInformation mouseInformation)
			{
				if (base.Start(mouseInformation))
					return true;

				if (this.StatefulGraphic.Callout.HitTest(mouseInformation.Location))
				{
					this.StatefulGraphic.State = new MoveAnnotationCalloutGraphicState(this.StatefulGraphic);
					this.StatefulGraphic.State.Start(mouseInformation);

					return true;
				}

				return false;
			}

			private class MoveAnnotationCalloutGraphicState : MoveGraphicState
			{
				public MoveAnnotationCalloutGraphicState(AnnotationGraphic annotation)
					: base(annotation, annotation.Callout) {}
			}
		}

		#endregion

		#region Protected Virtual Members

		protected virtual void OnSubjectChanged() { }

		protected virtual CalloutGraphic CreateCalloutGraphic()
		{
			return new UserCalloutGraphic();
		}

		#endregion

		private void Initialize(IAnnotationCalloutLocationStrategy calloutLocationStrategy)
		{
			if (!base.Graphics.Contains(_subjectGraphic))
				base.Graphics.Add(_subjectGraphic);

			if (_calloutGraphic == null)
			{
				_calloutGraphic = this.CreateCalloutGraphic();
				base.Graphics.Add(_calloutGraphic);
			}

			_subjectGraphic.ControlPoints.ControlPointChangedEvent += new EventHandler<ListEventArgs<PointF>>(OnControlPointChanged);
			_subjectGraphic.ControlPoints.Graphics.ItemAdded += new EventHandler<ListEventArgs<IGraphic>>(OnControlPointAdded);
			_subjectGraphic.ControlPoints.Graphics.ItemRemoved += new EventHandler<ListEventArgs<IGraphic>>(OnControlPointRemoved);
			_calloutGraphic.LocationChanged += new EventHandler<PointChangedEventArgs>(OnCalloutLocationChanged);

			Subject.ControlPoints.Visible = false;

			if (_calloutLocationStrategy == null)
				_calloutLocationStrategy = calloutLocationStrategy ?? new AnnotationCalloutLocationStrategy();

			_calloutLocationStrategy.SetAnnotationGraphic(this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_subjectGraphic = CollectionUtils.SelectFirst(base.Graphics,
			                                              delegate(IGraphic test) { return test is InteractiveGraphic; }) as InteractiveGraphic;

			_calloutGraphic = CollectionUtils.SelectFirst(base.Graphics,
			                                              delegate(IGraphic test) { return test is CalloutGraphic; }) as CalloutGraphic;

			Platform.CheckForNullReference(_subjectGraphic, "_subjectGraphic");
			Platform.CheckForNullReference(_calloutGraphic, "_calloutGraphic");

			this.Initialize(null);

			//the roi and callout may have been selected, so we force a state change
			this.State = this.CreateInactiveState();
		}

		private void SetCalloutControlPointVisibility(bool show)
		{
			//_calloutGraphic.ControlPoints.Visible = false;
			_calloutGraphic.EnableControlPoint = show;
		}

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

		private void OnControlPointAdded(object sender, ListEventArgs<IGraphic> e)
		{
			SetCalloutLocation();
			OnSubjectChanged();
		}

		private void OnControlPointRemoved(object sender, ListEventArgs<IGraphic> e)
		{
			SetCalloutLocation();
			OnSubjectChanged();
		}

		private void OnControlPointChanged(object sender, ListEventArgs<PointF> e)
		{
			SetCalloutLocation();
			OnSubjectChanged();
		}

		private void OnCalloutLocationChanged(object sender, PointChangedEventArgs e)
		{
			if (!_settingCalloutLocation)
				_calloutLocationStrategy.OnCalloutLocationChangedExternally();
			SetCalloutEndPoint();
		}
	}
}