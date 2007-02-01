using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public abstract class InteractiveGraphic
		: StatefulCompositeGraphic, IStandardStatefulGraphic, IMemorable
	{
		public static readonly int HitTestDistance = 10;

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
			set { _stretchToken = value; }
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

		public virtual GraphicState CreateFocusState()
		{
			return new FocusGraphicState(this);
		}

		public virtual GraphicState CreateFocusSelectedState()
		{
			return new FocusSelectedInteractiveGraphicState(this);
		}

		public virtual GraphicState CreateSelectedState()
		{
			return new SelectedGraphicState(this);
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
			base.Graphics.Add(this.ControlPoints);

			// Make sure we know when the control points change
			_controlPointGroup.ControlPointChangedEvent += new EventHandler<ControlPointEventArgs>(OnControlPointChanged);
		}
	}
}
