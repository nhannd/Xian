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
	/// Defines a lookup table in the standard grayscale image display pipeline used to transform stored pixel values to manufacturer-independent values.
	/// </summary>
	/// <seealso cref="LutComposer"/>
	public interface IModalityLut : IComposableLut
	{
		/// <summary>
		/// Gets or sets the minimum input stored pixel value.
		/// </summary>
		/// <remarks>
		/// This value is set internally by the framework and should not be modified by client code.
		/// </remarks>
		new int MinInputValue { get; set; }

		/// <summary>
		/// Gets or sets the maximum input stored pixel value.
		/// </summary>
		/// <remarks>
		/// This value is set internally by the framework and should not be modified by client code.
		/// </remarks>
		new int MaxInputValue { get; set; }

		/// <summary>
		/// Gets the output value of the lookup table for a given input stored pixel value.
		/// </summary>
		double this[int input] { get; }

		/// <summary>
		/// Creates a deep-copy of the <see cref="IModalityLut"/>.
		/// </summary>
		/// <remarks>
		/// Implementations may return null from this method when appropriate.
		/// </remarks>
		new IModalityLut Clone();
	}
}