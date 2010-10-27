#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
	/// <seealso cref="ILut"/>
	public interface IComposedLut : ILut
	{
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
