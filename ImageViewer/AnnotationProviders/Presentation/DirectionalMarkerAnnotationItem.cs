#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	/// <summary>
	/// Calculates the directional marker for a particular given edge of a viewport.  The algorithm works based
	/// on the fact that the source and destination (image & viewport) edges coincide exactly when there is no 
	/// transform applied (e.g. Left Edge corresponds to Left Edge, etc).  So, when a 2D transform is applied
	/// to a Dicom image (90 degree rotations only!) the marker at a particular edge will move to a different edge.
	/// For example, if the marker at the Left edge of the viewport is "Anterior" when there is no transformation
	/// applied, a horizontal flip will cause the "Anterior" marker to move to the Right Edge of the viewport.
	/// 
	/// The algorithm used here is quite simple:
	/// - 4 vectors, pointing to the 4 image edges (in source coordinates) are transformed to destination
	///   (viewport) coordinates.
	/// - Once in destination coordinates, the (image) edge that has now effectively moved to the viewport edge
	///   represented by this object is determined and its marker becomes the new marker for this viewport edge. 
	/// </summary>
	internal sealed class DirectionalMarkerAnnotationItem : AnnotationItem
	{
	    private readonly PatientOrientationHelper.ImageEdge _viewportEdge;

	    public DirectionalMarkerAnnotationItem(PatientOrientationHelper.ImageEdge viewportEdge)
			: base("Presentation.DirectionalMarkers." + viewportEdge, new AnnotationResourceResolver(typeof(DirectionalMarkerAnnotationItem).Assembly))
		{
		    _viewportEdge = viewportEdge;
		}

	    /// <summary>
		/// Gets the annotation text.
		/// </summary>
		/// <param name="presentationImage">the input presentation image.</param>
		/// <returns>the annotation text.</returns>
		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage != null)
			{
				var associatedTransform = presentationImage as ISpatialTransformProvider;
				var associatedDicom = presentationImage as IImageSopProvider;

				if (associatedDicom != null && associatedTransform != null)
				    return GetAnnotationTextInternal((SpatialTransform)associatedTransform.SpatialTransform, associatedDicom.Frame.ImageOrientationPatient);
			}

	        return String.Empty;
		}

        internal string GetAnnotationTextInternal(SpatialTransform spatialTransform, ImageOrientationPatient imageOrientationPatient)
        {
            var helper = new PatientOrientationHelper(spatialTransform, imageOrientationPatient);
            var edgeDirection = helper.GetEdgeDirection(_viewportEdge, PatientDirection.Component.Secondary);
            var translatedDirection = String.Empty;
            foreach (var directionComponent in edgeDirection)
                translatedDirection += GetMarkerText(directionComponent);

            return translatedDirection;
        }

		/// <summary>
        /// Converts direction values from <see cref="PatientDirection"/> to a marker string.
		/// </summary>
		/// <param name="direction">the direction (patient based system)</param>
		/// <returns>marker text</returns>
		private static string GetMarkerText(char direction)
		{
			switch (direction)
			{
				case PatientDirection.Left:
					return SR.ValueDirectionalMarkersLeft;
                case PatientDirection.Right:
					return SR.ValueDirectionalMarkersRight;
                case PatientDirection.Head:
					return SR.ValueDirectionalMarkersHead;
                case PatientDirection.Foot:
					return SR.ValueDirectionalMarkersFoot;
                case PatientDirection.Anterior:
					return SR.ValueDirectionalMarkersAnterior;
                case PatientDirection.Posterior:
					return SR.ValueDirectionalMarkersPosterior;
			}

			return "";
		}
	}
}
