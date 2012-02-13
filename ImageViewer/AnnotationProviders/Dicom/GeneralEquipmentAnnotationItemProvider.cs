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
	public class GeneralEquipmentAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public GeneralEquipmentAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralEquipment", new AnnotationResourceResolver(typeof (GeneralEquipmentAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();

			AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					"Dicom.GeneralEquipment.DateOfLastCalibration",
					resolver,
					FrameDataRetrieverFactory.GetStringRetriever(DicomTags.DateOfLastCalibration),
					DicomDataFormatHelper.DateFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					"Dicom.GeneralEquipment.TimeOfLastCalibration",
					resolver,
					FrameDataRetrieverFactory.GetStringRetriever(DicomTags.TimeOfLastCalibration),
					DicomDataFormatHelper.TimeFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					"Dicom.GeneralEquipment.DeviceSerialNumber",
					resolver,
					FrameDataRetrieverFactory.GetStringRetriever(DicomTags.DeviceSerialNumber),
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					"Dicom.GeneralEquipment.InstitutionAddress",
					resolver,
					FrameDataRetrieverFactory.GetStringRetriever(DicomTags.InstitutionAddress),
					SingleLineStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					"Dicom.GeneralEquipment.InstitutionalDepartmentName",
					resolver,
					delegate(Frame frame) { return frame.ParentImageSop.InstitutionalDepartmentName; },
					SingleLineStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					"Dicom.GeneralEquipment.InstitutionName",
					resolver,
					delegate(Frame frame) { return frame.ParentImageSop.InstitutionName; },
					SingleLineStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					"Dicom.GeneralEquipment.Manufacturer",
					resolver,
					delegate(Frame frame) { return frame.ParentImageSop.Manufacturer; },
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					"Dicom.GeneralEquipment.ManufacturersModelName",
					resolver,
					delegate(Frame frame) { return frame.ParentImageSop.ManufacturersModelName; },
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string>
					(
					"Dicom.GeneralEquipment.StationName",
					resolver,
					delegate(Frame frame) { return frame.ParentImageSop.StationName; },
					DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
				new DicomAnnotationItem<string[]>
					(
					"Dicom.GeneralEquipment.SoftwareVersions",
					resolver,
					FrameDataRetrieverFactory.GetStringArrayRetriever(DicomTags.SoftwareVersions),
					DicomDataFormatHelper.StringListFormat
					)
				);
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}

		private static string SingleLineStringFormat(string input)
		{
			return string.Join(SR.SeparatorSingleLine, input.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries));
		}
	}
}