#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.Iod;
using System.Drawing;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	internal sealed class DFOVAnnotationItem : AnnotationItem
	{
		public DFOVAnnotationItem()
			: base("Presentation.DFOV", new AnnotationResourceResolver(typeof(DFOVAnnotationItem).Assembly))
		{ 
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage == null)
				return String.Empty;
				
			IImageSopProvider imageSopProvider = presentationImage as IImageSopProvider;
			if (imageSopProvider  == null)
				return String.Empty;
				
			ISpatialTransformProvider spatialTransformProvider = presentationImage as ISpatialTransformProvider;
			if (spatialTransformProvider == null)
				return String.Empty;

			ImageSpatialTransform transform = spatialTransformProvider.SpatialTransform as ImageSpatialTransform;
			if (transform == null)
				return String.Empty;

			if (transform.RotationXY % 90 != 0)
				return SR.ValueNotApplicable;

			Frame frame = imageSopProvider.Frame;
			PixelSpacing normalizedPixelSpacing = frame.NormalizedPixelSpacing;
			if (normalizedPixelSpacing.IsNull)
				return String.Empty;

			RectangleF sourceRectangle = new RectangleF(0, 0, frame.Columns, frame.Rows);
			RectangleF destinationRectangle = transform.ConvertToDestination(sourceRectangle);
			destinationRectangle = RectangleUtilities.Intersect(destinationRectangle, presentationImage.ClientRectangle);

			//Convert the displayed width and height to source dimensions
			SizeF widthInSource = transform.ConvertToSource(new SizeF(destinationRectangle.Width, 0));
			SizeF heightInSource = transform.ConvertToSource(new SizeF(0, destinationRectangle.Height));

			//The displayed FOV is given by the magnitude of each line in source coordinates, but
			//for each of the 2 lines, one of x or y will be zero, so we can optimize.

			float x1 = Math.Abs(widthInSource.Width);
			float y1 = Math.Abs(widthInSource.Height);
			float x2 = Math.Abs(heightInSource.Width);
			float y2 = Math.Abs(heightInSource.Height);

			double displayedFieldOfViewX, displayedFieldOfViewY;

			if (x1 > y1) //the image is not rotated
			{
				displayedFieldOfViewX = x1 * normalizedPixelSpacing.Column / 10;
				displayedFieldOfViewY = y2 * normalizedPixelSpacing.Row / 10;
			}
			else //the image is rotated by 90 or 270 degrees
			{
				displayedFieldOfViewX = x2 * normalizedPixelSpacing.Column / 10;
				displayedFieldOfViewY = y1 * normalizedPixelSpacing.Row / 10;
			}

			return String.Format("{0:F1} x {1:F1} cm", displayedFieldOfViewX, displayedFieldOfViewY);
		}
	}
}
