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
	/// A provider of a modality lut.
	/// </summary>
	/// <remarks>
	/// Implementors should not return null from the <see cref="ModalityLut"/> property.
	/// </remarks>
	public interface IModalityLutProvider
	{
		/// <summary>
		/// Gets the modality lut owned by the provider.
		/// </summary>
		/// <remarks>
		/// This property should never return null.
		/// </remarks>
		IModalityLut ModalityLut { get; }
	}
}
