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
	public interface IControlPointsGraphic : IControlGraphic
	{
		/// <summary>
		/// Occurs when the location of a <see cref="ControlPoint"/> has changed.
		/// </summary>
		event EventHandler<ListEventArgs<PointF>> ControlPointChangedEvent;

		/// <summary>
		/// Returns the number of <see cref="ControlPoint"/>s in the <see cref="IControlGraphic"/>.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Gets or sets the location of the specified <see cref="ControlPoint"/>.
		/// </summary>
		/// <param name="index">The zero-based index of the <see cref="ControlPoint"/>.</param>
		/// <returns></returns>
		PointF this[int index] { get; set; }

		/// <summary>
		/// Performs a hit test on each <see cref="ControlPoint"/> and returns
		/// the index of the <see cref="ControlPoint"/> for which the test is true.
		/// </summary>
		/// <param name="point"></param>
		/// <returns>The index of the <see cref="ControlPoint"/> or
		/// -1 if the hit test failed for all <see cref="ControlPoint"/>s.</returns>
		int HitTestControlPoint(Point point);
	}

	[Cloneable]
	public abstract class ControlPointsGraphic : ControlGraphic, IControlPointsGraphic, IActiveControlGraphic
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

		public ControlPointsGraphic(IGraphic subject)
			: base(subject)
		{
			_stretchingToken = new CursorToken(CursorToken.SystemCursors.Cross);

			Initialize();
		}

		protected ControlPointsGraphic(ControlPointsGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Occurs when the location of a <see cref="ControlPoint"/> has changed.
		/// </summary>
		public event EventHandler<ListEventArgs<PointF>> ControlPointChangedEvent
		{
			add { _controlPointChangedEvent += value; }
			remove { _controlPointChangedEvent -= value; }
		}

		public CursorToken StretchingToken
		{
			get { return _stretchingToken; }
			set { _stretchingToken = value; }
		}

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
		/// Returns the number of <see cref="ControlPoint"/>s in the <see cref="IControlGraphic"/>.
		/// </summary>
		public int Count
		{
			get { return _controlPoints.Count; }
		}

		/// <summary>
		/// Gets or sets the location of the specified <see cref="ControlPoint"/>.
		/// </summary>
		/// <param name="index">The zero-based index of the <see cref="ControlPoint"/>.</param>
		/// <returns></returns>
		public PointF this[int index]
		{
			get { return _controlPoints[index]; }
			set { _controlPoints[index] = value; }
		}

		public int HitTestControlPoint(Point point)
		{
			return _controlPoints.HitTestControlPoint(point);
		}

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

		protected void SuspendControlPointEvents()
		{
			_bypassControlPointChangedEvent = true;
		}

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

		protected override CursorToken OnGetCursorToken(Point point)
		{
			if (this.IsTracking)
				return this.StretchingToken;

			if (this.Visible && this.HitTest(point))
			{
				if (this.StretchCursorTokenStrategy != null)
					return StretchCursorTokenStrategy.GetCursorToken(point);
			}

			return base.OnGetCursorToken(point);
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

		protected override void OnColorChanged() 
		{
			_controlPoints.Color = base.Color;
			base.OnColorChanged();
		}

		protected override void OnShowControlGraphicsChanged()
		{
			_controlPoints.Visible = base.ShowControlGraphics;
			base.OnShowControlGraphicsChanged();
		}

		protected override bool OnMouseStart(IMouseInformation mouseInformation)
		{
			this.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				_trackedControlPoint = this.ControlPoints.HitTestControlPoint(mouseInformation.Location);
				if (_trackedControlPoint >= 0)
				{
					if(this is IMemorable)
					{
						_memorableState = ((IMemorable) this).CreateMemento();
					}
					return true;
				}
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			return base.OnMouseStart(mouseInformation);
		}

		protected override bool OnMouseTrack(IMouseInformation mouseInformation)
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

			return base.OnMouseTrack(mouseInformation);
		}

		protected override bool OnMouseStop(IMouseInformation mouseInformation)
		{
			if (this is IMemorable && base.ImageViewer != null)
			{
				AddToCommandHistory(_memorableState, ((IMemorable) this).CreateMemento());
				_memorableState = null;
			}

			_trackedControlPoint = -1;
			return base.OnMouseStop(mouseInformation);
		}

		protected override void OnMouseCancel()
		{
			_memorableState = null;
			_trackedControlPoint = -1;
			base.OnMouseCancel();
		}

		protected void AddToCommandHistory(object beginState, object endState)
		{
			if (this is IMemorable && beginState != null && endState != null && !beginState.Equals(endState) && base.ImageViewer != null)
			{
				MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand((IMemorable) this);
				memorableCommand.BeginState = beginState;
				memorableCommand.EndState = endState;

				DrawableUndoableCommand command = new DrawableUndoableCommand(this);
				command.Name = this.CommandName;
				command.Enqueue(memorableCommand);

				base.ImageViewer.CommandHistory.AddCommand(command);
			}
		}
	}
}