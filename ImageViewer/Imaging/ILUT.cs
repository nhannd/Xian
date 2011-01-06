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
	/// The basic definition of a Lut.
	/// </summary>
	public interface ILut
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
	}
}
