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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Defines an <see cref="IControlGraphic"/> that consists of a number of individually adjustable control points.
	/// </summary>
	public interface IControlPointsGraphic : IControlGraphic
	{
		/// <summary>
		/// Occurs when the location of a control point has changed.
		/// </summary>
		event EventHandler<ListEventArgs<PointF>> ControlPointChangedEvent;

		/// <summary>
		/// Gets the number of control points in the <see cref="IControlGraphic"/>.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Gets or sets the location of the specified control point.
		/// </summary>
		/// <param name="index">The zero-based index of the control point.</param>
		PointF this[int index] { get; set; }

		/// <summary>
		/// Performs a hit test on each <see cref="ControlPoint"/> and returns
		/// the index of the <see cref="ControlPoint"/> for which the test is true.
		/// </summary>
		/// <param name="point">The screen coordinates to test for a control point.</param>
		/// <returns>The index of the control point or -1 if the hit test failed for all control points.</returns>
		int HitTestControlPoint(Point point);
	}

	/// <summary>
	/// Abstract base class for implementations of <see cref="IControlPointsGraphic"/>.
	/// </summary>
	/// <remarks>
	/// The use of the <see cref="ControlPointsGraphic"/> base class is for convenience, as much of the logic behind
	/// the control point graphics, handling user input and command history is implemented by default.
	/// </remarks>
	[Cloneable]
	public abstract partial class ControlPointsGraphic : ControlGraphic, IControlPointsGraphic, IMemorable
	{
		private event EventHandler<ListEventArgs<PointF>> _controlPointChangedEvent;

		[CloneCopyReference]
		private CursorToken _stretchingToken;

		[CloneIgnore]
		private int _trackedControlPoint = -1;

		[CloneIgnore]
		private bool _bypassControlPointChangedEvent = false;

		[CloneIgnore]
		private object _memorableState = null;

		[CloneIgnore]
		private ControlPointGroup _controlPoints;

		private StretchCursorTokenStrategy _stretchCursorTokenStrategy;

		/// <summary>
		/// Constructs a new <see cref="ControlPointsGraphic"/> to control the given subject graphic.
		/// </summary>
		/// <param name="subject">The graphic to control.</param>
		public ControlPointsGraphic(IGraphic subject)
			: base(subject)
		{
			_stretchingToken = new CursorToken(CursorToken.SystemCursors.Cross);

			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected ControlPointsGraphic(ControlPointsGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Occurs when the location of a control point has changed.
		/// </summary>
		public event EventHandler<ListEventArgs<PointF>> ControlPointChangedEvent
		{
			add { _controlPointChangedEvent += value; }
			remove { _controlPointChangedEvent -= value; }
		}

		/// <summary>
		/// Gets or sets the cursor token to use when moving a control point.
		/// </summary>
		public CursorToken StretchingToken
		{
			get { return _stretchingToken; }
			set { _stretchingToken = value; }
		}

		/// <summary>
		/// Gets or sets a strategy for selecting a cursor to show when hovering over a control point.
		/// </summary>
		public StretchCursorTokenStrategy StretchCursorTokenStrategy
		{
			get { return _stretchCursorTokenStrategy; }
			set
			{
				if (_stretchCursorTokenStrategy != value)
				{
					_stretchCursorTokenStrategy = value;

					if (_stretchCursorTokenStrategy != null)
						_stretchCursorTokenStrategy.TargetGraphic = this;
				}
			}
		}

		/// <summary>
		/// Gets the number of control points in the <see cref="IControlGraphic"/>.
		/// </summary>
		public int Count
		{
			get { return _controlPoints.Count; }
		}

		/// <summary>
		/// Gets or sets the location of the specified control point.
		/// </summary>
		/// <param name="index">The zero-based index of the control point.</param>
		public PointF this[int index]
		{
			get { return _controlPoints[index]; }
			set { _controlPoints[index] = value; }
		}

		/// <summary>
		/// Performs a hit test on each <see cref="ControlPoint"/> and returns
		/// the index of the <see cref="ControlPoint"/> for which the test is true.
		/// </summary>
		/// <param name="point">The screen coordinates to test for a control point.</param>
		/// <returns>The index of the control point or -1 if the hit test failed for all control points.</returns>
		public int HitTestControlPoint(Point point)
		{
			return _controlPoints.HitTestControlPoint(point);
		}

		/// <summary>
		/// Gets the collection of control points.
		/// </summary>
		protected ControlPointGroup ControlPoints
		{
			get { return _controlPoints; }
		}

		/// <summary>
		/// Releases all resources used by this <see cref="IControlGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				_controlPoints.ControlPointChangedEvent -= OnControlPointLocationChanged;
			}

			base.Dispose(disposing);
		}

		public override sealed CoordinateSystem CoordinateSystem
		{
			get { return base.CoordinateSystem; }
			set { base.CoordinateSystem = value; }
		}

		public override sealed void ResetCoordinateSystem()
		{
			base.ResetCoordinateSystem();
		}

		/// <summary>
		/// Temporarily suspends notification of control point change events.
		/// </summary>
		protected void SuspendControlPointEvents()
		{
			_bypassControlPointChangedEvent = true;
		}

		/// <summary>
		/// Resumes notification of control point change events.
		/// </summary>
		protected void ResumeControlPointEvents()
		{
			_bypassControlPointChangedEvent = false;
		}

		private void OnControlPointLocationChanged(object sender, ListEventArgs<PointF> e)
		{
			if (!_bypassControlPointChangedEvent)
			{
				this.OnControlPointChanged(e.Index, e.Item);
				EventsHelper.Fire(_controlPointChangedEvent, this, e);
			}
		}

		/// <summary>
		/// Called to notify the derived class of a control point change event.
		/// </summary>
		/// <param name="index">The index of the point that changed.</param>
		/// <param name="point">The value of the point that changed.</param>
		protected virtual void OnControlPointChanged(int index, PointF point) {}

		/// <summary>
		/// Computes a constrained control point location (in destination coordinates) given the location to which the cursor moved after starting drag on the control point.
		/// </summary>
		/// <param name="controlPointIndex">The index of the control point being dragged.</param>
		/// <param name="cursorLocation">The location to which the cursor moved.</param>
		/// <returns>The constrained control point location.</returns>
		/// <remarks>
		/// The default implementation is unconstrained. Subclasses can override this method to constrain the possible locations
		/// of a control point to a particular locus or to maintain some property such as aspect ratio.
		/// </remarks>
		protected virtual PointF ConstrainControlPointLocation(int controlPointIndex, PointF cursorLocation)
		{
			return cursorLocation;
		}

		/// <summary>
		/// Captures the current state of this <see cref="ControlPointsGraphic"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Any overriding implementation of <see cref="ControlPointsGraphic.CreateMemento"/> should return an
		/// object containing enough state information so that, when <see cref="ControlPointsGraphic.SetMemento"/> is called,
		/// the object can be restored to the original state.
		/// </para>
		/// <para>The default implementation returns null.</para>
		/// </remarks>
		public virtual object CreateMemento()
		{
			return null;
		}

		/// <summary>
		/// Restores the state of this <see cref="ControlPointsGraphic"/>.
		/// </summary>
		/// <param name="memento">The object that was originally created with <see cref="ControlPointsGraphic.CreateMemento"/>.</param>
		/// <remarks>
		/// <para>
		/// Any overriding implementation of <see cref="ControlPointsGraphic.SetMemento"/> should return the 
		/// object to the original state captured by <see cref="ControlPointsGraphic.CreateMemento"/>.
		/// </para>
		/// <para>The default implementation does nothing.</para>
		/// </remarks>
		public virtual void SetMemento(object memento) {}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to the framework requesting the cursor token for a particular screen coordinate via <see cref="ControlGraphic.GetCursorToken"/>.
		/// </summary>
		/// <param name="point">The screen coordinate for which the cursor is requested.</param>
		/// <returns></returns>
		protected override CursorToken GetCursorToken(Point point)
		{
			if (this.IsTracking)
				return this.StretchingToken;

			if (this.Visible && this.HitTest(point))
			{
				if (this.StretchCursorTokenStrategy != null)
					return StretchCursorTokenStrategy.GetCursorToken(point);
			}

			return base.GetCursorToken(point);
		}

		private void Initialize()
		{
			if (_stretchCursorTokenStrategy == null)
				_stretchCursorTokenStrategy = new CompassStretchCursorTokenStrategy();

			if (_controlPoints == null)
				base.Graphics.Add( _controlPoints = new ControlPointGroup());

			_stretchCursorTokenStrategy.TargetGraphic = this;
			_controlPoints.ControlPointChangedEvent += OnControlPointLocationChanged;
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_controlPoints = (ControlPointGroup)CollectionUtils.SelectFirst(base.Graphics, delegate(IGraphic graphic) { return graphic is ControlPointGroup; });

			Initialize();
		}

		/// <summary>
		/// Called when the <see cref="ControlGraphic.Color"/> property changes.
		/// </summary>
		protected override void OnColorChanged() 
		{
			_controlPoints.Color = base.Color;
			base.OnColorChanged();
		}

		/// <summary>
		/// Called when the <see cref="ControlGraphic.Show"/> property changes.
		/// </summary>
		protected override void OnShowControlGraphicsChanged()
		{
			_controlPoints.Visible = base.Show;
			base.OnShowControlGraphicsChanged();
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to a mouse button click via <see cref="ControlGraphic.Start"/>.
		/// </summary>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the <see cref="ControlGraphic"/> did something as a result of the call and hence would like to receive capture; False otherwise.</returns>
		protected override bool Start(IMouseInformation mouseInformation)
		{
			this.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				_trackedControlPoint = this.ControlPoints.HitTestControlPoint(mouseInformation.Location);
				if (_trackedControlPoint >= 0)
				{
					_memorableState = this.CreateMemento();
					return true;
				}
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			return base.Start(mouseInformation);
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to the framework tracking mouse input via <see cref="ControlGraphic.Track"/>.
		/// </summary>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the message was handled; False otherwise.</returns>
		protected override bool Track(IMouseInformation mouseInformation)
		{
			this.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				ControlPoint cpGraphic = null;
				if (_trackedControlPoint >= 0 && _trackedControlPoint < this.ControlPoints.Count)
					cpGraphic = (ControlPoint)this.ControlPoints.Graphics[_trackedControlPoint];

				if (base.IsTracking && cpGraphic != null)
				{
					cpGraphic.Location = this.ConstrainControlPointLocation(_trackedControlPoint, mouseInformation.Location);
					cpGraphic.Draw();
					return true;
				}

				if(this.ControlPoints.HitTest(mouseInformation.Location))
				{
					return true;
				}
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			return base.Track(mouseInformation);
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response a mouse button release via <see cref="ControlGraphic.Stop"/>.
		/// </summary>
		/// <param name="mouseInformation">The mouse input information.</param>
		/// <returns>True if the framework should <b>not</b> release capture; False otherwise.</returns>
		protected override bool Stop(IMouseInformation mouseInformation)
		{
			if (_memorableState != null)
			{
				AddToCommandHistory(this, _memorableState, this.CreateMemento());
				_memorableState = null;
			}

			_trackedControlPoint = -1;
			return base.Stop(mouseInformation);
		}

		/// <summary>
		/// Called by <see cref="ControlGraphic"/> in response to an attempt to cancel the current operation via <see cref="ControlGraphic.Cancel"/>.
		/// </summary>
		protected override void Cancel()
		{
			_memorableState = null;
			_trackedControlPoint = -1;
			base.Cancel();
		}
	}
}