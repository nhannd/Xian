#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
	/// Defines a Lut that can be added to a <see cref="LutCollection"/>.
	/// </summary>
	/// <seealso cref="ILut"/>
	/// <seealso cref="IMemorable"/>
	public interface IComposableLut : ILut, IMemorable
	{
		/// <summary>
		/// Gets or sets the minimum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		new int MinInputValue { get; set; }

		/// <summary>
		/// Gets the maximum input value.
		/// </summary>
		/// <remarks>
		/// This value should not be modified by your code.  It will be set internally by the framework.
		/// </remarks>
		new int MaxInputValue { get; set; }

		/// <summary>
		/// Fired when the LUT has changed in some way.
		/// </summary>
		event EventHandler LutChanged;
		
		/// <summary>
		/// Gets a string key that identifies this particular LUT's characteristics, so that 
		/// an image's <see cref="IComposedLut"/> can be more efficiently determined.
		/// </summary>
		/// <remarks>
		/// This method is not to be confused with <b>equality</b>, since some Luts can be
		/// dependent upon the actual image to which it belongs.
		/// </remarks>
		string GetKey();

		/// <summary>
		/// Gets an abbreviated description of the Lut.
		/// </summary>
		string GetDescription();

		/// <summary>
		/// Creates a deep-copy of the <see cref="IComposableLut"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="IComposableLut"/>s may return null from this method when appropriate.	
		/// </remarks>
		IComposableLut Clone();
	}
}
