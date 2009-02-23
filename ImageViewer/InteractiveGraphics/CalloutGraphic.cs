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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.PresentationStates;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// A graphical representation of a callout.
	/// </summary>
	/// <remarks>
	/// A callout can be used to label something in the scene graph. It is
	/// composed of a text label and a line that extends from the label
	/// to some user defined point in the scene.
	/// </remarks>
	[Cloneable]
	[DicomSerializableGraphicAnnotation(typeof(CalloutGraphicAnnotationSerializer))]
	public class CalloutGraphic : InteractiveGraphic, IMemorable
	{
		#region Private fields

		[CloneIgnore]
		private InvariantTextPrimitive _textGraphic;
		[CloneIgnore]
		private ArrowGraphic _lineGraphic;

		#endregion

		/// <summary>
		/// Instantiates a new instance of <see cref="CalloutGraphic"/>.
		/// </summary>
		public CalloutGraphic()
        {
			Initialize();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected CalloutGraphic(CalloutGraphic source, ICloningContext context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Gets or sets the text label.
		/// </summary>
		public string Text
		{
			get { return _textGraphic.Text; }
			set { _textGraphic.Text = value; }
		}

		/// <summary>
		/// Gets or sets the location of the center of the text label.
		/// </summary>
		public PointF Location
		{
			get { return _textGraphic.AnchorPoint; }
			set 
			{ 
				_textGraphic.AnchorPoint = value;
			}
		}

		/// <summary>
		/// Gets the starting point of the callout line.
		/// </summary>
		/// <remarks>
		/// The starting point of the callout line is automatically
		/// calculated so that it appears as though it starts
		/// from the center of the text label.
		/// </remarks>
		public PointF StartPoint
		{
			get
			{
				return _lineGraphic.StartPoint;
			}
		}

		/// <summary>
		/// Gets or sets the ending point of the callout line.
		/// </summary>
		public PointF EndPoint
		{
			get { return _lineGraphic.EndPoint; }
			set 
			{
				_lineGraphic.EndPoint = value;
				SetCalloutLineStart();
			}
		}

		public bool ShowArrow
		{
			get { return _lineGraphic.ShowArrowhead; }
			set { _lineGraphic.ShowArrowhead = value; }
		}

		protected override void OnColorChanged() 
		{
			base.OnColorChanged();

			_lineGraphic.Color = base.Color;
			_textGraphic.Color = base.Color;
		}

		/// <summary>
		/// Occurs when the location of the label portion of the callout
		/// has changed.
		/// </summary>
		public event EventHandler<PointChangedEventArgs> LocationChanged
		{
			add { _textGraphic.AnchorPointChanged += value; }
			remove { _textGraphic.AnchorPointChanged -= value; }
		}

		/// <summary>
		/// Creates a memento of this object.
		/// </summary>
		/// <returns></returns>
		public override object CreateMemento()
        {
			// Must store source coordinates in memento
			this.CoordinateSystem = CoordinateSystem.Source;
			PointMemento memento = new PointMemento(this.Location);
			this.ResetCoordinateSystem();

			return memento;
        }

		/// <summary>
		/// Sets a memento for this object.
		/// </summary>
		/// <param name="memento"></param>
        public override void SetMemento(object memento)
        {
			PointMemento pointMemento = (PointMemento)memento;

			this.CoordinateSystem = CoordinateSystem.Source;
			this.Location = pointMemento.Point;
			this.ResetCoordinateSystem();

			SetCalloutLineStart();
		}

		/// <summary>
		/// This method overrides <see cref="Graphic.Move"/>.
		/// </summary>
		/// <param name="delta"></param>
		public override void Move(SizeF delta)
		{
			_textGraphic.Move(delta);
		}
 
		/// <summary>
		/// This method overrides <see cref="Graphic.HitTest"/>.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		/// <remarks>
		/// A hit on either the text label or callout line consitutes
		/// a valid hit on the <see cref="CalloutGraphic"/>.
		/// </remarks>
		public override bool HitTest(Point point)
        {
			return _textGraphic.HitTest(point) || _lineGraphic.HitTest(point);
        }

		private void Initialize()
		{
			if (_textGraphic == null)
			{
				_textGraphic = new InvariantTextPrimitive();
				this.Graphics.Add(_textGraphic);
			}
			
			_textGraphic.BoundingBoxChanged += new EventHandler<RectangleChangedEventArgs>(OnTextBoundingBoxChanged);

			if (_lineGraphic == null)
			{
				_lineGraphic = new ArrowGraphic(false);
				this.Graphics.Add(_lineGraphic);
				_lineGraphic.LineStyle = LineStyle.Dash;
			}
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_textGraphic = CollectionUtils.SelectFirst(base.Graphics,
				delegate(IGraphic test) { return test is InvariantTextPrimitive; }) as InvariantTextPrimitive;

			_lineGraphic = CollectionUtils.SelectFirst(base.Graphics,
				delegate(IGraphic test) { return test is ArrowGraphic; }) as ArrowGraphic;

			Platform.CheckForNullReference(_lineGraphic, "_lineGraphic");
			Platform.CheckForNullReference(_textGraphic, "_textGraphic");

			Initialize();
		}

		private void SetCalloutLineStart()
		{
			this.CoordinateSystem = CoordinateSystem.Destination;
			this._lineGraphic.StartPoint = GetClosestPoint(this.EndPoint);
			this.ResetCoordinateSystem();
		}

		/// <summary>
		/// Gets the point on the graphic closest to the specified point in either source or destination coordinates.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public override PointF GetClosestPoint(PointF point) {
			RectangleF boundingBox = _textGraphic.BoundingBox;
			boundingBox.Inflate(3, 3);

			PointF topLeft = new PointF(boundingBox.Left, boundingBox.Top);
			PointF topRight = new PointF(boundingBox.Right, boundingBox.Top);
			PointF bottomLeft = new PointF(boundingBox.Left, boundingBox.Bottom);
			PointF bottomRight = new PointF(boundingBox.Right, boundingBox.Bottom);
			PointF geoCenter = Vector.Midpoint(topLeft, bottomRight);
			PointF intersectionPoint;

			if (Vector.LineSegmentIntersection(geoCenter, point, topLeft, topRight, out intersectionPoint) == Vector.LineSegments.Intersect)
				return intersectionPoint;

			if (Vector.LineSegmentIntersection(geoCenter, point, bottomLeft, bottomRight, out intersectionPoint) == Vector.LineSegments.Intersect)
				return intersectionPoint;

			if (Vector.LineSegmentIntersection(geoCenter, point, topLeft, bottomLeft, out intersectionPoint) == Vector.LineSegments.Intersect)
				return intersectionPoint;

			if (Vector.LineSegmentIntersection(geoCenter, point, topRight, bottomRight, out intersectionPoint) == Vector.LineSegments.Intersect)
				return intersectionPoint;

			return point;
		}

		private void OnTextBoundingBoxChanged(object sender, RectangleChangedEventArgs e)
		{
			SetCalloutLineStart();
		}

		public override RectangleF BoundingBox {
			get { return _textGraphic.BoundingBox; }
		}

		protected override void OnControlPointChanged(object sender, ListEventArgs<PointF> e) {
			// do nothing - we don't have any control points
		}
	}
}
