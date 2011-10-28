#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Defines the properties and methods to access a color map that defines the mapping of single-channel input pixel values to ARGB color values.
	/// </summary>
	public interface IColorMap : IMemorable
	{
		/// <summary>
		/// Gets the first mapped pixel value.
		/// </summary>
		int FirstMappedPixelValue { get; }

		/// <summary>
		/// Gets the color map data as a lookup table.
		/// </summary>
		int[] Data { get; }

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		int MinInputValue { get; set; }

		/// <summary>
		/// Gets or sets the maximum input value.
		/// </summary>
		int MaxInputValue { get; set; }

		/// <summary>
		/// Gets the output ARGB color at a given input index.
		/// </summary>
		int this[int index] { get; }

		/// <summary>
		/// Fired when the color map has changed in some way.
		/// </summary>
		event EventHandler LutChanged;

		/// <summary>
		/// Gets a string key that identifies this particular color map's characteristics.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with <b>equality</b>, since some color maps can be
		/// dependent upon the actual image to which it belongs.
		/// </remarks>
		string GetKey();

		/// <summary>
		/// Gets an abbreviated description of the color map.
		/// </summary>
		string GetDescription();

		/// <summary>
		/// Creates a deep-copy of the <see cref="IColorMap"/>.
		/// </summary>
		/// <remarks>
		/// Implementations may return NULL from this method when appropriate.	
		/// </remarks>
		IColorMap Clone();
	}
}