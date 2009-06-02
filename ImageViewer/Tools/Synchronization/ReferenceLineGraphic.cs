#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	#region ReferenceLineCompositeGraphic class

	internal class ReferenceLineCompositeGraphic : CompositeGraphic
	{
		private bool _dirty = false;

		public ReferenceLineCompositeGraphic()
		{
		}

		public bool Dirty
		{
			get { return _dirty; }
		}

		public override bool Visible
		{
			//always visible so the dirty flag gets reset on draw
			get { return true; }
			set
			{
			}
		}

		public ReferenceLineGraphic this[int index]
		{
			get
			{
				for (int i = base.Graphics.Count; i <= index; ++i)
				{
					ReferenceLineGraphic newGraphic = new ReferenceLineGraphic();
					newGraphic.Changed += OnChildChanged;
					base.Graphics.Add(newGraphic);
				}

				return (ReferenceLineGraphic)base.Graphics[index];
			}
		}

		public void HideAllReferenceLines()
		{
			foreach (ReferenceLineGraphic child in base.Graphics)
				child.Visible = false;
		}

		public override void OnDrawing()
		{
			base.OnDrawing();
			_dirty = false;
		}

		private void OnChildChanged(object sender, EventArgs e)
		{
			_dirty = true;
		}
	}

	#endregion

	#region ReferenceLineGraphic class

	internal class ReferenceLineGraphic : CompositeGraphic
	{
		private readonly LinePrimitive _line;
		private readonly InvariantTextPrimitive _text;
		private PointF _point1;
		private PointF _point2;
		public event EventHandler _changed;

		public ReferenceLineGraphic()
		{
			base.Graphics.Add(_text = new InvariantTextPrimitive());
			base.Graphics.Add(_line = new LinePrimitive());

			_text.BoundingBoxChanged += OnTextBoundingBoxChanged;
		}

		private void OnTextBoundingBoxChanged(object sender, RectangleChangedEventArgs e)
		{
			KeepTextInsideClientRectangle();
		}

		private void KeepTextInsideClientRectangle()
		{
			this.CoordinateSystem = CoordinateSystem.Destination;

			PointF startPoint, endPoint;
			float lengthOfLineThroughTextBox;
			if (!GetTextBoxAdjustmentParameters(out startPoint, out endPoint, out lengthOfLineThroughTextBox))
			{
				_line.Pt1 = Point1;
				_line.Pt2 = Point2;
				_text.Location = Point1;

				this.ResetCoordinateSystem();
				return;
			}

			Vector3D lineDirection = new Vector3D(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, 0F);

			float ratioLengthOfLineThroughTextBox = lengthOfLineThroughTextBox / lineDirection.Magnitude;

			SizeF textEdgeOffset = new SizeF(ratioLengthOfLineThroughTextBox * lineDirection.X, ratioLengthOfLineThroughTextBox * lineDirection.Y);

			SizeF textAnchorPointOffset = new SizeF(textEdgeOffset.Width / 2F, textEdgeOffset.Height / 2F);

			Vector3D lineUnit = lineDirection.Normalize();

			// extend the endpoint of the line by the distance to the outside text edge.
			endPoint = PointF.Add(endPoint, textEdgeOffset);
			// add an additional 5 pixel offset so we don't push back as far as the start point.
			endPoint = PointF.Add(endPoint, new SizeF(5F * lineUnit.X, 5F * lineUnit.Y));

			SizeF clientEdgeOffset = Size.Empty;

			// find the intersection of the extended line segment and either of the left or bottom client edge.
			PointF? intersectionPoint = GetClientRightOrBottomEdgeIntersectionPoint(startPoint, endPoint);
			if (intersectionPoint != null)
			{
				Vector3D clientEdgeOffsetVector = new Vector3D(endPoint.X - intersectionPoint.Value.X, endPoint.Y - intersectionPoint.Value.Y, 0);
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
			_text.Location = endPoint;
			
			// offset the line by half again the distance necessary to keep the text box inside the client rectangle.
			_line.Pt2 = PointF.Subtract(endPoint, textAnchorPointOffset);

			this.ResetCoordinateSystem();
		}

		private bool GetTextBoxAdjustmentParameters(out PointF startPoint, out PointF endPoint, out float lengthOfLineThroughTextBox)
		{
			startPoint = Point1;
			endPoint = Point2;
			lengthOfLineThroughTextBox = 0;

			Vector3D lineDirection = new Vector3D(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, 0F);

			if (Vector3D.AreEqual(lineDirection, Vector3D.Null))
				return false;

			Vector3D lineUnit = lineDirection.Normalize();

			Vector3D xUnit = new Vector3D(1F, 0, 0);
			Vector3D yUnit = new Vector3D(0, 1F, 0);

			float cosThetaX = Math.Abs(xUnit.Dot(lineUnit));
			float cosThetaY = Math.Abs(yUnit.Dot(lineUnit));

			float textWidth = _text.BoundingBox.Width;
			float textHeight = _text.BoundingBox.Height;

			if (cosThetaX >= cosThetaY)
			{
				// the distance along the line to where we want the outside right edge of the text to be.
				lengthOfLineThroughTextBox = cosThetaX*textWidth;
				if (lineDirection.X < 0)
				{
					startPoint = Point2;
					endPoint = Point1;
				}
			}
			else
			{
				// the distance along the line to where we want the outside bottom edge of the text to be.
				lengthOfLineThroughTextBox = cosThetaY*textHeight;
				if (lineDirection.Y < 0)
				{
					startPoint = Point2;
					endPoint = Point1;
				}
			}

			return true;
		}

		private PointF? GetClientRightOrBottomEdgeIntersectionPoint(PointF lineSegmentStartPoint, PointF lineSegmentEndPoint)
		{
			Rectangle clientRectangle = base.ParentPresentationImage.ClientRectangle;

			PointF clientTopRight = new PointF(clientRectangle.Right, clientRectangle.Top);
			PointF clientBottomLeft = new PointF(clientRectangle.Left, clientRectangle.Bottom);
			PointF clientBottomRight = new PointF(clientRectangle.Right, clientRectangle.Bottom);

			PointF intersectionPoint;
			if (Vector.LineSegments.Intersect == 
			    Vector.LineSegmentIntersection(lineSegmentStartPoint, lineSegmentEndPoint, clientTopRight, clientBottomRight, out intersectionPoint))
			{
				return intersectionPoint;
			}
			else if (Vector.LineSegments.Intersect ==
			         Vector.LineSegmentIntersection(lineSegmentStartPoint, lineSegmentEndPoint, clientBottomLeft, clientBottomRight, out intersectionPoint))
			{
				return intersectionPoint;
			}
			else
			{
				return null;
			}
		}

		public override bool Visible
		{
			get { return base.Visible; }
			set
			{
				if (base.Visible == value)
					return;

				base.Visible = value;
				EventsHelper.Fire(_changed, this, EventArgs.Empty);
			}
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
				if (base.CoordinateSystem != CoordinateSystem.Source)
					value = base.SpatialTransform.ConvertToSource(value);

				if (value == _point1)
					return;

				_point1 = value;
				EventsHelper.Fire(_changed, this, EventArgs.Empty);
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
				
				if (base.CoordinateSystem != CoordinateSystem.Source)
					value = base.SpatialTransform.ConvertToSource(value);

				if (value == _point2)
					return;

				_point2 = value;
				EventsHelper.Fire(_changed, this, EventArgs.Empty);
			}
		}

		public string Text
		{
			get { return _text.Text; }
			set
			{
				if (value == _text.Text)
					return;

				_text.Text = value;
				EventsHelper.Fire(_changed, this, EventArgs.Empty);
			}	
		}

		public event EventHandler Changed
		{
			add { _changed += value; }
			remove { _changed -= value; }
		}

		public override void OnDrawing()
		{
			OnTextBoundingBoxChanged(this, null);
			base.OnDrawing();
		}
	}

	#endregion
}
