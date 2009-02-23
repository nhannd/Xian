#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// An <see cref="InteractiveGraphic"/> that can be represented 
	/// as a bounding rectangle.
	/// </summary>
	[Cloneable]
	public abstract class BoundableInteractiveGraphic : InteractiveGraphic
	{
		private const int _topLeft = 0;
		private const int _topRight = 1;
		private const int _bottomLeft = 2;
		private const int _bottomRight = 3;
		
		[CloneIgnore]
		private BoundableGraphic _boundableGraphic;

		private bool _settingControlPoints;

		/// <summary>
		/// Initializes a new instance of <see cref="InteractiveGraphic"/>.
		/// </summary>
		protected BoundableInteractiveGraphic()
		{
			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected BoundableInteractiveGraphic(BoundableInteractiveGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
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

		private BoundableGraphic BoundableGraphic
		{
			get { return _boundableGraphic; }
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

			_settingControlPoints = true;

			PointF bottomRight = this.BottomRight;

			base.ControlPoints[_topLeft] = e.Point;
			base.ControlPoints[_topRight] = new PointF(bottomRight.X, e.Point.Y);
			base.ControlPoints[_bottomLeft] = new PointF(e.Point.X, bottomRight.Y);

			_settingControlPoints = false;
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

			_settingControlPoints = true;

			PointF topLeft = this.TopLeft;

			base.ControlPoints[_topRight] = new PointF(e.Point.X, topLeft.Y);
			base.ControlPoints[_bottomLeft] = new PointF(topLeft.X, e.Point.Y);
			base.ControlPoints[_bottomRight] = e.Point;

			_settingControlPoints = false;
		}

		/// <summary>
		/// Executed when a the position of a control point has changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnControlPointChanged(object sender, ListEventArgs<PointF> e)
		{
			// Don't set the TopLeft and BottomRight values when the control points
			// are currently being set based on their values.
			if (_settingControlPoints)
				return;

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

		private void Initialize()
		{
			for (int i = base.ControlPoints.Count; i < 4; i++)
				base.ControlPoints.Add(new PointF(0, 0));

			_settingControlPoints = false;

			if (_boundableGraphic == null)
			{
				_boundableGraphic = CreateBoundableGraphic();
				base.Graphics.Add(_boundableGraphic);
			}

			_boundableGraphic.TopLeftChanged += new EventHandler<PointChangedEventArgs>(OnTopLeftChanged);
			_boundableGraphic.BottomRightChanged += new EventHandler<PointChangedEventArgs>(OnBottomRightChanged);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_boundableGraphic = CollectionUtils.SelectFirst(base.Graphics,
				delegate(IGraphic test) { return test is BoundableGraphic; }) as BoundableGraphic;
			
			Platform.CheckForNullReference(_boundableGraphic, "_boundableGraphic");
			Initialize();
		}
	}
}