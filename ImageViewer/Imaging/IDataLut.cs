#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// Defines the properties and methods of a lookup table mapping input pixel values to output pixel values.
	/// </summary>
	public interface IDataLut
	{
		/// <summary>
		/// Gets the first mapped pixel value.
		/// </summary>
		int FirstMappedPixelValue { get; }

		/// <summary>
		/// Gets the lookup table data.
		/// </summary>
		int[] Data { get; }

		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		int MinInputValue { get; set; }

		/// <summary>
		/// Gets or sets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		int MaxInputValue { get; set; }

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		int MinOutputValue { get; }

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		int MaxOutputValue { get; }

		/// <summary>
		/// Gets the output value of the lookup table at a given input index.
		/// </summary>
		int this[int index] { get; }

		/// <summary>
		/// Fired when the lookup table has changed in some way.
		/// </summary>
		event EventHandler LutChanged;

		/// <summary>
		/// Gets a string key that identifies this particular lookup table's characteristics.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with <b>equality</b>, since some lookup tables can be
		/// dependent upon the actual image to which it belongs.
		/// </remarks>
		string GetKey();

		/// <summary>
		/// Gets an abbreviated description of the lookup table.
		/// </summary>
		string GetDescription();

		/// <summary>
		/// Creates a deep-copy of the <see cref="IDataLut"/>.
		/// </summary>
		/// <remarks>
		/// Implementations may return NULL from this method when appropriate.
		/// </remarks>
		IDataLut Clone();
	}
}