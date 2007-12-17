#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A base class graphic that has state and a set of control points
	/// that can be manipulated by the user.
	/// </summary>
	public abstract class InteractiveGraphic
		: StatefulCompositeGraphic, IStandardStatefulGraphic, IMemorable
	{
		private ControlPointGroup _controlPointGroup;
		private CursorToken _stretchToken;
		private ICursorTokenProvider _stretchIndicatorProvider;

		/// <summary>
		/// Initializes a new instance of <see cref="InteractiveGraphic"/>.
		/// </summary>
		/// <param name="userCreated"></param>
		protected InteractiveGraphic(bool userCreated)
		{
			Initialize(userCreated);
		}

		/// <summary>
		/// A group of control points.
		/// </summary>
		public ControlPointGroup ControlPoints
		{
			get 
			{
				if (_controlPointGroup == null)
					_controlPointGroup = new ControlPointGroup();

				return _controlPointGroup; 
			}
		}

		/// <summary>
		/// Gets or sets the colour of the <see cref="InteractiveGraphic"/>.
		/// </summary>
		public virtual Color Color
		{
			get { return _controlPointGroup.Color; }
			set { _controlPointGroup.Color = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="CursorToken"/> that should be shown when stretching
		/// this graphic.
		/// </summary>
		public CursorToken StretchToken
		{
			get { return _stretchToken; }
			set { _stretchToken = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="CursorToken"/> that should be shown to indicate
		/// that the operation performed at a given point will be a stretch operation.
		/// </summary>
		public ICursorTokenProvider StretchIndicatorProvider
		{
			get { return _stretchIndicatorProvider; }
			set { _stretchIndicatorProvider = value; }
		}

		private bool Stretching
		{
			get { return (this.State is MoveControlPointGraphicState || this.State is CreateGraphicState); }
		}

		#region IMemorable Members

		/// <summary>
		/// Captures the state of the <see cref="InteractiveGraphic"/>.
		/// </summary>
		/// <returns></returns>
		public abstract object CreateMemento();

		/// <summary>
		/// Restores the state of the <see cref="InteractiveGraphic"/>.
		/// </summary>
		/// <param name="memento"></param>
		public abstract void SetMemento(object memento);

		#endregion

		/// <summary>
		/// Gets the cursor token to be shown at the current mouse position.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override CursorToken GetCursorToken(Point point)
		{
			CursorToken returnToken = null;

			if (_controlPointGroup.HitTest(point))
			{
				returnToken = this.StretchToken;

				if (!this.Stretching && _stretchIndicatorProvider != null)
				{
					CursorToken indicatorToken = _stretchIndicatorProvider.GetCursorToken(point);
					if (indicatorToken != null)
						returnToken = indicatorToken;
				}
			}

			return returnToken;
		}

		/// <summary>
		/// Invalid operation for <see cref="InteractiveGraphic"/>.
		/// </summary>
		/// <returns></returns>
		public virtual GraphicState CreateCreateState()
		{
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Creates an inactive <see cref="GraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public virtual GraphicState CreateInactiveState()
		{
			return new InactiveGraphicState(this);
		}

		/// <summary>
		/// Creates a focussed <see cref="GraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public virtual GraphicState CreateFocussedState()
		{
			return new FocussedGraphicState(this);
		}

		/// <summary>
		/// Creates a focussed and selected <see cref="GraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public virtual GraphicState CreateFocussedSelectedState()
		{
			return new FocussedSelectedInteractiveGraphicState(this);
		}

		/// <summary>
		/// Creates a selected <see cref="GraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public virtual GraphicState CreateSelectedState()
		{
			return new SelectedGraphicState(this);
		}

		/// <summary>
		/// Executed when a the position of a control point has changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected abstract void OnControlPointChanged(object sender, ListEventArgs<PointF> e);

		/// <summary>
		/// Releases all resources used by this <see cref="InteractiveGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
				_controlPointGroup.ControlPointChangedEvent -= new EventHandler<ListEventArgs<PointF>>(OnControlPointChanged);

			base.Dispose(disposing);
		}

		private void Initialize(bool userCreated)
		{
			if (userCreated)
				base.State = CreateCreateState();
			else
				base.State = CreateInactiveState();

			base.Graphics.Add(this.ControlPoints);

			// Make sure we know when the control points change
			_controlPointGroup.ControlPointChangedEvent += new EventHandler<ListEventArgs<PointF>>(OnControlPointChanged);

			_stretchToken = new CursorToken(CursorToken.SystemCursors.Cross);
			_stretchIndicatorProvider = new CompassStretchIndicatorCursorProvider(_controlPointGroup);
		}
	}
}
