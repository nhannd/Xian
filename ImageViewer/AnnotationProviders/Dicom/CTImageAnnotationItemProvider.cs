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
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class CTImageAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public CTImageAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.CTImage", new AnnotationResourceResolver(typeof(CTImageAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();

			AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.CTImage.KVP",
						resolver,
						delegate(Frame frame)
						{
							double value;
							bool tagExists = frame.ParentImageSop[DicomTags.Kvp].TryGetFloat64(0, out value);
							if (tagExists)
								return String.Format(SR.FormatkV, value);

							return "";
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.CTImage.XRayTubeCurrent",
						resolver,
						delegate(Frame frame)
						{
							int value;
							bool tagExists = frame.ParentImageSop[DicomTags.XRayTubeCurrent].TryGetInt32(0, out value);
							if (tagExists)
								return String.Format(SR.FormatmA, value);

							return "";
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.CTImage.GantryDetectorTilt",
						resolver,
						delegate(Frame frame)
						{
							double value;
							bool tagExists = frame.ParentImageSop[DicomTags.GantryDetectorTilt].TryGetFloat64(0, out value);
							if (tagExists)
								return String.Format(SR.FormatDegrees, value);

							return "";
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.CTImage.ExposureTime",
						resolver,
						delegate(Frame frame)
						{
							int value;
							bool tagExists = frame.ParentImageSop[DicomTags.ExposureTime].TryGetInt32(0, out value);
							if (tagExists)
								return String.Format(SR.Formatms, value);

							return "";
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.CTImage.ConvolutionKernel",
						resolver,
						delegate(Frame frame)
						{
							string value;
							value = frame.ParentImageSop[DicomTags.ConvolutionKernel].GetString(0, null);
							return value;
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}
	}
}
