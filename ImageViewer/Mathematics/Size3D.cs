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

		//TODO (CR May09): unclear from the property name what this is.

		/// <summary>
		/// Returns the width * height * depth
		/// </summary>
		public int Size
		{
			get { return Width * Height * Depth; }
		}

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