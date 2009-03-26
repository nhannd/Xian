using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public class MoveControlGraphic : ControlGraphic, IActiveControlGraphic
	{
		[CloneCopyReference]
		private CursorToken _cursor;

		public MoveControlGraphic(IGraphic subject) : base(subject)
		{
			_cursor = new CursorToken(CursorToken.SystemCursors.SizeAll);
		}

		protected MoveControlGraphic(MoveControlGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
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
			this.DecoratedGraphic.Move(delta);
		}

		protected override bool OnMouseStart(IMouseInformation mouseInformation)
		{
			this.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				if (this.HitTest(mouseInformation.Location))
				{
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
	}
}