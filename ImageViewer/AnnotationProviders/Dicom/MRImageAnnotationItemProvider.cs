#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
	public class MRImageAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public MRImageAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.MRImage", new AnnotationResourceResolver(typeof(MRImageAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();

			AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.EchoTime",
						resolver,
						delegate(Frame frame)
						{
							double value;
							bool tagExists = frame.ParentImageSop[DicomTags.EchoTime].TryGetFloat64(0, out value);
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
						"Dicom.MRImage.MagneticFieldStrength",
						resolver,
						delegate(Frame frame)
						{
							double value;
							bool tagExists = frame.ParentImageSop[DicomTags.MagneticFieldStrength].TryGetFloat64(0, out value);
							if (tagExists)
								return String.Format(SR.FormatTeslas, value);

							return "";
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.AcquisitionMatrix",
						resolver,
						delegate(Frame frame)
						{
							//TODO (CR Mar 2010): check for the 2 that aren't zero.
							string phaseDirection = frame.ParentImageSop[DicomTags.InPlanePhaseEncodingDirection].ToString().ToUpperInvariant();

							DicomAttribute acqAttrib = frame.ParentImageSop[DicomTags.AcquisitionMatrix];
							if (!acqAttrib.IsEmpty && acqAttrib.Count > 3)
							{
								ushort frequencyRows = acqAttrib.GetUInt16(0, 0);
								ushort frequencyColumns = acqAttrib.GetUInt16(1, 0);
								ushort phaseRows = acqAttrib.GetUInt16(2, 0);
								ushort phaseColumns = acqAttrib.GetUInt16(3, 0);

								switch (phaseDirection)
								{
									case "COL":
										return String.Format(SR.Format2Dimensions, phaseColumns, frequencyRows);
									case "ROW":
									default:
										return String.Format(SR.Format2Dimensions, frequencyColumns, phaseRows);
								}
							}

							return "";
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.ReceiveCoilName",
						resolver,
						delegate(Frame frame)
						{
							string value;
							value = frame.ParentImageSop[DicomTags.ReceiveCoilName].GetString(0, null);
							return value;
						},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.MRImage.RepetitionTime",
						resolver,
						delegate(Frame frame)
						{
							double value;
							bool tagExists = frame.ParentImageSop[DicomTags.RepetitionTime].TryGetFloat64(0, out value);
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
						"Dicom.MRImage.EchoTrainLength",
						resolver,
						delegate(Frame frame)
						{
							int value;
							bool tagExists = frame.ParentImageSop[DicomTags.EchoTrainLength].TryGetInt32(0, out value);
							if (tagExists)
								return String.Format("{0}", value);

							return "";
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