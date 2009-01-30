using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public class ArrowInteractiveGraphic : InteractiveGraphic
	{
		[CloneIgnore]
		private ArrowGraphic _arrowGraphic;

		[CloneCopyReference]
		private CursorToken _moveToken;

		public ArrowInteractiveGraphic(bool userCreated) : base(userCreated)
		{
			Initialize();
		}

		protected ArrowInteractiveGraphic(ArrowInteractiveGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		private void Initialize()
		{
			if (_arrowGraphic == null)
			{
				base.Graphics.Add(_arrowGraphic = new ArrowGraphic());
				base.ControlPoints.Add(new PointF());
				base.ControlPoints.Add(new PointF());
			}

			if (_moveToken == null)
			{
				_moveToken = new CursorToken(CursorToken.SystemCursors.SizeAll);
			}
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_arrowGraphic = CollectionUtils.SelectFirst(base.Graphics,
			                                            delegate(IGraphic test) { return test is ArrowGraphic; }) as ArrowGraphic;
			Platform.CheckForNullReference(_arrowGraphic, "_anchorPointsGraphic");
			Initialize();
		}

		public ArrowGraphic Arrow
		{
			get { return _arrowGraphic; }
		}

		public override Color Color
		{
			get { return _arrowGraphic.Color; }
			set { _arrowGraphic.Color = value; }
		}

		public override RectangleF BoundingBox
		{
			get { return RectangleUtilities.ComputeBoundingRectangle(_arrowGraphic.StartPoint, _arrowGraphic.EndPoint); }
		}

		public CursorToken MoveToken
		{
			get { return _moveToken; }
			set { _moveToken = value; }
		}

		public override object CreateMemento()
		{
			PointsMemento memento = new PointsMemento();

			// Must store source coordinates in memento
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				memento.Add(this.Arrow.StartPoint);
				memento.Add(this.Arrow.EndPoint);
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			return memento;
		}

		public override void SetMemento(object memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			PointsMemento pointsMemento = (PointsMemento) memento;

			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				this.Arrow.StartPoint = pointsMemento[0];
				this.Arrow.EndPoint = pointsMemento[1];
			}
			finally
			{
				this.ResetCoordinateSystem();
			}
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			bool result = base.Start(mouseInformation);

			if (base.State is MoveControlPointGraphicState ||
				base.State is MoveGraphicState)
			{
				UpdateControlPointVisiblity(false);
			}

			return result;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (base.State is MoveControlPointGraphicState ||
				base.State is MoveGraphicState)
			{
				UpdateControlPointVisiblity(true);
			}

			return base.Stop(mouseInformation);
		}

		public override void Cancel()
		{
			if (base.State is MoveControlPointGraphicState ||
				base.State is MoveGraphicState)
			{
				UpdateControlPointVisiblity(true);
			}

			base.Cancel();
		}

		private void UpdateControlPointVisiblity(bool visible)
		{
			foreach (IGraphic graphic in base.ControlPoints.Graphics)
			{
				graphic.Visible = visible;
			}
		}

		public override void Move(SizeF delta)
		{
			this.ControlPoints[0] += delta;
			this.ControlPoints[1] += delta;
		}

		public override bool HitTest(Point point)
		{
			return this.Arrow.HitTest(point);
		}

		public override PointF GetClosestPoint(PointF point)
		{
			return this.Arrow.GetClosestPoint(point);
		}

		public override CursorToken GetCursorToken(Point point)
		{
			CursorToken token = base.GetCursorToken(point);
			if (token == null)
			{
				if (this.HitTest(point))
					token = this.MoveToken;
			}
			return token;
		}

		protected override GraphicState CreateCreateState()
		{
			return new CreateArrowGraphicState(this);
		}

		protected override void OnControlPointChanged(object sender, ListEventArgs<PointF> e)
		{
			if (e.Index == 0)
			{
				// first control point is the arrow's tail
				this.Arrow.StartPoint = e.Item;
			}
			else if (e.Index == 1)
			{
				// second control point is the arrow's head
				this.Arrow.EndPoint = e.Item;
			}
		}
	}
}