#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Defines a lookup table in the standard grayscale image display pipeline used to transform stored pixel values to manufacturer-independent values implemented as an array of values.
	/// </summary>
	/// <seealso cref="LutComposer"/>
	/// <see cref="IModalityLut"/>
	public interface IDataModalityLut : IModalityLut
	{
		/// <summary>
		/// Gets the first mapped pixel value.
		/// </summary>
		int FirstMappedPixelValue { get; }

		/// <summary>
		/// Gets the lookup table data.
		/// </summary>
		double[] Data { get; }

		/// <summary>
		/// Creates a deep-copy of the <see cref="IDataModalityLut"/>.
		/// </summary>
		/// <remarks>
		/// Implementations may return NULL from this method when appropriate.
		/// </remarks>
		new IDataModalityLut Clone();
	}
}