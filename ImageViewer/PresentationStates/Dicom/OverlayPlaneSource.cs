#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Enumeration to indicate the source of an <see cref="OverlayPlaneGraphic"/>.
	/// </summary>
	public enum OverlayPlaneSource
	{
		/// <summary>
		/// Indicates that the associated <see cref="OverlayPlaneGraphic"/> was defined in the image SOP or the image SOP referenced by the presentation state SOP.
		/// </summary>
		Image,

		/// <summary>
		/// Indicates that the associated <see cref="OverlayPlaneGraphic"/> was defined in the presentation state SOP.
		/// </summary>
		PresentationState,

		/// <summary>
		/// Indicates that the associated <see cref="OverlayPlaneGraphic"/> was user-created.
		/// </summary>
		User
	}
}