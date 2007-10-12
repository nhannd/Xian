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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public class RectangleInteractiveGraphic : InteractiveGraphic
	{
		private const int _topLeft = 0;
		private const int _topRight = 1;
		private const int _bottomLeft = 2;
		private const int _bottomRight = 3;

		private RectanglePrimitive _rectangleGraphic;

		private CursorToken _moveToken;
		
		public RectangleInteractiveGraphic(bool userCreated)
			: base(userCreated)
		{
			BuildGraphic();
		}

		public PointF TopLeft
		{
			get { return _rectangleGraphic.TopLeft; }
			set { _rectangleGraphic.TopLeft = value; }
		}

		public PointF BottomRight
		{
			get { return _rectangleGraphic.BottomRight; }
			set { _rectangleGraphic.BottomRight = value; }
		}

		public float Left
		{
			get { return this.TopLeft.X; }
		}

		public float Right
		{
			get { return this.BottomRight.X; }
		}

		public float Top
		{
			get { return this.TopLeft.Y; }
		}

		public float Bottom
		{
			get { return this.BottomRight.Y; }
		}

		public float Width
		{
			get { return this.Right - this.Left; }
		}

		public float Height
		{
			get { return this.Bottom - this.Top; }
		}

		public CursorToken MoveToken
		{
			get { return _moveToken; }
			set { _moveToken = value; }
		}

		public override Color Color
		{
			get { return base.Color; }
			set
			{
				_rectangleGraphic.Color = value;
				base.Color = value;
			}
		}

		#region IMemorable Members

		public override IMemento CreateMemento()
		{
			RectangleMemento memento = new RectangleMemento();

			// Must store source coordinates in memento
			this.CoordinateSystem = CoordinateSystem.Source;
			memento.TopLeft = this.TopLeft;
			memento.BottomRight = this.BottomRight;
			this.ResetCoordinateSystem();

			return memento;
		}

		public override void SetMemento(IMemento memento)
		{
			Platform.CheckForNullReference(memento, "memento");
			RectangleMemento graphicMemento = memento as RectangleMemento;
			Platform.CheckForInvalidCast(graphicMemento, "memento", "RectangleMemento");

			this.CoordinateSystem = CoordinateSystem.Source;
			this.TopLeft = graphicMemento.TopLeft;
			this.BottomRight = graphicMemento.BottomRight;
			this.ResetCoordinateSystem();
		}

		#endregion

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

		public override GraphicState CreateCreateState()
		{
			return new CreateRectangleGraphicState(this);
		}

		public override bool HitTest(Point point)
		{
			return _rectangleGraphic.HitTest(point);
		}

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

		public override void InstallDefaultCursors()
		{
			base.InstallDefaultCursors();
			this.MoveToken = new CursorToken(CursorToken.SystemCursors.SizeAll);
		}
		
		private void BuildGraphic()
		{
			AddRectangle();
			AddControlPoints();
		}

		private void AddRectangle()
		{
			_rectangleGraphic = new RectanglePrimitive();
			base.Graphics.Add(_rectangleGraphic);
			_rectangleGraphic.TopLeft = new PointF(0, 0);
			_rectangleGraphic.BottomRight = new PointF(0, 0);
			_rectangleGraphic.TopLeftChanged += new EventHandler<PointChangedEventArgs>(OnTopLeftChanged);
			_rectangleGraphic.BottomRightChanged += new EventHandler<PointChangedEventArgs>(OnBottomRightChanged);
		}

		private void AddControlPoints()
		{
			for (int i = 0; i < 4; i++)
				base.ControlPoints.Add(new PointF(0, 0));
		}

		// This acts as a mediator.  It listens for changes in the anchor points
		// and make corresponding changes in the position of the control points.
		protected void OnTopLeftChanged(object sender, PointChangedEventArgs e)
		{
			Platform.CheckForNullReference(sender, "sender");
			Platform.CheckForNullReference(e, "e");

			Trace.Write(String.Format("OnTopLeftChanged: {0}\n", e.Point.ToString()));

			base.ControlPoints[_topLeft] = e.Point;
			base.ControlPoints[_topRight] = new PointF(this.BottomRight.X, this.TopLeft.Y);
			base.ControlPoints[_bottomLeft] = new PointF(this.TopLeft.X, this.BottomRight.Y);
		}

		protected void OnBottomRightChanged(object sender, PointChangedEventArgs e)
		{
			Platform.CheckForNullReference(sender, "sender");
			Platform.CheckForNullReference(e, "e");

			Trace.Write(String.Format("OnBottomRightChanged: {0}\n", e.Point.ToString()));

			base.ControlPoints[_topRight] = new PointF(this.BottomRight.X, this.TopLeft.Y);
			base.ControlPoints[_bottomLeft] = new PointF(this.TopLeft.X, this.BottomRight.Y);
			base.ControlPoints[_bottomRight] = e.Point;
		}

		protected override void OnControlPointChanged(object sender, ControlPointEventArgs e)
		{
			Platform.CheckForNullReference(sender, "sender");
			Platform.CheckForNullReference(e, "e");

			Trace.Write(String.Format("OnControlPointChanged: index = {0}, {1}\n", e.ControlPointIndex, e.ControlPointLocation.ToString()));

			switch (e.ControlPointIndex)
			{
				case _topLeft:
					this.TopLeft = e.ControlPointLocation;
					break;
				case _topRight:
					this.TopLeft = new PointF(this.TopLeft.X, e.ControlPointLocation.Y);
					this.BottomRight = new PointF(e.ControlPointLocation.X, this.BottomRight.Y);
				    break;
				case _bottomLeft:
					this.TopLeft = new PointF(e.ControlPointLocation.X, this.TopLeft.Y);
					this.BottomRight = new PointF(this.BottomRight.X, e.ControlPointLocation.Y);
				    break;
				case _bottomRight:
					this.BottomRight = e.ControlPointLocation;
					break;
			}
		}
	}
}
