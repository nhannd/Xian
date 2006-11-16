using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Layers;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.DynamicOverlays
{
	public abstract class InteractiveGraphic : StatefulGraphic, IMemorable
	{
		public static readonly int _hitTestDistance = 10;

		private ControlPointsGraphic _controlPoints = new ControlPointsGraphic();
		private CursorToken _stretchToken;

		protected InteractiveGraphic(bool userCreated)
		{
			Initialize();

			if (userCreated)
				base.State = CreateCreateState();
			else
				base.State = CreateInactiveState();
		}

		public ControlPointsGraphic ControlPoints
		{
			get { return _controlPoints; }
		}

		public CursorToken StretchToken
		{
			get { return _stretchToken; }
			set { _stretchToken = value; }
		}

		#region IMemorable Members

		public abstract IMemento CreateMemento();

		public abstract void SetMemento(IMemento memento);

		#endregion

		public override bool SetCursorToken(MouseInformation pointerInformation)
		{
			if (this._controlPoints.HitTest(pointerInformation.Point))
			{
				pointerInformation.Tile.CursorToken = this.StretchToken;
				return true;
			}

			return false;
		}

		public override GraphicState CreateFocusSelectedState()
		{
			return new FocusSelectedInteractiveGraphicState(this);
		}

		public override void OnEnterInactiveState(MouseInformation pointerInformation)
		{
			//this.ControlPoints.Visible = false;
			//base.OnEnterInactiveState(e);
		}

		public override void OnEnterFocusState(MouseInformation pointerInformation)
		{
			//this.ControlPoints.Visible = true;
			//base.OnEnterFocusState(e);
		}

		public override void OnEnterFocusSelectedState(MouseInformation pointerInformation)
		{
			//this.ControlPoints.Visible = true;
			//base.OnEnterSelectedState(e);
		}

		public virtual PointF CalcControlPointPosition(int controlPointIndex, PointF lastPoint, PointF currentPoint)
		{
			// Sample constraint: only alow first control point to move vertically.
			// This would be placed in the derived class
			//if (controlPointIndex == 0)
			//	return new Point(lastPoint.X, currentPoint.Y);

			return currentPoint;
		}

		protected abstract void OnControlPointChanged(object sender, ControlPointEventArgs e);
	
		private void Initialize()
		{
			base.Graphics.Add((Graphic)_controlPoints);

			// Make sure we know when the control points change
			_controlPoints.ControlPointChangedEvent += new EventHandler<ControlPointEventArgs>(OnControlPointChanged);
		}
	}
}
