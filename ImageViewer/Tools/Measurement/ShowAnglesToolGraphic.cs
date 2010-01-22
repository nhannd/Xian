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
		[Cloneable]
		private class ShowAnglesToolGraphic : CompositeGraphic
		{
			private const int _minLength = 36;

			[CloneIgnore]
			private readonly PointsList _endPoints;

			[CloneIgnore]
			private AngleCalloutGraphic _angleCalloutGraphic1;

			[CloneIgnore]
			private AngleCalloutGraphic _angleCalloutGraphic2;

			[CloneIgnore]
			private ILineSegmentGraphic _extenderLine1;

			[CloneIgnore]
			private ILineSegmentGraphic _extenderLine2;

			[CloneIgnore]
			private ILineSegmentGraphic _riserLine1;

			[CloneIgnore]
			private ILineSegmentGraphic _riserLine2;

			public ShowAnglesToolGraphic()
			{
				base.Graphics.Add(_angleCalloutGraphic1 = new AngleCalloutGraphic());
				base.Graphics.Add(_angleCalloutGraphic2 = new AngleCalloutGraphic());
				base.Graphics.Add(_extenderLine1 = new LinePrimitive());
				base.Graphics.Add(_extenderLine2 = new LinePrimitive());
				base.Graphics.Add(_riserLine1 = new LinePrimitive());
				base.Graphics.Add(_riserLine2 = new LinePrimitive());

				_angleCalloutGraphic1.ShowArrowhead = _angleCalloutGraphic2.ShowArrowhead = false;
				_angleCalloutGraphic1.Color = _angleCalloutGraphic2.Color = Color.LightGoldenrodYellow;
				_angleCalloutGraphic1.LineStyle = _angleCalloutGraphic2.LineStyle = LineStyle.Dash;
				_extenderLine1.LineStyle = _extenderLine2.LineStyle = LineStyle.Dot;
				_extenderLine1.Color = _riserLine1.Color = Color.LightGoldenrodYellow;
				_riserLine1.LineStyle = _riserLine2.LineStyle = LineStyle.Dot;
				_riserLine2.Color = _extenderLine2.Color = Color.LimeGreen;

				_endPoints = new PointsList(new PointF[] {PointF.Empty, PointF.Empty, PointF.Empty, PointF.Empty}, this);
			}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			/// <param name="source">The source object from which to clone.</param>
			/// <param name="context">The cloning context object.</param>
			protected ShowAnglesToolGraphic(ShowAnglesToolGraphic source, ICloningContext context) : base()
			{
				context.CloneFields(source, this);

				_endPoints = new PointsList(source._endPoints, this);
			}

			[OnCloneComplete]
			private void OnCloneComplete() {}

			public void Set(PointF p1, PointF p2, PointF q1, PointF q2)
			{
				_endPoints[0] = p1;
				_endPoints[1] = p2;
				_endPoints[2] = q1;
				_endPoints[3] = q2;

				this.Update();
			}

			private void Update()
			{
				DrawAngleCallout(_angleCalloutGraphic1, string.Empty, PointF.Empty, PointF.Empty);
				DrawAngleCallout(_angleCalloutGraphic2, string.Empty, PointF.Empty, PointF.Empty);
				_extenderLine1.Visible = _extenderLine2.Visible = false;
				_riserLine1.Visible = _riserLine2.Visible = false;

				if (this.ParentPresentationImage == null)
					return;

				this.CoordinateSystem = CoordinateSystem.Destination;
				try
				{
					PointF p1 = _endPoints[0];
					PointF p2 = _endPoints[1];
					PointF q1 = _endPoints[2];
					PointF q2 = _endPoints[3];
					RectangleF bounds = base.ParentPresentationImage.ClientRectangle;

					if (!RestrictLine(ref p1, ref p2, bounds) || !RestrictLine(ref q1, ref q2, bounds))
						return;

					PointF intersection;
					Vector.LineSegments result = Vector.LineSegmentIntersection(p1, p2, q1, q2, out intersection);

					if (result == Vector.LineSegments.Intersect)
					{
						// the line segments intersect onscreen, so just draw callouts at the intersection
						DrawIntersection(p1, p2, q1, q2, intersection);
					}
					else if (result == Vector.LineSegments.DoNotIntersect)
					{
						const int threshold = 36;
						// the line segments do not intersect onscreen, so figure out where they would have intersected and decide from there
						intersection = Intersect(p1, p2, q1, q2);

						if (base.ParentPresentationImage.ClientRectangle.Contains(Point.Round(intersection)))
						{
							DrawExtenderLine(_extenderLine1, ref p1, ref p2, intersection);
							DrawExtenderLine(_extenderLine2, ref q1, ref q2, intersection);

							DrawIntersection(p1, p2, q1, q2, intersection);
						}
						else
						{
							// the virtual intersection isn't onscreen either - average the points to find a suitable intersection onscreen
							intersection = new PointF((p1.X + p2.X + q1.X + q2.X)/4, (p1.Y + p2.Y + q1.Y + q2.Y)/4);

							PointF pV = DrawExtenderLine(_extenderLine1, ref p1, ref p2, intersection);
							PointF qV = DrawExtenderLine(_extenderLine2, ref q1, ref q2, intersection);

							DrawRiserLine(_riserLine1, ref p1, ref p2, pV, intersection);
							DrawRiserLine(_riserLine2, ref q1, ref q2, qV, intersection);

							DrawIntersection(p1, p2, q1, q2, intersection);
						}
					}
				}
				finally
				{
					this.ResetCoordinateSystem();
				}
			}

			private void DrawIntersection(PointF p1, PointF p2, PointF q1, PointF q2, PointF intersection)
			{
				const float calloutOffset = 36;
				const float largerCalloutOffset = 64;

				double angle = this.ComputeAngle();
				string textAngle = string.Format(SR.ToolsMeasurementFormatDegrees, angle);
				string textComplementaryAngle = string.Format(SR.ToolsMeasurementFormatDegrees, 180 - angle);

				Console.WriteLine("Prime Angle: {0:f2}", angle);

				bool p1MeetsThreshold = Vector.Distance(p1, intersection) >= _minLength;
				bool p2MeetsThreshold = Vector.Distance(p2, intersection) >= _minLength;
				bool q1MeetsThreshold = Vector.Distance(q1, intersection) >= _minLength;
				bool q2MeetsThreshold = Vector.Distance(q2, intersection) >= _minLength;

				if (p2MeetsThreshold && q2MeetsThreshold)
					DrawAngleCallout(_angleCalloutGraphic1, textAngle, intersection, BisectAngle(p2, intersection, q2, angle > 30 ? calloutOffset : largerCalloutOffset));
				else if (p1MeetsThreshold && q1MeetsThreshold)
					DrawAngleCallout(_angleCalloutGraphic1, textAngle, intersection, BisectAngle(p1, intersection, q1, angle > 30 ? calloutOffset : largerCalloutOffset));

				if (p1MeetsThreshold && q2MeetsThreshold)
					DrawAngleCallout(_angleCalloutGraphic2, textComplementaryAngle, intersection, BisectAngle(q2, intersection, p1, angle < 150 ? calloutOffset : largerCalloutOffset));
				else if (p2MeetsThreshold && q1MeetsThreshold)
					DrawAngleCallout(_angleCalloutGraphic2, textComplementaryAngle, intersection, BisectAngle(q1, intersection, p2, angle < 150 ? calloutOffset : largerCalloutOffset));
			}

			private static void DrawRiserLine(ILineSegmentGraphic riser, ref PointF p1, ref PointF p2, PointF pV, PointF intersection)
			{
				PointF pW = ExtendRay(pV, intersection, _minLength + 1);
				if (Determinant(pW.X - pV.X, pW.Y - pV.Y, p2.X - p1.X, p2.Y - p1.Y) < 0)
				{
					PointF temp = pW;
					pW = pV;
					pV = temp;
				}
				riser.Visible = true;
				riser.Point1 = p1 = pV;
				riser.Point2 = p2 = pW;
			}

			private static PointF DrawExtenderLine(ILineSegmentGraphic extender, ref PointF p1, ref PointF p2, PointF intersection)
			{
				float pF = DotProduct(p1, intersection, p1, p2)/DotProduct(p1, p2, p1, p2);
				PointF pC = p1 + new SizeF(pF*(p2.X - p1.X), pF*(p2.Y - p1.Y));

				if (pF > 1)
				{
					extender.Visible = true;
					extender.Point1 = p2;
					extender.Point2 = p2 = pC;
				}
				else if (pF < 0)
				{
					extender.Visible = true;
					extender.Point2 = p1;
					extender.Point1 = p1 = pC;
				}

				return pC;
			}

			private static void DrawAngleCallout(AngleCalloutGraphic callout, string text, PointF anchor, PointF location)
			{
				callout.Text = text;
				callout.Visible = !string.IsNullOrEmpty(text);
				callout.AnchorPoint = anchor;
				callout.TextLocation = location;
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

			private static bool RestrictLine(ref PointF p1, ref PointF p2, RectangleF bounds)
			{
				bool p1Inside = bounds.Contains(p1);
				bool p2Inside = bounds.Contains(p2);

				if (p1Inside && p2Inside)
					return true;

				PointF[] sides = new PointF[] {new PointF(bounds.Left, bounds.Top), new PointF(bounds.Right, bounds.Top), new PointF(bounds.Right, bounds.Bottom), new PointF(bounds.Left, bounds.Bottom)};

				for (int n = 0; n < 4; n++)
				{
					PointF intersection;
					if (Vector.LineSegmentIntersection(p1, p2, sides[n], sides[(n + 1)%4], out intersection) == Vector.LineSegments.Intersect)
					{
						if (p1Inside)
							p2 = intersection;
						else if (p2Inside)
							p1 = intersection;
						return true;
					}
				}
				return false;
			}

			private static float Determinant(float m11, float m12, float m21, float m22)
			{
				return m11*m22 - m12*m21;
			}

			private static float DotProduct(PointF p1, PointF p2, PointF q1, PointF q2)
			{
				return (p2.X - p1.X)*(q2.X - q1.X) + (p2.Y - p1.Y)*(q2.Y - q1.Y);
			}

			private static PointF ExtendRay(PointF p1, PointF p2, float magnitude)
			{
				PointF v = p2 - new SizeF(p1);
				float factor = 1+(float) (magnitude/Math.Sqrt((v.X*v.X + v.Y*v.Y)));
				return p1 + new SizeF(v.X*factor, v.Y*factor);
			}

			private static PointF ClosestPointOnLine(PointF p1, PointF p2, PointF pX)
			{
				PointF v = p2 - new SizeF(p1);
				float factor = ((pX.X - p1.X)*v.X + (pX.Y - p1.Y)*v.Y)/(v.X*v.X + v.Y*v.Y);
				return p1 + new SizeF(v.X*factor, v.Y*factor);
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
			public AngleCalloutGraphic() {}

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