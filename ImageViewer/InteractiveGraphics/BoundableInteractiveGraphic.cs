using System;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// An <see cref="InteractiveGraphic"/> that can be represented 
	/// as a bounding rectangle.
	/// </summary>
	public abstract class BoundableInteractiveGraphic : InteractiveGraphic
	{
		private const int _topLeft = 0;
		private const int _topRight = 1;
		private const int _bottomLeft = 2;
		private const int _bottomRight = 3;
		private BoundableGraphic _boundableGraphic;
		private CursorToken _moveToken;

		/// <summary>
		/// Initializes a new instance of <see cref="InteractiveGraphic"/>.
		/// </summary>
		/// <param name="userCreated"></param>
		protected BoundableInteractiveGraphic(bool userCreated) : base(userCreated)
		{
			AddControlPoints();

			_moveToken = new CursorToken(CursorToken.SystemCursors.SizeAll);
		}

		/// <summary>
		/// Gets or sets the top left corner of the rectangle graphic.
		/// </summary>
		public PointF TopLeft
		{
			get { return this.BoundableGraphic.TopLeft; }
			set { this.BoundableGraphic.TopLeft = value; }
		}

		/// <summary>
		/// Gets or sets the bottom right corner of the rectangle graphic.
		/// </summary>
		public PointF BottomRight
		{
			get { return this.BoundableGraphic.BottomRight; }
			set { this.BoundableGraphic.BottomRight = value; }
		}

		/// <summary>
		/// Gets the left coordinate of the rectangle graphic.
		/// </summary>
		public float Left
		{
			get { return this.TopLeft.X; }
		}

		/// <summary>
		/// Gets the right coordinate of the rectangle graphic.
		/// </summary>
		public float Right
		{
			get { return this.BottomRight.X; }
		}

		/// <summary>
		/// Gets the top coordinate of the rectangle graphic.
		/// </summary>
		public float Top
		{
			get { return this.TopLeft.Y; }
		}

		/// <summary>
		/// Gets the bottom coordinate of the rectangle graphic.
		/// </summary>
		public float Bottom
		{
			get { return this.BottomRight.Y; }
		}

		/// <summary>
		/// Gets the width of the rectangle graphic.
		/// </summary>
		public float Width
		{
			get { return this.Right - this.Left; }
		}

		/// <summary>
		/// Gets the height of the rectangle graphic.
		/// </summary>
		public float Height
		{
			get { return this.Bottom - this.Top; }
		}

		/// <summary>
		/// Gets the graphic's tightest bounding box.
		/// </summary>
		public override RectangleF BoundingBox
		{
			get { return new RectangleF(this.TopLeft, new SizeF(this.Width, this.Height)); }
		}

		/// <summary>
		/// Gets or sets the colour of the <see cref="RectangleInteractiveGraphic"/>.
		/// </summary>
		public override Color Color
		{
			get { return base.Color; }
			set
			{
				this.BoundableGraphic.Color = value;
				base.Color = value;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="CursorToken"/> that should be shown
		/// when moving this graphic.
		/// </summary>
		public CursorToken MoveToken
		{
			get { return _moveToken; }
			set { _moveToken = value; }
		}

		private BoundableGraphic BoundableGraphic
		{
			get
			{
				if (_boundableGraphic == null)
				{
					_boundableGraphic = CreateBoundableGraphic();
					base.Graphics.Add(_boundableGraphic);
					_boundableGraphic.TopLeftChanged += new EventHandler<PointChangedEventArgs>(OnTopLeftChanged);
					_boundableGraphic.BottomRightChanged += new EventHandler<PointChangedEventArgs>(OnBottomRightChanged);
				}

				return _boundableGraphic;
			}
		}

		/// <summary>
		/// Creates the actual boundable graphic, such as a rectangle or ellipse.
		/// </summary>
		/// <returns></returns>
		protected abstract BoundableGraphic CreateBoundableGraphic();

		/// <summary>
		/// Captures the state of the <see cref="RectangleInteractiveGraphic"/>.
		/// </summary>
		/// <returns></returns>
		public override object CreateMemento()
		{
			RectangleMemento memento = new RectangleMemento();

			// Must store source coordinates in memento
			this.CoordinateSystem = CoordinateSystem.Source;
			memento.TopLeft = this.TopLeft;
			memento.BottomRight = this.BottomRight;
			this.ResetCoordinateSystem();

			return memento;
		}

		/// <summary>
		/// Restores the state of the <see cref="RectangleInteractiveGraphic"/>.
		/// </summary>
		/// <param name="memento"></param>
		public override void SetMemento(object memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			RectangleMemento graphicMemento = memento as RectangleMemento;
			Platform.CheckForInvalidCast(graphicMemento, "memento", "RectangleMemento");

			this.CoordinateSystem = CoordinateSystem.Source;
			this.TopLeft = graphicMemento.TopLeft;
			this.BottomRight = graphicMemento.BottomRight;
			this.ResetCoordinateSystem();
		}

		/// <summary>
		/// Moves the <see cref="RectangleInteractiveGraphic"/> by the
		/// specified delta.
		/// </summary>
		/// <param name="delta"></param>
		public override void Move(SizeF delta)
		{
#if MONO
			Size del = new Size((int)delta.Width, (int)delta.Height);
			this.TopLeft += del;
			this.BottomRight += del;
#else
			this.TopLeft += delta;
			this.BottomRight += delta;
#endif
		}

		/// <summary>
		/// Gets the cursor token to be shown at the current mouse position.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override CursorToken GetCursorToken(Point point)
		{
			CursorToken token = base.GetCursorToken(point);
			if (token == null)
			{
				if (this.HitTest(point))
				{
					token = this.MoveToken;
				}
			}

			return token;
		}

		/// <summary>
		/// Creates a creation <see cref="GraphicState"/>.
		/// </summary>
		/// <returns></returns>
		protected override GraphicState CreateCreateState()
		{
			return new CreateBoundableGraphicState(this);
		}

		/// <summary>
		/// Executed when the position of the top left corner of the 
		/// rectangle has changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnTopLeftChanged(object sender, PointChangedEventArgs e)
		{
			// This acts as a mediator.  It listens for changes in the anchor points
			// and make corresponding changes in the position of the control points.
			Platform.CheckForNullReference(sender, "sender");
			Platform.CheckForNullReference(e, "e");

			Trace.Write(String.Format("OnTopLeftChanged: {0}\n", e.Point.ToString()));

			base.ControlPoints[_topLeft] = e.Point;
			base.ControlPoints[_topRight] = new PointF(this.BottomRight.X, this.TopLeft.Y);
			base.ControlPoints[_bottomLeft] = new PointF(this.TopLeft.X, this.BottomRight.Y);
		}

		/// <summary>
		/// Executed when the position of the bottom right corner of the 
		/// rectangle has changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnBottomRightChanged(object sender, PointChangedEventArgs e)
		{
			Platform.CheckForNullReference(sender, "sender");
			Platform.CheckForNullReference(e, "e");

			Trace.Write(String.Format("OnBottomRightChanged: {0}\n", e.Point.ToString()));

			base.ControlPoints[_topRight] = new PointF(this.BottomRight.X, this.TopLeft.Y);
			base.ControlPoints[_bottomLeft] = new PointF(this.TopLeft.X, this.BottomRight.Y);
			base.ControlPoints[_bottomRight] = e.Point;
		}

		/// <summary>
		/// Executed when a the position of a control point has changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnControlPointChanged(object sender, ListEventArgs<PointF> e)
		{
			Platform.CheckForNullReference(sender, "sender");
			Platform.CheckForNullReference(e, "e");

			Trace.Write(String.Format("OnControlPointChanged: index = {0}, {1}\n", e.Index, e.Item.ToString()));

			switch (e.Index)
			{
				case _topLeft:
					this.TopLeft = e.Item;
					break;
				case _topRight:
					this.TopLeft = new PointF(this.TopLeft.X, e.Item.Y);
					this.BottomRight = new PointF(e.Item.X, this.BottomRight.Y);
					break;
				case _bottomLeft:
					this.TopLeft = new PointF(e.Item.X, this.TopLeft.Y);
					this.BottomRight = new PointF(this.BottomRight.X, e.Item.Y);
					break;
				case _bottomRight:
					this.BottomRight = e.Item;
					break;
			}
		}

		/// <summary>
		/// Peforms a hit test on the <see cref="BoundableInteractiveGraphic"/>
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override bool HitTest(Point point)
		{
			return this.BoundableGraphic.HitTest(point);
		}

		private void AddControlPoints()
		{
			for (int i = 0; i < 4; i++)
				base.ControlPoints.Add(new PointF(0, 0));
		}

	}
}