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
	/// Defines a lookup table in the standard grayscale image display pipeline used to select a range from manufacturer-independent values for display.
	/// </summary>
	/// <seealso cref="LutComposer"/>
	public interface IVoiLut : IComposableLut
	{
		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		new int MinOutputValue { get; }

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		new int MaxOutputValue { get; }

		/// <summary>
		/// Gets the output value of the lookup table for a given input value.
		/// </summary>
		new int this[double input] { get; }

		/// <summary>
		/// Creates a deep-copy of the <see cref="IVoiLut"/>.
		/// </summary>
		/// <remarks>
		/// Implementations may return null from this method when appropriate.
		/// </remarks>
		new IVoiLut Clone();
	}
}