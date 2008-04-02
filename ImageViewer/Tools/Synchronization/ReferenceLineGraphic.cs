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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	internal class ReferenceLineCompositeGraphic : CompositeGraphic
	{
		private object _tag;

		public ReferenceLineCompositeGraphic()
		{
		}

		public object Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}

		public ReferenceLineGraphic this[int index]
		{
			get
			{
				for (int i = base.Graphics.Count; i <= index; ++i)
					base.Graphics.Add(new ReferenceLineGraphic());

				return (ReferenceLineGraphic)base.Graphics[index];
			}
		}
	}

	internal class ReferenceLineGraphic : CompositeGraphic
	{
		private readonly LinePrimitive _line;
		private readonly InvariantTextPrimitive _text;
		private PointF _point1;
		private PointF _point2;

		public ReferenceLineGraphic()
		{
			base.Graphics.Add(_text = new InvariantTextPrimitive());
			base.Graphics.Add(_line = new LinePrimitive());

			_text.BoundingBoxChanged += OnTextBoundingBoxChanged;
		}

		public override void OnDrawing()
		{
			OnTextBoundingBoxChanged(this, null);

			base.OnDrawing();
		}

		private void OnTextBoundingBoxChanged(object sender, RectangleChangedEventArgs e)
		{
			this.CoordinateSystem = CoordinateSystem.Destination;

			Rectangle clientRectangle = base.ParentPresentationImage.ClientRectangle;

			PointF startPoint = Point1;
			PointF endPoint = Point2;

			Vector3D lineDirection = new Vector3D(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, 0F);

			if (Vector3D.AreEqual(lineDirection, Vector3D.Empty))
			{
				_line.Pt1 = Point1;
				_line.Pt2 = Point2;
				_text.AnchorPoint = Point1;
				return;
			}
					
			Vector3D lineUnit = lineDirection.Normalize();

			Vector3D xUnit = new Vector3D(1F, 0, 0);
			Vector3D yUnit = new Vector3D(0, 1F, 0);

			float cosThetaX = Math.Abs(xUnit.Dot(lineUnit));
			float cosThetaY = Math.Abs(yUnit.Dot(lineUnit));

			PointF clientTopRight = new PointF(clientRectangle.Right, clientRectangle.Top);
			PointF clientBottomLeft = new PointF(clientRectangle.Left, clientRectangle.Bottom);
			PointF clientBottomRight = new PointF(clientRectangle.Right, clientRectangle.Bottom);

			float textWidth = _text.BoundingBox.Width;
			float textHeight = _text.BoundingBox.Height;

			float lineDistanceToOutsideTextEdge;

			if (cosThetaX >= cosThetaY)
			{
				// the distance along the line to where we want the outside right edge of the text to be.
				lineDistanceToOutsideTextEdge = cosThetaX * textWidth;
				if (lineDirection.X < 0)
				{
					lineDirection *= -1F;
					startPoint = Point2;
					endPoint = Point1;
				}
			}
			else
			{
				// the distance along the line to where we want the outside bottom edge of the text to be.
				lineDistanceToOutsideTextEdge = cosThetaY * textHeight;
				if (lineDirection.Y < 0)
				{
					lineDirection *= -1F;
					startPoint = Point2;
					endPoint = Point1;
				}
			}

			float lineDistanceToOutsideTextEdgeRatio = lineDistanceToOutsideTextEdge / lineDirection.Magnitude;

			SizeF outsideTextEdgeOffset = new SizeF(lineDistanceToOutsideTextEdgeRatio * lineDirection.X,
												   lineDistanceToOutsideTextEdgeRatio * lineDirection.Y);

			SizeF textAnchorPointOffset = new SizeF(outsideTextEdgeOffset.Width / 2F, outsideTextEdgeOffset.Height / 2F);

			// extend the endpoint of the line by the distance to the outside text edge.
			endPoint = PointF.Add(endPoint, outsideTextEdgeOffset);
			// add an additional 5 pixel offset so we don't push back as far as the start point.
			endPoint = PointF.Add(endPoint, new SizeF(5F * lineUnit.X, 5F * lineUnit.Y));

			SizeF clientEdgeOffset = Size.Empty;

			// find the intersection of the extended line segment and either of the left or bottom client edge.
			PointF intersectionPoint;
			if (Vector.LineSegments.Intersect ==
				Vector.LineSegmentIntersection(startPoint, endPoint, clientTopRight, clientBottomRight, out intersectionPoint) ||
				Vector.LineSegments.Intersect ==
				Vector.LineSegmentIntersection(startPoint, endPoint, clientBottomLeft, clientBottomRight, out intersectionPoint))
			{
				Vector3D clientEdgeOffsetVector = new Vector3D(endPoint.X - intersectionPoint.X, endPoint.Y - intersectionPoint.Y, 0);
				//don't allow the text to be pushed back past the start point.
				if (clientEdgeOffsetVector.Magnitude > lineDirection.Magnitude)
					clientEdgeOffsetVector = lineDirection;

				clientEdgeOffset = new SizeF(clientEdgeOffsetVector.X, clientEdgeOffsetVector.Y);
			}
			
			_line.Pt1 = startPoint;

			// offset by the distance from the extended endpoint to the client rectangle edge.
			endPoint = PointF.Subtract(endPoint, clientEdgeOffset);
			// offset again by half the distance necessary to keep the text box inside the client rectangle
			endPoint = PointF.Subtract(endPoint, textAnchorPointOffset);

			// this aligns the text edge with the client edge in the case where the line intersects the client edge.
			_text.AnchorPoint = endPoint;
			
			// offset the line by half again the distance necessary to keep the text box inside the client rectangle.
			_line.Pt2 = PointF.Subtract(endPoint, textAnchorPointOffset);
			
			base.ResetCoordinateSystem();
		}

		public PointF Point1
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _point1;
				else
					return base.SpatialTransform.ConvertToDestination(_point1);
			}
			set
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					_point1 = value;
				else
					_point1 = base.SpatialTransform.ConvertToSource(_point1);
			}
		}

		public PointF Point2
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					return _point2;
				else
					return base.SpatialTransform.ConvertToDestination(_point2);
			}
			set
			{
				if (base.CoordinateSystem == CoordinateSystem.Source)
					_point2 = value;
				else
					_point2 = base.SpatialTransform.ConvertToSource(_point2);
			}
		}

		public string Text
		{
			get { return _text.Text; }	
			set { _text.Text = value; }	
		}
	}
}
