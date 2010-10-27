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
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class GeneralSeriesAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public GeneralSeriesAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralSeries", new AnnotationResourceResolver(typeof(GeneralSeriesAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();

			AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralSeries.BodyPartExamined",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.BodyPartExamined; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralSeries.Laterality",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.Laterality; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralSeries.Modality",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.Modality; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<PersonName[]>
					(
						"Dicom.GeneralSeries.OperatorsName",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.OperatorsName; },
						DicomDataFormatHelper.PersonNameListFormatter
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralSeries.PerformedProcedureStepDescription",
						resolver,
						FrameDataRetrieverFactory.GetStringRetriever(DicomTags.PerformedProcedureStepDescription),
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<PersonName[]>
					(
						"Dicom.GeneralSeries.PerformingPhysiciansName",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.PerformingPhysiciansName; },
						DicomDataFormatHelper.PersonNameListFormatter
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralSeries.ProtocolName",
						resolver,
						FrameDataRetrieverFactory.GetStringRetriever(DicomTags.ProtocolName),
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralSeries.SeriesDate",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.SeriesDate; },
						DicomDataFormatHelper.DateFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralSeries.SeriesTime",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.SeriesTime; },
						DicomDataFormatHelper.TimeFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralSeries.SeriesDescription",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.SeriesDescription; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.GeneralSeries.SeriesNumber",
						resolver,
						delegate(Frame frame)
						{
							if (frame.ParentImageSop.ParentSeries != null)
							{
								return String.Format("{0}/{1}", frame.ParentImageSop.SeriesNumber, 
									frame.ParentImageSop.ParentSeries.ParentStudy.Series.Count);
							}
							else
							{
								return frame.ParentImageSop.SeriesNumber.ToString();
							}
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
