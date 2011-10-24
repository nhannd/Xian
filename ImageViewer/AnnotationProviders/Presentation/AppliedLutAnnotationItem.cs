#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	/// <summary>
	/// Describes whatever Lut is currently applied to a presentation image.
	/// </summary>
	/// <remarks>
	/// At first glance, you might think this belongs in the Dicom namespace within this project.
	/// However, the information in the Dicom header only applies to the initial presentation of
	/// the image and is not representative of the current state of the image in the viewport.
	/// The user could have changed the W/L, applied a custom Data Lut, or a Lut from a related
	/// Grayscale Presentation State object could be applied.
	/// </remarks>
	internal sealed class AppliedLutAnnotationItem : AnnotationItem
	{
		public AppliedLutAnnotationItem()
			: base("Presentation.AppliedLut", new AnnotationResourceResolver(typeof(AppliedLutAnnotationItem).Assembly))
		{
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage == null)
				return String.Empty;

			IVoiLutProvider image = presentationImage as IVoiLutProvider;

			if (image == null || !image.VoiLutManager.Enabled)
				return String.Empty;

			IVoiLut voiLut = image.VoiLutManager.VoiLut;
			if (voiLut == null)
				return String.Empty;

			return voiLut.GetDescription();
		}
	}
}
