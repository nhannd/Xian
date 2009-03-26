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
	/// <see cref="XAnnotationGraphic"/> essentially acts as a template for any kind
	/// of interactive graphic defining some object of interest.  The type of region of interest
	/// can be any <see cref="InteractiveGraphic"/> such as a line, a rectangle, 
	/// an ellipse, etc.; it is defined by the tool writer via the constructor.  
	/// By default, the callout line will snap to the
	/// nearest point on the <see cref="InteractiveGraphic"/>.
	/// </remarks>
	[DicomSerializableGraphicAnnotation(typeof (StandardAnnotationGraphicSerializer))]
	[Cloneable]
	public class AnnotationGraphic : StandardStatefulGraphic
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
		private CalloutGraphic _calloutGraphic;

		[CloneIgnore]
		private bool _settingCalloutLocation = false;

		private IAnnotationCalloutLocationStrategy _calloutLocationStrategy;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="XAnnotationGraphic"/>.
		/// </summary>
		public AnnotationGraphic(IGraphic subjectGraphic)
			: this(subjectGraphic, null) {}

		/// <summary>
		/// Initializes a new instance of <see cref="XAnnotationGraphic"/> with the given <see cref="IXAnnotationCalloutLocationStrategy"/>.
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

		public virtual void Refresh()
		{
			SetCalloutLocation();
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

		protected override void OnSubjectChanged()
		{
			SetCalloutLocation();
			base.OnSubjectChanged();
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

		private void OnCalloutLocationChanged(object sender, PointChangedEventArgs e)
		{
			if (!_settingCalloutLocation)
				_calloutLocationStrategy.OnCalloutLocationChangedExternally();
			SetCalloutEndPoint();
		}

		protected override void OnStateChanged(GraphicStateChangedEventArgs e)
		{
			base.OnStateChanged(e);
		}
	}
}