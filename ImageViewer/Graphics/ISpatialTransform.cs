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

using System.Drawing;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Defines a 2D spatial transformation.
	/// </summary>
	public interface ISpatialTransform : IMemorable
	{
		/// <summary>
		/// Gets or sets a value indicating that the object is flipped vertically
		/// (i.e. mirrored on the x-axis).
		/// </summary>
		bool FlipX { get; set; }

		/// <summary>
		/// Gets or sets a value indicating that the object is flipped horizontally
		/// (i.e. mirrored on the y-axis).
		/// </summary>
		bool FlipY { get; set; }

		/// <summary>
		/// Gets or sets the rotation.
		/// </summary>
		int RotationXY { get; set; }

		/// <summary>
		/// Gets or sets the scale.
		/// </summary>
		float Scale { get; set; }
		
		/// <summary>
		/// Gets or sets the translation in the x-direction.
		/// </summary>
		float TranslationX { get; set; }

		/// <summary>
		/// Gets or sets the translation in the y-direction.
		/// </summary>
		float TranslationY { get; set; }

		/// <summary>
		/// Gets or sets the center of rotation.
		/// </summary>
		/// <remarks>
		/// The point should be specified in terms of the coordinate system
		/// of the parent graphic, i.e. source coordinates.
		/// </remarks>
		PointF CenterOfRotationXY { get; set; }

		/// <summary>
		/// Gets the cumulative scale.
		/// </summary>
		/// <remarks>
		/// Gets the scale relative to the root of the scene graph.
		/// </remarks>
		float CumulativeScale { get; }
	}
}
