#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using Matrix2D=System.Drawing.Drawing2D.Matrix;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	partial class ShowAnglesTool
	{
		[Cloneable(true)]
		private class ShowAnglesToolGraphic : CompositeGraphic
		{
			[CloneIgnore]
			private PointsList _endPoints;

			private AngleCalloutGraphic _angleCalloutGraphic;
			private AngleCalloutGraphic _complementaryAngleCalloutGraphic;

			public ShowAnglesToolGraphic()
			{
				_endPoints = new PointsList(new PointF[] {PointF.Empty, PointF.Empty, PointF.Empty, PointF.Empty}, this);

				base.Graphics.Add(_angleCalloutGraphic = new AngleCalloutGraphic());
				base.Graphics.Add(_complementaryAngleCalloutGraphic = new AngleCalloutGraphic());
			}

			public void Set(PointF p1, PointF p2, PointF q1, PointF q2)
			{
				_endPoints[0] = p1;
				_endPoints[1] = p2;
				_endPoints[2] = q1;
				_endPoints[3] = q2;

				this.Update();
			}

			private double ComputeAngle()
			{
				this.CoordinateSystem = CoordinateSystem.Source;
				try
				{
					return Math.Abs(Vector.SubtendedAngle(_endPoints[1], _endPoints[0], _endPoints[3] + new SizeF(_endPoints[0]) - new SizeF(_endPoints[2])));
				}
				finally
				{
					this.ResetCoordinateSystem();
				}
			}

			private void Update()
			{
				const int threshold = 36;
				const float calloutOffset = 36;
				const float largerCalloutOffset = 64;

				double angle = this.ComputeAngle();
				string textAngle = string.Format(SR.ToolsMeasurementFormatDegrees, angle);
				string textComplementaryAngle = string.Format(SR.ToolsMeasurementFormatDegrees, 180 - angle);

				SetAngleCallout(_angleCalloutGraphic, string.Empty, PointF.Empty, PointF.Empty);
				SetAngleCallout(_complementaryAngleCalloutGraphic, string.Empty, PointF.Empty, PointF.Empty);

				this.CoordinateSystem = CoordinateSystem.Destination;
				try
				{
					PointF p1 = _endPoints[0];
					PointF p2 = _endPoints[1];
					PointF q1 = _endPoints[2];
					PointF q2 = _endPoints[3];
					PointF intersection;
					Vector.LineSegments result = Vector.LineSegmentIntersection(p1, p2, q1, q2, out intersection);

					if (result == Vector.LineSegments.Intersect)
					{
						// the line segments intersect onscreen, so draw callouts at intersection

						bool p1MeetsThreshold = Vector.Distance(p1, intersection) > threshold;
						bool p2MeetsThreshold = Vector.Distance(p2, intersection) > threshold;
						bool q1MeetsThreshold = Vector.Distance(q1, intersection) > threshold;
						bool q2MeetsThreshold = Vector.Distance(q2, intersection) > threshold;

						if (p2MeetsThreshold && q2MeetsThreshold)
							SetAngleCallout(_angleCalloutGraphic, textAngle, intersection, BisectAngle(p2, intersection, q2, angle > 30 ? calloutOffset : largerCalloutOffset));
						else if (p1MeetsThreshold && q1MeetsThreshold)
							SetAngleCallout(_angleCalloutGraphic, textAngle, intersection, BisectAngle(p1, intersection, q1, angle > 30 ? calloutOffset : largerCalloutOffset));

						if (p1MeetsThreshold && q2MeetsThreshold)
							SetAngleCallout(_complementaryAngleCalloutGraphic, textComplementaryAngle, intersection, BisectAngle(q2, intersection, p1, angle < 150 ? calloutOffset : largerCalloutOffset));
						else if (p2MeetsThreshold && q1MeetsThreshold)
							SetAngleCallout(_complementaryAngleCalloutGraphic, textComplementaryAngle, intersection, BisectAngle(q1, intersection, p2, angle < 150 ? calloutOffset : largerCalloutOffset));

						//if (p2MeetsThreshold && q2MeetsThreshold)
						//    _calloutRoseGraphic.AddCallout(textAngle, BisectAngle(p2, intersection, q2, angle > 30 ? calloutOffset : largerCalloutOffset));
						//else if (p1MeetsThreshold && q1MeetsThreshold)
						//    _calloutRoseGraphic.AddCallout(textAngle, BisectAngle(p1, intersection, q1, angle > 30 ? calloutOffset : largerCalloutOffset));

						//if (p1MeetsThreshold && q2MeetsThreshold)
						//    _calloutRoseGraphic.AddCallout(textComplementaryAngle, BisectAngle(q2, intersection, p1, angle < 150 ? calloutOffset : largerCalloutOffset));
						//else if (p2MeetsThreshold && q1MeetsThreshold)
						//    _calloutRoseGraphic.AddCallout(textComplementaryAngle, BisectAngle(q1, intersection, p2, angle < 150 ? calloutOffset : largerCalloutOffset));
					}
					else if (result == Vector.LineSegments.DoNotIntersect)
					{
						// the line segments do not intersect onscreen, so figure out where they would have intersected and decide from there

						intersection = Intersect(p1, p2, q1, q2);
					}
				}
				finally
				{
					this.ResetCoordinateSystem();
				}
			}

			private void SetAngleCallout(AngleCalloutGraphic callout, string text, PointF anchor, PointF location)
			{
				callout.Text = text;
				callout.Visible = !string.IsNullOrEmpty(text);
				callout.CoordinateSystem = this.CoordinateSystem;
				callout.AnchorPoint = anchor;
				callout.TextLocation = location;
				callout.ResetCoordinateSystem();
			}

			private static PointF Intersect(PointF p1, PointF p2, PointF q1, PointF q2)
			{
				// The computation is really just one of the two equations formed by:
				// [ a b ] [ s ]   [ x ]
				// [ c d ] [ t ]   [ y ]

				float a = p2.X - p1.X;
				float b = q1.X - q2.X;
				float c = p2.Y - p1.Y;
				float d = q1.Y - q2.Y;
				float x = q1.X - p1.X;
				float y = q1.Y - p1.Y;
				float s = (d*x - b*y)/(a*d - b*c);
				return new PointF(p1.X + s*(p2.X - p1.X), p1.Y + s*(p2.Y - p1.Y));
			}

			/// <summary>
			/// Bisects the inner angle formed by <paramref name="point1"/> <paramref name="point2"/> <paramref name="point3"/>.
			/// </summary>
			/// <remarks>
			/// <para>
			/// Based largely on <see cref="ProtractorRoiCalloutLocationStrategy"/>.
			/// </para>
			/// <para>
			/// The return value is a point within the angle such that the line segment
			/// from this point to <paramref name="point2"/> has the specified <paramref name="magnitude"/> and bisects the angle
			/// <paramref name="point1"/> <paramref name="point2"/> <paramref name="point3"/>.
			/// </para>
			/// </remarks>
			private static PointF BisectAngle(PointF point1, PointF point2, PointF point3, float magnitude)
			{
				PointF[] points = new PointF[] {point1, point3};
				using (Matrix2D rotation = new Matrix2D())
				{
					rotation.Rotate((float) (-Vector.SubtendedAngle(point1, point2, point3)/2 + 180));
					rotation.Translate(-point2.X, -point2.Y);
					rotation.TransformPoints(points);
				}

				Vector3D result = new Vector3D(points[0].X, points[0].Y, 0);
				if (FloatComparer.AreEqual(result.Magnitude, 0F, 0.01F))
					result = new Vector3D(-1, 0, 0);
				result = result/result.Magnitude*magnitude;

				return new PointF(point2.X - result.X, point2.Y - result.Y);
			}
		}

		[Cloneable]
		private class AngleCalloutGraphic : CalloutGraphic
		{
			public AngleCalloutGraphic()
			{
				this.ShowArrowhead = false;
				this.Color = Color.LemonChiffon;
				this.LineStyle = LineStyle.Dash;
			}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			/// <param name="source">The source object from which to clone.</param>
			/// <param name="context">The cloning context object.</param>
			protected AngleCalloutGraphic(AngleCalloutGraphic source, ICloningContext context) : base(source, context)
			{
				context.CloneFields(source, this);
			}

			public new string Text
			{
				get { return base.Text; }
				set { base.Text = value; }
			}

			protected override IControlGraphic InitializePointControlGraphic(IPointGraphic pointGraphic)
			{
				return new NullControlGraphic(pointGraphic);
			}

			protected override IControlGraphic InitializeTextControlGraphic(ITextGraphic textGraphic)
			{
				return new NullControlGraphic(textGraphic);
			}

			#region NullControlGraphic Class

			[Cloneable]
			private class NullControlGraphic : ControlGraphic
			{
				public NullControlGraphic(IGraphic graphic) : base(graphic) {}

				/// <summary>
				/// Cloning constructor.
				/// </summary>
				/// <param name="source">The source object from which to clone.</param>
				/// <param name="context">The cloning context object.</param>
				protected NullControlGraphic(NullControlGraphic source, ICloningContext context)
					: base(source, context)
				{
					context.CloneFields(source, this);
				}
			}

			#endregion
		}
	}
}