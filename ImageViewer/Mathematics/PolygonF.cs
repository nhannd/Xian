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

#region softSurfer License (for polygon winding count code)

// Copyright 2001, softSurfer (www.softsurfer.com)
// This code may be freely used and modified for any purpose
// providing that this copyright notice is included with it.
// SoftSurfer makes no warranty for this code, and cannot be held
// liable for any real or imagined damage resulting from its use.
// Users of this code must verify correctness for their application.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Mathematics
{
	/// <summary>
	/// A class containing useful methods for computing with polygons defined on floating-point coordinates.
	/// </summary>
	public class PolygonF : IEnumerable<PointF>
	{
		private readonly IList<PointF> _vertices;
		private readonly RectangleF _boundingRect;
		private readonly PointF[] _vertexArray;
		private readonly bool _isComplex;

		/// <summary>
		/// Constructs a new polygon.
		/// </summary>
		/// <param name="vertices">A list of vertices defining the polygon.</param>
		public PolygonF(IEnumerable<PointF> vertices)
		{
			Platform.CheckForNullReference(vertices, "vertices");
			List<PointF> list = new List<PointF>(vertices);
			Platform.CheckTrue(list.Count >= 3, "A polygon must have at least 3 vertices.");

			// initialize vertex lists
			_vertices = list.AsReadOnly();
			_vertexArray = list.ToArray();

			// initialize side list
			_boundingRect = InitializeBoundingRectangle(_vertexArray);
			_isComplex = InitializeIsComplex(_vertexArray);
		}

		/// <summary>
		/// Gets a value indicating if the polygon is complex.
		/// </summary>
		public bool IsComplex
		{
			get { return _isComplex; }
		}

		/// <summary>
		/// Gets the number of vertices or sides of the polygon.
		/// </summary>
		public int CountVertices
		{
			get { return _vertices.Count; }
		}

		/// <summary>
		/// Gets the smallest rectangle that encloses the entirety of the polygon.
		/// </summary>
		public RectangleF BoundingRectangle
		{
			get { return _boundingRect; }
		}

		/// <summary>
		/// Gets a list of the vertices of the polygon.
		/// </summary>
		public IList<PointF> Vertices
		{
			get { return _vertices; }
		}

		/// <summary>
		/// Tests if a point lies within the boundaries of the polygon.
		/// </summary>
		/// <remarks>
		/// This method is indeterminate for points lying exactly on the
		/// boundaries of the polygon, and may test either true or false
		/// (but will not throw an exception either way). The distribution
		/// of results is approximately even, and thus this method can be
		/// used in statistical calculations.
		/// </remarks>
		/// <param name="point">The test point.</param>
		/// <returns>True if the polygon contains the given point; False otherwise.</returns>
		public bool Contains(PointF point)
		{
			if (!_boundingRect.Contains(point))
				return false;

			unsafe
			{
				fixed (PointF* vertices = _vertexArray)
				{
					return CountWindings(point, vertices, _vertexArray.Length) != 0;
				}
			}
		}

		/// <summary>
		/// Computes the area of the polygon.
		/// </summary>
		/// <returns>The area of the polygon in square units.</returns>
		public double ComputeArea()
		{
			CodeClock clock = new CodeClock();
			clock.Start();
			try
			{
				unsafe
				{
					fixed (PointF* vertices = _vertexArray)
					{
						if (!_isComplex)
							return ComputeSimpleArea(vertices, _vertexArray.Length);
						else
							return ComputeComplexArea(_boundingRect, vertices, _vertexArray.Length);
					}
				}
			}
			finally
			{
				clock.Stop();
				Trace.WriteLine(string.Format("{1} Polygon area calculation took {0:f4} ms", clock.Seconds*1000, _isComplex ? "Complex" : "Simple"), "Math.Polygons");
			}
		}

		/// <summary>
		/// Counts the number of counter-clockwise windings that the polygon makes around a given <paramref name="point">point</paramref>.
		/// </summary>
		/// <param name="point">The test point.</param>
		/// <returns>The number of counter-clockwise windings.</returns>
		public int CountWindings(PointF point)
		{
			unsafe
			{
				fixed (PointF* vertices = _vertexArray)
				{
					return CountWindings(point, vertices, _vertexArray.Length);
				}
			}
		}

		#region Math: Area

		private static unsafe double ComputeComplexArea(RectangleF bounds, PointF* vertices, int vertexCount)
		{
			// This algorithm is more computationally expensive and provides a weaker approximation of
			// the area of an n-polygon when compared with Formula.AreaOfPolygon, although it does
			// compute the desired area of a self-intersecting polygon.
			int areaInPixels = 0;
			for (float r = bounds.Left; r <= bounds.Right; r++)
			{
				for (float c = bounds.Top; c <= bounds.Bottom; c++)
				{
					if (CountWindings(new PointF(r, c), vertices, vertexCount) != 0)
						areaInPixels++;
				}
			}
			return areaInPixels;
		}

		/// <summary>
		/// Green's Theorem-derived formula to compute the area of the polygon.
		/// </summary>
		private static unsafe double ComputeSimpleArea(PointF* vertices, int vertexCount)
		{
			// technically, this formula does work for complex polygons, it's just that we define inside/outside a complex polygon differently.
			double result = 0;
			int point0 = vertexCount - 1;
			for (int point1 = 0; point1 < vertexCount; point0 = point1, point1++)
			{
				result += vertices[point0].X*vertices[point1].Y - vertices[point1].X*vertices[point0].Y;
			}
			return Math.Abs(result/2);
		}

		#endregion

		#region Math: Winding Count

		/// <summary>
		/// Counts the number of counter-clockwise windings that the polygon makes around a given <paramref name="pt">point</paramref>.
		/// </summary>
		/// <param name="pt">The test point.</param>
		/// <param name="vertices">A pointer to the array of vertices defining the polygon.</param>
		/// <param name="vertexCount">The number of vertices in the array.</param>
		/// <returns>The number of CCW windings.</returns>
		/// <seealso cref="http://softsurfer.com/Archive/algorithm_0103/algorithm_0103.htm"/>
		private static unsafe int CountWindings(PointF pt, PointF* vertices, int vertexCount)
		{
			int wn = 0; // the winding number counter

			// loop through all edges of the polygon
			for (int i = 0; i < vertexCount; i++)
			{
				// edge from vertices[i] to vertices[i+1]
				if (vertices[i].Y <= pt.Y)
				{
					// start y <= pt.y
					if (vertices[i + 1].Y > pt.Y) // an upward crossing
						if (IsLeft(vertices[i], vertices[i + 1], pt) > 0) // pt left of edge
							wn++; // have a valid up intersect
				}
				else
				{
					// start y > pt.y (no test needed)
					if (vertices[i + 1].Y <= pt.Y) // a downward crossing
						if (IsLeft(vertices[i], vertices[i + 1], pt) < 0) // pt right of edge
							--wn; // have a valid down intersect
				}
			}
			return wn;
		}

		/// <summary>Used by <see cref="CountWindings"/>.</summary>
		/// <seealso cref="http://softsurfer.com/Archive/algorithm_0103/algorithm_0103.htm"/>
		private static int IsLeft(PointF p0, PointF p1, PointF p2)
		{
			float result = (p1.X - p0.X)*(p2.Y - p0.Y) - (p2.X - p0.X)*(p1.Y - p0.Y);
			return FloatComparer.Compare(result, 0, 1);
		}

		#endregion

		#region Initializers

		private static RectangleF InitializeBoundingRectangle(PointF[] vertices)
		{
			return RectangleUtilities.ComputeBoundingRectangle(vertices);
		}

		private static bool InitializeIsComplex(PointF[] vertices)
		{
			unsafe
			{
				int vertexCount = vertices.Length;
				fixed (PointF* v = vertices)
				{
					// the line segments immediately preceding and succeeding a given segment are never considered as "intersecting"
					for (int n = 2; n < vertexCount - 1; n++)
					{
						for (int i = 0; i <= n - 2; i++)
						{
							PointF intersection;
							Vector.LineSegments type = Vector.LineSegmentIntersection(v[n], v[n+1], v[i], v[i+1], out intersection);
							if (type == Vector.LineSegments.Intersect)
								return true;
						}
					}

					// when checking against the last line segment, skip the first line segment (since they are adjacent)
					for (int i = 1; i < vertexCount - 2; i++) {
						PointF intersection;
						Vector.LineSegments type = Vector.LineSegmentIntersection(v[vertexCount-1], v[0], v[i], v[i + 1], out intersection);
						if (type == Vector.LineSegments.Intersect)
							return true;
					}
				}
			}

			return false;
		}

		#endregion

		#region Enumerators

		IEnumerator<PointF> IEnumerable<PointF>.GetEnumerator()
		{
			return _vertices.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<PointF>) this).GetEnumerator();
		}

		#endregion
	}
}