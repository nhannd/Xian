#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.Mathematics
{
	/// <summary>
	/// A simple 3D size class.
	/// </summary>
	/// <remarks>
	/// The Size3D class is immutable.  All necessary operations
	/// can be done via the operator overloads.
	/// </remarks>
	public class Size3D : IEquatable<Size3D>
	{
		private readonly int _width;
		private readonly int _height;
		private readonly int _depth;

		/// <summary>
		/// Constructor.
		/// </summary>
		public Size3D(int width, int height, int depth)
		{
			_width = width;
			_height = height;
			_depth = depth;
		}

		/// <summary>
		/// Copy Constructor.
		/// </summary>
		public Size3D(Size3D src)
		{
			_width = src.Width;
			_height = src.Height;
			_depth = src.Depth;
		}

		/// <summary>
		/// Gets the width
		/// </summary>
		public int Width
		{
			get { return _width; }
		}

		/// <summary>
		/// Gets the height
		/// </summary>
		public int Height
		{
			get { return _height; }
		}

		/// <summary>
		/// Gets the depth
		/// </summary>
		public int Depth
		{
			get { return _depth; }
		}

		/// <summary>
		/// Returns the width * height * depth
		/// </summary>
		public int Volume
		{
			get { return Width * Height * Depth; }
		}

	    /// TODO (CR Oct 2011): Need to override base Equals and GetHashCode, or make this a struct.
		#region IEquatable<Vector3D> Members

		/// <summary>
		/// Gets whether or not this object equals <paramref name="other"/>.
		/// </summary>
		public bool Equals(Size3D other)
		{
			if (other == null)
				return false;

			return (Width == other.Width && Height == other.Height && Depth == other.Depth);
		}

		#endregion
	}
}