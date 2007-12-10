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
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// An interactive graphic that consists of region of interest
	/// of some kind and a callout.
	/// </summary>
	/// <remarks>
	/// <see cref="RoiGraphic"/> essentially acts as a template for any kind
	/// of interactive region of interest.  The type of region of interest
	/// can be any <see cref="InteractiveGraphic"/>, such as a line, a rectangle, 
	/// an ellipse, etc.; it is definable by the
	/// the tool writer via the constructor.  The callout line will snap to the
	/// nearest <see cref="ControlPoint"/> on the <see cref="InteractiveGraphic"/>.
	/// </remarks>
	public class RoiGraphic
		: StatefulCompositeGraphic, 
		  IStandardStatefulGraphic, 
		  ISelectableGraphic, 
		  IFocussableGraphic, 
		  IContextMenuProvider,
		  IMemorable
	{
		#region Private fields

		private InteractiveGraphic _roiGraphic;
        private CalloutGraphic _calloutGraphic;
		private ToolSet _toolSet;
		private event EventHandler _roiChangedEvent;
		private bool _selected;
		private bool _focussed;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="RoiGraphic"/>.
		/// </summary>
		/// <param name="graphic"></param>
		/// <param name="userCreated"></param>
		public RoiGraphic(InteractiveGraphic graphic, bool userCreated)
		{
			_roiGraphic = graphic;

			BuildGraphic();

			if (userCreated)
				base.State = CreateCreateState();
			else
				base.State = CreateInactiveState();
		}

		/// <summary>
		/// Gets the <see cref="CalloutGraphic"/>.
		/// </summary>
		public CalloutGraphic Callout
		{
			get { return _calloutGraphic; }
		}

		/// <summary>
		/// Gets the <see cref="InteractiveGraphic"/> that defines
		/// the ROI.
		/// </summary>
        public InteractiveGraphic Roi
        {
            get { return _roiGraphic; }
        }

		/// <summary>
		/// Gets the colour of the <see cref="RoiGraphic"/>.
		/// </summary>
		public Color Color
		{
			get { return this.Roi.Color; }
			private set 
			{
				this.Roi.Color = value;
				this.Callout.Color = value;
			}
		}

		/// <summary>
		/// Occurs when the size or position of 
		/// <see cref="RoiGraphic.Roi"/> has changed
		/// </summary>
		public event EventHandler RoiChanged
		{
			add { _roiChangedEvent += value; }
			remove { _roiChangedEvent -= value; }
		}

		/// <summary>
		/// Gets the cursor token to be shown at the current mouse position.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override CursorToken GetCursorToken(Point point)
		{
			CursorToken token = _roiGraphic.GetCursorToken(point);
			if (token == null)
				token = _calloutGraphic.GetCursorToken(point);

			return token;
		}

		/// <summary>
		/// Performs a hit test on both the ROI and callout.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override bool HitTest(Point point)
		{
			return _roiGraphic.HitTest(point) || _calloutGraphic.HitTest(point);
		}


		#region IContextMenuProvider Members

		/// <summary>
		/// Gets the context menu <see cref="ActionModelNode"/> based on the current state of the mouse.
		/// </summary>
		public virtual ActionModelNode GetContextMenuModel(IMouseInformation mouseInformation)
		{
			if (!this.HitTest(mouseInformation.Location))
				return null;

			if (_toolSet == null)
				_toolSet = new ToolSet(new GraphicToolExtensionPoint(), new GraphicToolContext(this));

			return ActionModelRoot.CreateModel(this.GetType().FullName, "basicgraphic-menu", _toolSet.Actions);
		}

		#endregion

		#region IStandardStatefulGraphic Members

		/// <summary>
		/// Creates a creation <see cref="GraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public GraphicState CreateCreateState()
		{
			return new CreateRoiGraphicState(this);
		}

		/// <summary>
		/// Creates a focussed and selected <see cref="GraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public GraphicState CreateFocussedSelectedState()
		{
			return new FocussedSelectedRoiGraphicState(this);
		}

		/// <summary>
		/// Creates a focussed <see cref="GraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public GraphicState CreateFocussedState()
		{
			return new FocussedGraphicState(this);
		}

		/// <summary>
		/// Creates an inactive <see cref="GraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public GraphicState CreateInactiveState()
		{
			return new InactiveGraphicState(this);
		}

		/// <summary>
		/// Creates a selected <see cref="GraphicState"/>.
		/// </summary>
		/// <returns></returns>
		public GraphicState CreateSelectedState()
		{
			return new SelectedGraphicState(this);
		}

		#endregion

		#region ISelectable Members

		/// <summary>
		/// Gets or set a value indicating whether the <see cref="RoiGraphic"/> is selected.
		/// </summary>
		public bool Selected
		{
			get
			{
				return _selected;
			}
			set
			{
				if (_selected != value)
				{
					_selected = value;

					if (_selected)
						this.ParentPresentationImage.SelectedGraphic = this;

					if (_focussed)
					{
						if (_selected)
							this.State = CreateFocussedSelectedState();
						else
							this.State = CreateFocussedState();
					}
					else
					{
						if (_selected)
							this.State = CreateSelectedState();
						else
							this.State = CreateInactiveState();
					}
				}
			}
		}

		
		#endregion

		#region IFocussable Members

		/// <summary>
		/// Gets or set a value indicating whether the <see cref="RoiGraphic"/> is in focus.
		/// </summary>
		public bool Focussed
		{
			get
			{
				return _focussed;
			}
			set
			{
				if (_focussed != value)
				{
					_focussed = value;

					if (_focussed)
					{
						this.ParentPresentationImage.FocussedGraphic = this;

						if (this.Selected)
							this.State = CreateFocussedSelectedState();
						else
							this.State = CreateFocussedState();
					}
					else
					{
						if (this.Selected)
							this.State = CreateSelectedState();
						else
							this.State = CreateInactiveState();
					}
				}
			}
		}

		#endregion

		#region IMemorable Members

		// TODO:  This is a total hack.  We've gotten away with these methods
		// doing nothing because they're never actually called.  The problem is that
		// if RoiGraphic is no longer IMemorable, CreateRoiGraphicstate.OnRoiStateChanged
		// will throw an exception, because it tries to create a PositionGraphicCommand
		// where the originator is null, which is not allowed.  The real problem
		// is that we don't properly store a command that recreates an RoiGraphic.

		/// <summary>
		/// Not implemented.
		/// </summary>
		/// <returns></returns>
		public IMemento CreateMemento()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not implemented.
		/// </summary>
		/// <param name="memento"></param>
		public void SetMemento(IMemento memento)
		{
			throw new NotImplementedException();
		}

		#endregion

		private void BuildGraphic()
		{
			base.Graphics.Add(_roiGraphic);

			_calloutGraphic = new CalloutGraphic();
			base.Graphics.Add(_calloutGraphic);

			_roiGraphic.ControlPoints.ControlPointChangedEvent += new EventHandler<CollectionEventArgs<PointF>>(OnControlPointChanged);
			_calloutGraphic.LocationChanged += new EventHandler<PointChangedEventArgs>(OnCalloutTopLeftChanged);

			this.StateChanged += new EventHandler<GraphicStateChangedEventArgs>(OnROIGraphicStateChanged);
		}

		private void OnROIGraphicStateChanged(object sender, GraphicStateChangedEventArgs e)
		{
			if (typeof(InactiveGraphicState).IsAssignableFrom(e.NewState.GetType()))
				EnterInactiveState(e.MouseInformation);
			else if (typeof(FocussedGraphicState).IsAssignableFrom(e.NewState.GetType()))
				EnterFocusState(e.MouseInformation);
			else if (typeof(SelectedGraphicState).IsAssignableFrom(e.NewState.GetType()))
				EnterSelectedState(e.MouseInformation);
			else if (typeof(FocussedSelectedGraphicState).IsAssignableFrom(e.NewState.GetType()))
				EnterFocusSelectedState(e.MouseInformation);
		}

		private void EnterInactiveState(IMouseInformation mouseInformation)
		{
			// If the currently selected graphic is this one,
			// and we're about to go inactive, set the selected graphic
			// to null, indicating that no graphic is currently selected
			if (this.ParentPresentationImage.SelectedGraphic == this)
				this.ParentPresentationImage.SelectedGraphic = null;

			if (this.ParentPresentationImage.FocussedGraphic == this)
				this.ParentPresentationImage.FocussedGraphic = null;

			this.Roi.State = this.Roi.CreateInactiveState();
			this.Callout.State = this.Callout.CreateInactiveState();

			this.Roi.ControlPoints.Visible = false;
			this.Color = Color.Yellow;
			Draw();

			Trace.Write("EnterInactiveState\n");
		}

		private void EnterFocusState(IMouseInformation mouseInformation)
		{
			this.Focussed = true;

			if (this.Roi.HitTest(mouseInformation.Location))
				this.Roi.ControlPoints.Visible = true;
			else
				this.Roi.ControlPoints.Visible = false;

			this.Roi.State = this.Roi.CreateFocussedState();
			this.Callout.State = this.Callout.CreateFocussedState();

			this.Color = Color.Orange;
			Draw();

			Trace.Write("EnterFocusState\n");
		}

		private void EnterSelectedState(IMouseInformation mouseInformation)
		{
			this.Selected = true;

			if (this.ParentPresentationImage.FocussedGraphic == this)
				this.ParentPresentationImage.FocussedGraphic = null;

			//synchronize the states of the child graphics on entering this state so that everything works correctly.
			this.Roi.State = this.Roi.CreateSelectedState();
			this.Callout.State = this.Callout.CreateSelectedState();

			this.Roi.ControlPoints.Visible = true;
			this.Color = Color.Tomato;
			Draw();

			Trace.Write("EnterSelectedState\n");
		}

		private void EnterFocusSelectedState(IMouseInformation mouseInformation)
		{
			this.Selected = true;
			this.Focussed = true;

			//synchronize the states of the child graphics on entering this state so that everything works correctly.
			this.Roi.State = this.Roi.CreateFocussedSelectedState();
			this.Callout.State = this.Callout.CreateFocussedSelectedState();

			this.Roi.ControlPoints.Visible = true;
			this.Color = Color.Tomato;
			Draw();

			Trace.Write("EnterFocusSelectedState\n");
		}

		private void OnControlPointChanged(object sender, CollectionEventArgs<PointF> e)
		{
			// We're attaching the callout to the ROI, so make sure the two
			// graphics are in the same coordinate system before we do that.
			_calloutGraphic.CoordinateSystem = _roiGraphic.CoordinateSystem;
			_calloutGraphic.EndPoint = FindClosestControlPoint();
			_calloutGraphic.ResetCoordinateSystem();
			EventsHelper.Fire(_roiChangedEvent, this, EventArgs.Empty);
		}

		private void OnCalloutTopLeftChanged(object sender, PointChangedEventArgs e)
		{
			_calloutGraphic.CoordinateSystem = _roiGraphic.CoordinateSystem;
			_calloutGraphic.EndPoint = FindClosestControlPoint();
			_calloutGraphic.ResetCoordinateSystem();
		}

		private PointF FindClosestControlPoint()
		{
			double distance;
			double shortest;
			PointF closestPoint;
			PointF calloutStart = _calloutGraphic.StartPoint;

			shortest = double.MaxValue;
			closestPoint = new PointF(float.MaxValue, float.MaxValue);

			for (int i = 0; i < _roiGraphic.ControlPoints.Count; i++)
			{
				PointF controlPoint = _roiGraphic.ControlPoints[i];
				distance = Vector.Distance(controlPoint, calloutStart);

				if (distance < shortest)
				{
					shortest = distance;
					closestPoint = controlPoint;
				}
			}

			return closestPoint;
		}
	}
}