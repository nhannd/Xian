#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer {
	/// <summary>
	/// The interface for all DICOM-based Presentation Images.
	/// </summary>
	public interface IDicomPresentationImage :
		IPresentationImage,
		IImageSopProvider,
		IAnnotationLayoutProvider,
		IImageGraphicProvider,
		IApplicationGraphicsProvider,
		IOverlayGraphicsProvider,
		ISpatialTransformProvider,
		IPresentationStateProvider
		
	{
		/// <summary>
		/// Gets direct access to the presentation image's collection of domain-level graphics.
		/// Consider using <see cref="DicomGraphicsPlane.GetDicomGraphicsPlane(IDicomPresentationImage)"/> instead.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Use <see cref="DicomGraphics"/> to add DICOM-defined graphics that you want to
		/// overlay the image at the domain-level. These graphics are rendered
		/// before any <see cref="IApplicationGraphicsProvider.ApplicationGraphics"/>
		/// and before any <see cref="IOverlayGraphicsProvider.OverlayGraphics"/>.
		/// </para>
		/// <para>
		/// This property gives direct access to all the domain-level graphics of a DICOM presentation image.
		/// However, most of the graphics concepts defined in the DICOM Standard are already supported
		/// by the <see cref="DicomGraphicsPlane"/> which inserts itself into this domain-level collection.
		/// Consider using <see cref="DicomGraphicsPlane.GetDicomGraphicsPlane(IDicomPresentationImage)"/> to get
		/// a reference to a usable DicomGraphicsPlane object instead, since that provides all the logical support
		/// for layer activation and shutters in addition to enumerating all domain-level graphics. This property
		/// may change, be deprecated, and even outright removed in a future framework release.
		/// </para>
		/// </remarks>
		GraphicCollection DicomGraphics { get; }
	}
}
