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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	[Cloneable(true)]
	public abstract class GeometricShutter
	{
		internal GeometricShutter() {}

		internal abstract void AddToGraphicsPath(GraphicsPath path);

		public GeometricShutter Clone()
		{
			return CloneBuilder.Clone(this) as GeometricShutter;
		}
	}

	[Cloneable(true)]
	public class CircularShutter : GeometricShutter
	{
		public CircularShutter(Point center, int radius)
		{
			this.Center = center;
			this.Radius = radius;
		}

		private CircularShutter() {}

		public readonly Point Center;
		public readonly int Radius;

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

		internal override void AddToGraphicsPath(GraphicsPath path)
		{
			path.AddEllipse(BoundingRectangle);
		}
	}

	[Cloneable(true)]
	public class RectangularShutter : GeometricShutter
	{
		public RectangularShutter(int left, int right, int top, int bottom)
			: this(new Rectangle(left, top, right - left, bottom - top)) {}

		public RectangularShutter(Rectangle rectangle)
		{
			this.Rectangle = RectangleUtilities.ConvertToPositiveRectangle(rectangle);
		}

		private RectangularShutter() {}

		public readonly Rectangle Rectangle;

		internal override void AddToGraphicsPath(GraphicsPath path)
		{
			path.AddRectangle(Rectangle);
		}
	}

	[Cloneable]
	public class PolygonalShutter : GeometricShutter
	{
		private readonly List<Point> _vertices;
		private readonly ReadOnlyCollection<Point> _readOnlyVertices;

		public PolygonalShutter(IEnumerable<Point> vertices)
		{
			_vertices = new List<Point>(vertices);
			_readOnlyVertices = new ReadOnlyCollection<Point>(_vertices);
		}

		private PolygonalShutter(PolygonalShutter source, ICloningContext context)
			: this(source._vertices) {}

		public ReadOnlyCollection<Point> Vertices
		{
			get { return _readOnlyVertices; }
		}

		internal override void AddToGraphicsPath(GraphicsPath path)
		{
			path.AddPolygon(_vertices.ToArray());
		}
	}
}