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
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public abstract class InteractiveGraphic
		: StatefulCompositeGraphic, IStandardStatefulGraphic, IMemorable
	{
		private ControlPointGroup _controlPointGroup;
		private CursorToken _stretchToken;
		private ICursorTokenProvider _stretchIndicatorProvider;

		protected InteractiveGraphic(bool userCreated)
		{
			Initialize();

			if (userCreated)
				base.State = CreateCreateState();
			else
				base.State = CreateInactiveState();
		}

		public ControlPointGroup ControlPoints
		{
			get 
			{
				if (_controlPointGroup == null)
					_controlPointGroup = new ControlPointGroup();

				return _controlPointGroup; 
			}
		}

		public CursorToken StretchToken
		{
			get { return _stretchToken; }
			protected set { _stretchToken = value; }
		}

		protected ICursorTokenProvider StretchIndicatorProvider
		{
			get { return _stretchIndicatorProvider; }
			set { _stretchIndicatorProvider = value; }
		}

		protected virtual bool Stretching
		{
			get { return (this.State is MoveControlPointGraphicState || this.State is CreateGraphicState); }
		}

		public virtual Color Color
		{
			get { return _controlPointGroup.Color; }
			set { _controlPointGroup.Color = value; }
		}

		#region IMemorable Members

		public abstract IMemento CreateMemento();

		public abstract void SetMemento(IMemento memento);

		#endregion

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

		public override void InstallDefaultCursors()
		{
			base.InstallDefaultCursors();

			_stretchToken = new CursorToken(CursorToken.SystemCursors.Cross);
			_stretchIndicatorProvider = new CompassStretchIndicatorCursorProvider(_controlPointGroup);
		}

		public virtual GraphicState CreateCreateState()
		{
			throw new InvalidOperationException();
		}

		public virtual GraphicState CreateInactiveState()
		{
			return new InactiveGraphicState(this);
		}

		public virtual GraphicState CreateFocussedState()
		{
			return new FocussedGraphicState(this);
		}

		public virtual GraphicState CreateFocussedSelectedState()
		{
			return new FocussedSelectedInteractiveGraphicState(this);
		}

		public virtual GraphicState CreateSelectedState()
		{
			return new SelectedGraphicState(this);
		}

		protected abstract void OnControlPointChanged(object sender, ControlPointEventArgs e);
	
		private void Initialize()
		{
			base.Graphics.Add(this.ControlPoints);

			// Make sure we know when the control points change
			_controlPointGroup.ControlPointChangedEvent += new EventHandler<ControlPointEventArgs>(OnControlPointChanged);
		}
	}
}
