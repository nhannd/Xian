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
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public sealed class MoveControlGraphic : ControlGraphic, IMemorable
	{
		[CloneCopyReference]
		private CursorToken _cursor;

		[CloneIgnore]
		private object _stateMemento = null;

		private PointF _anchor = PointF.Empty;

		public MoveControlGraphic(IGraphic subject) : base(subject)
		{
			_cursor = new CursorToken(CursorToken.SystemCursors.SizeAll);
		}

		private MoveControlGraphic(MoveControlGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		public override string CommandName
		{
			get { return SR.CommandMove; }
		}

		/// <summary>
		/// Gets a point that can be used as a landmark to quantify the difference in position of the controlled
		/// graphic when moved by this <see cref="ControlGraphic"/>.
		/// </summary>
		/// <remarks>
		/// <para>This point has no meaning outside of the same instance of a <see cref="MoveControlGraphic"/>.
		/// Specifically, there is no defined relationship between the <see cref="Anchor"/> and the underlying
		/// <see cref="ControlGraphic.Subject"/>. The typical usage is to take the difference between the current
		/// value and a previous reading of the value to determine the position delta between the two readings.</para>
		/// <para>This point is given in either source or destination coordinates depending on the current value
		/// of <see cref="Graphic.CoordinateSystem"/>.</para>
		/// </remarks>
		public PointF Anchor
		{
			get
			{
				if (this.CoordinateSystem == CoordinateSystem.Destination)
					return this.SpatialTransform.ConvertToDestination(_anchor);
				return _anchor;
			}
			private set
			{
				if (this.CoordinateSystem == CoordinateSystem.Destination)
					value = this.SpatialTransform.ConvertToSource(value);
				_anchor = value;
			}
		}

		public CursorToken Cursor
		{
			get { return _cursor; }
			set { _cursor = value; }
		}

		protected override CursorToken OnGetCursorToken(Point point)
		{
			if (this.HitTest(point))
				return this.Cursor;
			return base.OnGetCursorToken(point);
		}

		public override bool HitTest(Point point)
		{
			return base.DecoratedGraphic.HitTest(point);
		}

		public override void Move(SizeF delta)
		{
			this.Anchor += delta;
			this.DecoratedGraphic.Move(delta);
		}

		protected override bool OnMouseStart(IMouseInformation mouseInformation)
		{
			this.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				if (this.HitTest(mouseInformation.Location))
				{
					_stateMemento = this.CreateMemento();
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
				if (base.IsTracking)
				{
					this.Move(Vector.CalculatePositionDelta(base.LastTrackedPosition, mouseInformation.Location));
					this.Draw();
					return true;
				}

				if (this.HitTest(mouseInformation.Location))
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
			if (_stateMemento != null)
			{
				this.AddToCommandHistory(_stateMemento, this.CreateMemento());
				_stateMemento = null;
			}
			return base.OnMouseStop(mouseInformation);
		}

		protected override void OnMouseCancel()
		{
			_stateMemento = null;
			base.OnMouseCancel();
		}

		private void AddToCommandHistory(object beginState, object endState)
		{
			if (beginState != null && endState != null && !beginState.Equals(endState) && base.ImageViewer != null)
			{
				MemorableUndoableCommand memorableCommand = new MemorableUndoableCommand(this);
				memorableCommand.BeginState = beginState;
				memorableCommand.EndState = endState;

				DrawableUndoableCommand command = new DrawableUndoableCommand(this);
				command.Name = this.CommandName;
				command.Enqueue(memorableCommand);

				base.ImageViewer.CommandHistory.AddCommand(command);
			}
		}

		#region IMemorable Members

		public object CreateMemento()
		{
			PointMemento pointMemento;

			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				pointMemento = new PointMemento(this.Anchor);
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			return pointMemento;
		}

		public void SetMemento(object memento)
		{
			PointMemento pointMemento = memento as PointMemento;
			if (pointMemento == null)
				throw new ArgumentException("The provided memento is not the expected type.", "memento");

			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				// figure out what delta is needed to move the anchor to the desired point specified by the memento
				this.Move(Vector.CalculatePositionDelta(this.Anchor, pointMemento.Point));
			}
			finally
			{
				this.ResetCoordinateSystem();
			}
		}

		#endregion
	}
}