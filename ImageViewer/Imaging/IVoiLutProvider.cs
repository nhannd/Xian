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
	/// A provider of a VOI LUT, accessed and manipulated via the <see cref="VoiLutManager"/> property.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <see cref="VoiLutManager"/> property allows access to and manipulation of the installed VOI LUT.
	/// </para>
	/// <para>
	/// Implementors should not return null for the <see cref="VoiLutManager"/> property.
	/// </para>
	/// </remarks>
	/// <seealso cref="IVoiLutManager"/>
	public interface IVoiLutProvider : IDrawable
	{
		/// <summary>
		/// Gets the <see cref="IVoiLutManager"/> associated with the provider.
		/// </summary>
		/// <remarks>
		/// This property should never return null.
		/// </remarks>
		IVoiLutManager VoiLutManager { get; }
	}
}