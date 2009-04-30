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
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines a <see cref="SpatialTransformValidationPolicy"/> for images.
	/// </summary>
	[Cloneable(true)]
	public class ImageTransformPolicy : SpatialTransformValidationPolicy
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ImageTransformPolicy()
		{
		}

		/// <summary>
		/// Performs validation on the specified <see cref="ISpatialTransform"/>.
		/// </summary>
		/// <param name="transform"></param>
		/// <remarks>
		/// At present, validation amounts to ensuring that rotations are
		/// only in increments of 90 degrees.
		/// </remarks>
		public override void Validate(ISpatialTransform transform)
		{
			PointF xVector = new PointF(100, 0);
			SizeF xVectorTransformed = ((SpatialTransform)transform).ConvertToDestination(new SizeF(xVector));

			//figure out where the source x-axis went in destination
			int rotation = (int) Math.Round(Vector.SubtendedAngle(xVectorTransformed.ToPointF(), PointF.Empty, xVector));
			if (rotation < 0)
				rotation += 360;

			if ((rotation % 90) != 0)
				throw new ArgumentException("Images can only be rotated by multiples of 90 degrees.");
		}
	}
}
