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
	/// A Composed Lut is one that has been created by combining multiple Luts together.
	/// </summary>
	/// <remarks>
	/// The <see cref="Data"/> property should be considered readonly and is only provided
	/// for fast (unsafe) iteration overy the array.  However, it also enforces that <see cref="IComposedLut"/>s
	/// be data Luts, which is important because the overall efficiency of the Lut pipeline is improved 
	/// substantially.
	/// </remarks>
	public interface IComposedLut
	{
		/// <summary>
		/// Gets the minimum input value.
		/// </summary>
		int MinInputValue { get; }

		/// <summary>
		/// Gets the maximum input value.
		/// </summary>
		int MaxInputValue { get; }

		/// <summary>
		/// Gets the minimum output value.
		/// </summary>
		int MinOutputValue { get; }

		/// <summary>
		/// Gets the maximum output value.
		/// </summary>
		int MaxOutputValue { get; }

		/// <summary>
		/// Gets the output value of the lut at a given input index.
		/// </summary>
		int this[int index] { get; }

		/// <summary>
		/// Gets the lut's data.
		/// </summary>
		/// <remarks>
		/// This property should be considered readonly and is only 
		/// provided for fast (unsafe) iteration over the array.
		/// </remarks>
		int[] Data { get; }
	}
}
