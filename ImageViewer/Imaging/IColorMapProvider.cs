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
	/// A provider of a color map, accessed and manipulated via the <see cref="ColorMapManager"/> property.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="ColorMapManager"/> property allows access to and manipulation of the installed color map.
	/// </para>
	/// <para>
	/// Implementors should not return null for the <see cref="ColorMapManager"/> property.
	/// </para>
	/// </remarks>
	/// <seealso cref="IColorMapManager"/>
	public interface IColorMapProvider : IDrawable
	{
		/// <summary>
		/// Gets the <see cref="IColorMapManager"/> associated with the provider.
		/// </summary>
		/// <remarks>
		/// This property should never return null.
		/// </remarks>
		IColorMapManager ColorMapManager { get; }
	}
}
