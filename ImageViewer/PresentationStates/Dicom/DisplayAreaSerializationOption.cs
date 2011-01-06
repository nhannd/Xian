#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	/// <summary>
	/// Enumerated values that specify how the displayed area of an image should be serialized in a presentation state.
	/// </summary>
	/// <remarks>
	/// <para>If the image with presentation state is being rendered in a screen rectangle of the same size as when the presentation
	/// state was created, then the displayed area is the same. However, this is the exception rather than the norm. The typical
	/// use case for presentation states would have the rendering of the image take place on a different workstation, typically
	/// with differing physical monitor size, screen resolution, window dimensions, window aspect ratio, etc. These enumerated
	/// values serve to specify which quality of the visible area should be preserved in such circumstances.</para>
	/// <para>An alternative description can be found in DICOM Standard 2008, PS 3.3 C.10.4.</para>
	/// </remarks>
	public enum DisplayAreaSerializationOption
	{
		/// <summary>
		/// Specifies that maintaining the same minimum visible area has higher precedence over maintaining the same magnification.
		/// </summary>
		/// <remarks>
		/// In cases where the aspect ratio of the screen rectangle where the image and presentation state is being rendered differs
		/// from that of the creator, then the magnification will be adjusted such that, at a minimum, the original visible area
		/// remains completely visible.
		/// </remarks>
		SerializeAsDisplayedArea,

		/// <summary>
		/// Specifies that maintaining the true physical size of the image has higher precedence over maintaining the same visible area.
		/// </summary>
		SerializeAsTrueSize,

		/// <summary>
		/// Specifies that maintaining the same magnification ratio has higher precedence over maintaining the same visible area.
		/// </summary>
		SerializeAsMagnification
	}
}