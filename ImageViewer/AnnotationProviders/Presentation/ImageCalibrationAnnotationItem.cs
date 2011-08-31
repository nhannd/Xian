#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	internal sealed class ImageCalibrationAnnotationItem : AnnotationItem
	{
		public ImageCalibrationAnnotationItem()
			: base("Presentation.ImageCalibration", new AnnotationResourceResolver(typeof (ImageCalibrationAnnotationItem).Assembly)) {}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			var imageSopProvider = presentationImage as IImageSopProvider;
			if (imageSopProvider == null)
				return string.Empty;

			var details = imageSopProvider.Frame.NormalizedPixelSpacing.CalibrationDetails;
			switch (imageSopProvider.Frame.NormalizedPixelSpacing.CalibrationType)
			{
				case NormalizedPixelSpacingCalibrationType.None:
					return string.Empty;
				case NormalizedPixelSpacingCalibrationType.Manual:
					return SR.ValueManualCalibration;
				case NormalizedPixelSpacingCalibrationType.CrossSectionalSpacing:
					return SR.ValueActualSpacingCalibration;
				case NormalizedPixelSpacingCalibrationType.Detector:
					return SR.ValueDetectorSpacingCalibration;
				case NormalizedPixelSpacingCalibrationType.Geometry:
					return FormatCalibrationDetails(SR.ValueGeometricCalibration, details);
				case NormalizedPixelSpacingCalibrationType.Fiducial:
					return FormatCalibrationDetails(SR.ValueFiducialCalibration, details);
				case NormalizedPixelSpacingCalibrationType.Magnified:
					return FormatCalibrationDetails(SR.ValueMagnifiedCalibration, details);
				case NormalizedPixelSpacingCalibrationType.Unknown:
				default:
					return FormatCalibrationDetails(SR.ValueUnknownCalibration, details);
			}
		}

		private static string FormatCalibrationDetails(string calibration, string details)
		{
			if (string.IsNullOrEmpty(details))
				return calibration;
			return string.Format(SR.FormatCalibrationDetails, calibration, details);
		}
	}
}