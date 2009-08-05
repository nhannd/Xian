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

using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <summary>
	/// Defines the parameters for a particular slicing of a <see cref="Volume"/> object (i.e. plane boundaries, orientation, thickness, etc.)
	/// </summary>
	public interface IVolumeSlicerParams
	{
		/// <summary>
		/// Gets or sets a string describing the slicing parameters.
		/// </summary>
		string Description { get; set; }

		/// <summary>
		/// Gets or sets the rotation applied to the slicing plane as a 4x4 affine transform <see cref="Matrix"/>.
		/// </summary>
		/// <remarks>
		/// <para>Implementations may choose to return a new object instance each time to ensure immutability if <see cref="IsReadOnly"/> is true.</para>
		/// </remarks>
		Matrix SlicingPlaneRotation { get; set; }

		/// <summary>
		/// Gets or sets the point, in patient coordinates, though which the slicing should begin.
		/// </summary>
		/// <remarks>
		/// <para>Implementations may choose to return a new object instance each time to ensure immutability if <see cref="IsReadOnly"/> is true.</para>
		/// </remarks>
		Vector3D SliceThroughPointPatient { get; set; }

		/// <summary>
		/// Gets or sets the interpolation mode used in slicing a <see cref="Volume"/>.
		/// </summary>
		VolumeSlicerInterpolationMode InterpolationMode { get; set; }

		/// <summary>
		/// Gets or sets the physical width of the slicing plane.
		/// </summary>
		float SliceExtentXMillimeters { get; set; }

		/// <summary>
		/// Gets or sets the physical height of the slicing plane.
		/// </summary>
		float SliceExtentYMillimeters { get; set; }

		/// <summary>
		/// Gets or sets the physical spacing, in millimetres, between consecutive slices.
		/// </summary>
		float SliceSpacing { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether or not these parameters are immutable.
		/// </summary>
		bool IsReadOnly { get; }
	}

	/// <summary>
	/// Enumerated values for specifying the interpolation mode used in slicing a <see cref="Volume"/>.
	/// </summary>
	public enum VolumeSlicerInterpolationMode
	{
		NearestNeighbor,
		Linear,
		Cubic
	}
}