#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
		public enum ImageEdge { Left = 0, Top = 1, Right = 2, Bottom = 3 };
		private static readonly SizeF[] _edgeVectors = new SizeF[] { new SizeF(-1, 0), new SizeF(0, -1), new SizeF(1, 0), new SizeF(0, 1) };

		private ImageEdge _viewportEdge;
		
		public DirectionalMarkerAnnotationItem(ImageEdge viewportEdge)
			: base("Presentation.DirectionalMarkers." + viewportEdge.ToString(), new AnnotationResourceResolver(typeof(DirectionalMarkerAnnotationItem).Assembly))
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
			string markerText = "";

			if (presentationImage != null)
			{
				ISpatialTransformProvider associatedTransform = presentationImage as ISpatialTransformProvider;
				IImageSopProvider associatedDicom = presentationImage as IImageSopProvider;

				if (associatedDicom != null && associatedTransform != null)
				{
					SpatialTransform spatialTransform = associatedTransform.SpatialTransform as SpatialTransform;
					if (spatialTransform != null && associatedDicom.Frame.ImageOrientationPatient != null)
						markerText = GetAnnotationTextInternal(spatialTransform, associatedDicom.Frame.ImageOrientationPatient);
				}
			}

			return markerText;
		}

		/// <summary>
		/// Called by GetAnnotationText (and also by Unit Test code).  Making this function internal simply makes it easier
		/// to write unit tests for this class (don't have to implement a fake PresentationImage).
		/// </summary>
		/// <param name="imageTransform">the image transform</param>
		/// <param name="imageOrientationPatient">the image orientation patient (direction cosines)</param>
		/// <returns></returns>
		internal string GetAnnotationTextInternal(SpatialTransform imageTransform, ImageOrientationPatient imageOrientationPatient)
		{
			SizeF[] imageEdgeVectors = new SizeF[4];
			for (int i = 0; i < 4; ++i)
				imageEdgeVectors[i] = imageTransform.ConvertToDestination(_edgeVectors[i]);
			
			//find out which source image edge got transformed to coincide with this viewport edge.
			ImageEdge transformedEdge = GetTransformedEdge(imageEdgeVectors);

			//get the marker for the appropriate (source) image edge.
			return GetMarker(transformedEdge, imageOrientationPatient);
		}

		/// <summary>
		/// After undergoing the transformation, the input vectors are now in Destination (viewport) coordinates.
		/// This function determines which edge in the image has moved to this edge of the viewport.
		/// That edge's original marker (when there is no transform), becomes this edge's marker.
		/// </summary>
		/// <param name="transformedVectors">source vectors transformed to the destination coordinate system</param>
		/// <returns>the source image edge that has effectively moved to this edge</returns>
		private ImageEdge GetTransformedEdge(SizeF[] transformedVectors)
		{
			//the original (untransformed) vector for this viewport edge.
			SizeF thisViewportEdge = _edgeVectors[(int)_viewportEdge];

			//find out which edge in the source image has moved to this edge of the viewport.
			for (int index = 0; index < transformedVectors.Length; ++index)
			{
				//normalize the vector before comparing.
				SizeF transformedVector = transformedVectors[index];
				double magnitude = Math.Sqrt(transformedVector.Width * transformedVector.Width +
												transformedVector.Height * transformedVector.Height);

				transformedVector.Width = (float)Math.Round(transformedVector.Width / magnitude);
				transformedVector.Height = (float)Math.Round(transformedVector.Height / magnitude);

				//is it the same as the original vector for this edge?
				if (transformedVector == thisViewportEdge)
				{
					//return the image edge that has now moved to this edge of the viewport.
					return (ImageEdge)index;
				}
			}

			//this should never happen.
			throw new IndexOutOfRangeException(SR.ExceptionTransformedEdgeDoesNotMatch);
		}
		
		/// <summary>
		/// Determines the (untransformed) marker for a particular image edge.
		/// </summary>
		/// <param name="imageEdge">the edge (image coordinates)</param>
		/// <param name="imageOrientationPatient">the direction cosines of the image</param>
		/// <returns>a string representation of the direction (a 'marker')</returns>
		private string GetMarker(ImageEdge imageEdge, ImageOrientationPatient imageOrientationPatient)
		{
			bool negativeDirection = (imageEdge == ImageEdge.Left || imageEdge == ImageEdge.Top);
			bool rowValues = (imageEdge == ImageEdge.Left || imageEdge == ImageEdge.Right);

			string markerText = "";

			if (rowValues)
			{
				ImageOrientationPatient.Directions primary = imageOrientationPatient.GetPrimaryRowDirection(negativeDirection);
				ImageOrientationPatient.Directions secondary = imageOrientationPatient.GetSecondaryRowDirection(negativeDirection, 1);
				markerText += GetMarkerText(primary);
				markerText += GetMarkerText(secondary);
			}
			else
			{
				ImageOrientationPatient.Directions primary = imageOrientationPatient.GetPrimaryColumnDirection(negativeDirection);
				ImageOrientationPatient.Directions secondary = imageOrientationPatient.GetSecondaryColumnDirection(negativeDirection, 1);
				markerText += GetMarkerText(primary);
				markerText += GetMarkerText(secondary);
			}

			return markerText;
		}

		/// <summary>
		/// Converts an <see cref="ImageOrientationPatient.Directions"/> to a marker string.
		/// </summary>
		/// <param name="direction">the direction (patient based system)</param>
		/// <returns>marker text</returns>
		private string GetMarkerText(ImageOrientationPatient.Directions direction)
		{
			switch (direction)
			{
				case ImageOrientationPatient.Directions.Left:
					return SR.ValueDirectionalMarkersLeft;
				case ImageOrientationPatient.Directions.Right:
					return SR.ValueDirectionalMarkersRight;
				case ImageOrientationPatient.Directions.Head:
					return SR.ValueDirectionalMarkersHead;
				case ImageOrientationPatient.Directions.Foot:
					return SR.ValueDirectionalMarkersFoot;
				case ImageOrientationPatient.Directions.Anterior:
					return SR.ValueDirectionalMarkersAnterior;
				case ImageOrientationPatient.Directions.Posterior:
					return SR.ValueDirectionalMarkersPosterior;
			}

			return "";
		}
	}
}
