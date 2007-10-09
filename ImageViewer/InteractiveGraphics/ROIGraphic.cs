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
	public class RoiGraphic
		: StatefulCompositeGraphic, 
		  IStandardStatefulGraphic, 
		  ISelectableGraphic, 
		  IFocussableGraphic, 
		  IMemorable, 
		  IContextMenuProvider
    {
        private InteractiveGraphic _roiGraphic;
        private CalloutGraphic _calloutGraphic;
		private ToolSet _toolSet;
		private event EventHandler _roiChangedEvent;
		private bool _selected;
		private bool _focussed;

		public RoiGraphic(InteractiveGraphic graphic, bool userCreated)
			: this(graphic, userCreated, true)
		{
		}

		public RoiGraphic(InteractiveGraphic graphic, bool userCreated, bool installDefaultCursors)
		{
			_roiGraphic = graphic;

			BuildGraphic();

			if (userCreated)
				base.State = CreateCreateState();
			else
				base.State = CreateInactiveState();

			if (installDefaultCursors)
			{
				_roiGraphic.InstallDefaultCursors();
				_calloutGraphic.InstallDefaultCursors();
			}
		}

		public CalloutGraphic Callout
		{
			get { return _calloutGraphic; }
		}

        public InteractiveGraphic Roi
        {
            get { return _roiGraphic; }
        }

		public Color Color
		{
			get { return this.Roi.Color; }
			private set 
			{
				this.Roi.Color = value;
				this.Callout.Color = value;
			}
		}

		public event EventHandler RoiChanged
		{
			add { _roiChangedEvent += value; }
			remove { _roiChangedEvent -= value; }
		}

		public override CursorToken GetCursorToken(Point point)
		{
			CursorToken token = _roiGraphic.GetCursorToken(point);
			if (token == null)
				token = _calloutGraphic.GetCursorToken(point);

			return token;
		}

		public override bool HitTest(Point point)
		{
			return _roiGraphic.HitTest(point) || _calloutGraphic.HitTest(point);
		}


		#region IContextMenuProvider Members

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

		public GraphicState CreateCreateState()
		{
			return new CreateRoiGraphicState(this);
		}

		public GraphicState CreateFocussedSelectedState()
		{
			return new FocussedSelectedRoiGraphicState(this);
		}

		public GraphicState CreateFocussedState()
		{
			return new FocussedGraphicState(this);
		}

		public GraphicState CreateInactiveState()
		{
			return new InactiveGraphicState(this);
		}

		public GraphicState CreateSelectedState()
		{
			return new SelectedGraphicState(this);
		}

		#endregion

		#region ISelectable Members

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

		public IMemento CreateMemento()
		{
			return null;
		}

		public void SetMemento(IMemento memento)
		{
		}

		#endregion

		private void BuildGraphic()
		{
			base.Graphics.Add(_roiGraphic);

			_calloutGraphic = new CalloutGraphic();
			base.Graphics.Add(_calloutGraphic);

			_roiGraphic.ControlPoints.ControlPointChangedEvent += new EventHandler<ControlPointEventArgs>(OnControlPointChanged);
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

		private void OnControlPointChanged(object sender, ControlPointEventArgs e)
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