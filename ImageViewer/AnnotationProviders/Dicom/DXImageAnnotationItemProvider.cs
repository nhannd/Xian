#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof (AnnotationItemProviderExtensionPoint))]
	public class DXImageAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public DXImageAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.DXImage", new AnnotationResourceResolver(typeof (DXImageAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();

			AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this is actually DX Positioning module
					"Dicom.DXImage.DetectorPrimaryAngle",
					resolver,
					f => FormatFloat64(f, DicomTags.DetectorPrimaryAngle, SR.FormatDegrees),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this is actually DX Positioning module
					"Dicom.DXImage.BodyPartThickness",
					resolver,
					f => string.Format(SR.FormatMillimeters, FormatFloat64(f, DicomTags.BodyPartThickness, "{0:F1}")),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this is actually DX Positioning module
					"Dicom.DXImage.CompressionForce",
					resolver,
					f => FormatInt32(f, DicomTags.CompressionForce, SR.FormatNewtons),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this is actually DX Series module
					"Dicom.DXImage.PresentationIntentType",
					resolver,
					f => f.ParentImageSop[DicomTags.PresentationIntentType].ToString(),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// whoa, this is actually DX Image module
					"Dicom.DXImage.AcquisitionDeviceProcessingDescription",
					resolver,
					f => f.ParentImageSop[DicomTags.AcquisitionDeviceProcessingDescription].ToString(),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this is actually DX Detector module
					"Dicom.DXImage.CassetteId",
					resolver,
					f => f.ParentImageSop[DicomTags.CassetteId].ToString(),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this is actually DX Detector module
					"Dicom.DXImage.PlateId",
					resolver,
					f => f.ParentImageSop[DicomTags.PlateId].ToString(),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this isn't even a DX module, but other X-Ray related modules
					"Dicom.DXImage.KVP",
					resolver,
					f => FormatFloat64(f, DicomTags.Kvp, SR.FormatKilovolts),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this isn't even a DX module, but other X-Ray related modules
					"Dicom.DXImage.ExposureInMas",
					resolver,
					f => FormatFloat64(f, DicomTags.ExposureInMas, SR.FormatMilliampSeconds),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					// this isn't even a DX module, but other X-Ray related modules
					"Dicom.DXImage.FilterMaterial",
					resolver,
					f => f.ParentImageSop[DicomTags.FilterMaterial].ToString(),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new CodeSequenceAnnotationItem
					(
					// this isn't even a DX module, but other X-Ray related modules
					"Dicom.DXImage.ContrastBolusAgent",
					resolver,
					DicomTags.ContrastBolusAgentSequence,
					DicomTags.ContrastBolusAgent
					)
				);
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}

		private static string FormatInt32(Frame frame, uint dicomTag, string formatString)
		{
			int value;
			bool tagExists = frame.ParentImageSop[dicomTag].TryGetInt32(0, out value);
			if (tagExists)
				return String.Format(formatString, value);

			return "";
		}

		private static string FormatFloat64(Frame frame, uint dicomTag, string formatString)
		{
			double value;
			bool tagExists = frame.ParentImageSop[dicomTag].TryGetFloat64(0, out value);
			if (tagExists)
				return String.Format(formatString, value);

			return "";
		}
	}
}