using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	[Cloneable]
	public sealed class BoundableResizeControlGraphic : ControlPointsGraphic
	{
		private const int _topLeft = 0;
		private const int _topRight = 1;
		private const int _bottomLeft = 2;
		private const int _bottomRight = 3;

		private float? _fixedAspectRatio = null;

		[CloneIgnore]
		private bool _bypassControlPointChangedEvent = false;

		public BoundableResizeControlGraphic(IGraphic subject)
			: base(subject)
		{
			Platform.CheckExpectedType(base.Subject, typeof(IBoundableGraphic));

			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				RectangleF rf = this.Subject.Rectangle;
				base.ControlPoints.Add(new PointF(rf.Left, rf.Top));
				base.ControlPoints.Add(new PointF(rf.Right, rf.Top));
				base.ControlPoints.Add(new PointF(rf.Left, rf.Bottom));
				base.ControlPoints.Add(new PointF(rf.Right, rf.Bottom));
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			Initialize();
		}

		public BoundableResizeControlGraphic(float aspectRatio, IGraphic subject)
			: this(subject)
		{
			Platform.CheckPositive(aspectRatio, "aspectRatio");
			_fixedAspectRatio = aspectRatio;
		}

		public BoundableResizeControlGraphic(float? fixedAspectRatio, IGraphic subject)
			: this(subject)
		{
			if (fixedAspectRatio.HasValue)
				Platform.CheckPositive(fixedAspectRatio.Value, "fixedAspectRatio");
			_fixedAspectRatio = fixedAspectRatio;
		}

		private BoundableResizeControlGraphic(BoundableResizeControlGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			Initialize();
		}

		public float? FixedAspectRatio
		{
			get { return _fixedAspectRatio; }
			set
			{
				if (value.HasValue)
					Platform.CheckPositive(value.Value, "FixedAspectRatio");
				_fixedAspectRatio = value;
			}
		}

		public new IBoundableGraphic Subject
		{
			get { return base.Subject as IBoundableGraphic; }
		}

		private void Initialize()
		{
			this.Subject.BottomRightChanged += OnSubjectBottomRightChanged;
			this.Subject.TopLeftChanged += OnSubjectTopLeftChanged;
		}

		protected override void Dispose(bool disposing)
		{
			this.Subject.BottomRightChanged -= OnSubjectBottomRightChanged;
			this.Subject.TopLeftChanged -= OnSubjectTopLeftChanged;

			base.Dispose(disposing);
		}

		protected override PointF ConstrainControlPointLocation(int controlPointIndex, PointF cursorLocation) {
			if (!_fixedAspectRatio.HasValue)
				return cursorLocation;

			float targetRatio = _fixedAspectRatio.Value;
			PointF result = cursorLocation;
			RectangleF originalRect;

			this.CoordinateSystem = CoordinateSystem.Destination;
			try
			{
				originalRect = this.Subject.Rectangle;
			}
			finally
			{
				this.ResetCoordinateSystem();
			}

			float rectLeft = originalRect.Left;
			float rectRight = originalRect.Right;
			float rectTop = originalRect.Top;
			float rectBottom = originalRect.Bottom;

			switch (controlPointIndex)
			{
				case _topLeft:
					rectLeft = cursorLocation.X;
					rectTop = cursorLocation.Y;
					break;
				case _bottomRight:
					rectRight = cursorLocation.X;
					rectBottom = cursorLocation.Y;
					break;
				case _topRight:
					rectRight = cursorLocation.X;
					rectTop = cursorLocation.Y;
					break;
				case _bottomLeft:
					rectLeft = cursorLocation.X;
					rectBottom = cursorLocation.Y;
					break;
			}

			if(!FloatComparer.AreEqual(rectLeft, rectRight) && !FloatComparer.AreEqual(rectTop, rectBottom))
			{
				float cursorRatio = (rectRight - rectLeft)/(rectBottom - rectTop);
				int ratioSgn = Math.Sign(cursorRatio);
				cursorRatio = Math.Abs(cursorRatio);
				if(!FloatComparer.AreEqual(cursorRatio, targetRatio))
				{
					SizeF offset;
					if(cursorRatio > targetRatio)
					{
						float height = rectBottom - rectTop;
						float width = height*targetRatio;
						float delta = rectRight - rectLeft - ratioSgn*width;

						if (controlPointIndex == _topLeft || controlPointIndex == _bottomLeft)
							offset = new SizeF(delta, 0);
						else
							offset = new SizeF(-delta, 0);
					}
					else
					{
						float width = rectRight - rectLeft;
						float height = width / targetRatio;
						float delta = rectBottom - rectTop - ratioSgn*height;

						if (controlPointIndex == _topLeft || controlPointIndex == _topRight)
							offset = new SizeF(0, delta);
						else
							offset = new SizeF(0, -delta);
					}
					result += offset;
				}
			}
			return result;
		}

		protected override void OnControlPointChanged(int index, PointF point)
		{
			if (!_bypassControlPointChangedEvent)
			{
				IBoundableGraphic subject = this.Subject;
				RectangleF rect = subject.Rectangle;
				switch (index)
				{
					case _topLeft:
						subject.TopLeft = point;
						break;
					case _bottomRight:
						subject.BottomRight = point;
						break;
					case _topRight:
						subject.TopLeft = new PointF(rect.Left, point.Y);
						subject.BottomRight = new PointF(point.X, rect.Bottom);
						break;
					case _bottomLeft:
						subject.TopLeft = new PointF(point.X, rect.Top);
						subject.BottomRight = new PointF(rect.Right, point.Y);
						break;
				}
			}
			base.OnControlPointChanged(index, point);
		}

		private void OnSubjectTopLeftChanged(object sender, PointChangedEventArgs e)
		{
			_bypassControlPointChangedEvent = true;
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				RectangleF rect = this.Subject.Rectangle;
				this[_topRight] = new PointF(rect.Right, rect.Top);
				this[_topLeft] = new PointF(rect.Left, rect.Top);
				this[_bottomLeft] = new PointF(rect.Left, rect.Bottom);
			}
			finally
			{
				this.ResetCoordinateSystem();
				_bypassControlPointChangedEvent = false;
			}
		}

		private void OnSubjectBottomRightChanged(object sender, PointChangedEventArgs e)
		{
			_bypassControlPointChangedEvent = true;
			this.CoordinateSystem = CoordinateSystem.Source;
			try
			{
				RectangleF rect = this.Subject.Rectangle;
				this[_topRight] = new PointF(rect.Right, rect.Top);
				this[_bottomRight] = new PointF(rect.Right, rect.Bottom);
				this[_bottomLeft] = new PointF(rect.Left, rect.Bottom);
			}
			finally
			{
				this.ResetCoordinateSystem();
				_bypassControlPointChangedEvent = false;
			}
		}
	}
}