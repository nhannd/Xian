#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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