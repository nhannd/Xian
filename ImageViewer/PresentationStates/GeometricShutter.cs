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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;
using System;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	/// <summary>
	/// A geometric shutter.
	/// </summary>
	[Cloneable(true)]
	public abstract class GeometricShutter : IEquatable<GeometricShutter>
	{
		internal GeometricShutter() {}

		internal abstract void AddToGraphicsPath(GraphicsPath path);

		/// <summary>
		/// Clones this geometric shutter.
		/// </summary>
		/// <returns>An identical clone.</returns>
		public abstract GeometricShutter Clone();

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public sealed override bool Equals(object obj)
		{
			if (obj is GeometricShutter)
				return ReferenceEquals(this, obj) || Equals((GeometricShutter)obj);

			return false;
		}

		#region IEquatable<GeometricShutter> Members

		public abstract bool Equals(GeometricShutter other);

		#endregion

	}

	/// <summary>
	/// A circular geometric shutter.
	/// </summary>
	[Cloneable(true)]
	public class CircularShutter : GeometricShutter, IEquatable<CircularShutter>
	{
		/// <summary>
		/// Constructs a new circular geometric shutter.
		/// </summary>
		/// <param name="center">The centre of the circle defining the shutter boundary.</param>
		/// <param name="radius">The radius of the circle defining the shutter boundary.</param>
		public CircularShutter(Point center, int radius)
		{
			this.Center = center;
			this.Radius = radius;
		}

		private CircularShutter() {}

		/// <summary>
		/// Gets the centre of the circle defining the shutter boundary.
		/// </summary>
		public readonly Point Center;

		/// <summary>
		/// Gets the radius of the circle defining the shutter boundary.
		/// </summary>
		public readonly int Radius;

		/// <summary>
		/// Gets the tightest rectangle that bounds the circle.
		/// </summary>
		public Rectangle BoundingRectangle
		{
			get
			{
				int x = Center.X - Radius;
				int y = Center.Y - Radius;
				int widthHeight = 2*Radius;
				return new Rectangle(x, y, widthHeight, widthHeight);
			}
		}

		public override GeometricShutter Clone()
		{
			return new CircularShutter(Center, Radius);
		}

		internal override void AddToGraphicsPath(GraphicsPath path)
		{
			path.AddEllipse(BoundingRectangle);
		}

		public override int GetHashCode()
		{
			int hash = 0x1EEFD25E;
			hash ^= Center.GetHashCode();
			hash ^= Radius.GetHashCode();
			return hash;
		}

		public override bool Equals(GeometricShutter other)
		{
			if (other is CircularShutter)
				return Equals((CircularShutter)other);

			return false;
		}

		#region IEquatable<CircularShutter> Members

		public bool Equals(CircularShutter other)
		{
			if (other == null)
				return false;

			return other.Radius == Radius && other.Center == Center;
		}

		#endregion

		public override string ToString()
		{
			return String.Format("Circular (C={0}, R={1})", Center, Radius);
		}
	}

	/// <summary>
	/// A rectangular geometric shutter.
	/// </summary>
	[Cloneable(true)]
	public class RectangularShutter : GeometricShutter, IEquatable<RectangularShutter>
	{
		/// <summary>
		/// Constructs a new rectangular geometric shutter.
		/// </summary>
		/// <param name="left">The left edge of the rectangle defining the shutter boundary.</param>
		/// <param name="right">The right edge of the rectangle defining the shutter boundary.</param>
		/// <param name="top">The top edge of the rectangle defining the shutter boundary.</param>
		/// <param name="bottom">The bottom edge of the rectangle defining the shutter boundary.</param>
		public RectangularShutter(int left, int right, int top, int bottom)
			: this(new Rectangle(left, top, right - left, bottom - top)) {}

		/// <summary>
		/// Constructs a new rectangular geometric shutter.
		/// </summary>
		/// <param name="rectangle">The rectangle defining the shutter boundary.</param>
		public RectangularShutter(Rectangle rectangle)
		{
			this.Rectangle = RectangleUtilities.ConvertToPositiveRectangle(rectangle);
		}

		private RectangularShutter() {}

		/// <summary>
		/// Gets the rectangle defining the shutter boundary.
		/// </summary>
		public readonly Rectangle Rectangle;

		public override GeometricShutter Clone()
		{
			return new RectangularShutter(Rectangle);
		}

		internal override void AddToGraphicsPath(GraphicsPath path)
		{
			path.AddRectangle(Rectangle);
		}

		public override int GetHashCode()
		{
			return 0x42FF54F2 ^ Rectangle.GetHashCode();
		}

		public override bool Equals(GeometricShutter other)
		{
			if (other is RectangularShutter)
				return Equals((RectangularShutter)other);

			return false;
		}

		#region IEquatable<RectangularShutter> Members

		public bool Equals(RectangularShutter other)
		{
			return other != null && other.Rectangle.Equals(Rectangle);
		}

		#endregion

		public override string ToString()
		{
			return String.Format("Rectangular ({0})", Rectangle);
		}
	}

	/// <summary>
	/// A polygonal geometric shutter.
	/// </summary>
	[Cloneable]
	public class PolygonalShutter : GeometricShutter, IEquatable<PolygonalShutter>
	{
		private readonly List<Point> _vertices;
		private readonly ReadOnlyCollection<Point> _readOnlyVertices;

		/// <summary>
		/// Constructs a new polygonal geometric shutter.
		/// </summary>
		/// <param name="vertices">An ordered list of vertices defining the shutter boundary.</param>
		public PolygonalShutter(IEnumerable<Point> vertices)
		{
			_vertices = new List<Point>(vertices ?? new Point[0]);
			_readOnlyVertices = new ReadOnlyCollection<Point>(_vertices);
		}

		private PolygonalShutter(PolygonalShutter source, ICloningContext context)
			: this(source._vertices) {}

		/// <summary>
		/// Gets an ordered list of vertices defining the shutter boundary.
		/// </summary>
		public ReadOnlyCollection<Point> Vertices
		{
			get { return _readOnlyVertices; }
		}

		public override GeometricShutter Clone()
		{
			return new PolygonalShutter(this, null);
		}

		internal override void AddToGraphicsPath(GraphicsPath path)
		{
			path.AddPolygon(_vertices.ToArray());
		}

		public override int GetHashCode()
		{
			int hash = 0X6B8DE6E8;
			foreach (Point point in Vertices)
				hash ^= point.GetHashCode();

			return hash;
		}

		public override bool Equals(GeometricShutter other)
		{
			if (other is PolygonalShutter)
				return Equals((PolygonalShutter)other);

			return false;
		}

		#region IEquatable<PolygonalShutter> Members

		public bool Equals(PolygonalShutter other)
		{
			if (other == null)
				return false;

			if (other._vertices.Count != _vertices.Count)
				return false;

			for (int i = 0; i < _vertices.Count; ++i)
			{
				if (!_vertices[i].Equals(other._vertices[i]))
					return false;
			}

			return true;
		}

		#endregion

		public override string ToString()
		{
			return String.Format("Polygonal ({0} vertices)", _vertices.Count);
		}
	}
}