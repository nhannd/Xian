using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Reflection;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class GeneralSeriesAnnotationItemProvider : AnnotationItemProvider
	{
		private List<IAnnotationItem> _annotationItems;

		public GeneralSeriesAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralSeries")
		{
		}

		protected override List<IAnnotationItem> AnnotationItems
		{
			get
			{
				if (_annotationItems == null)
				{
					_annotationItems = new List<IAnnotationItem>();

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralSeries.BodyPartExamined",
								this,
								delegate(ImageSop imageSop) { return imageSop.BodyPartExamined; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralSeries.Laterality",
								this,
								delegate(ImageSop imageSop) { return imageSop.Laterality; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralSeries.Modality",
								this,
								delegate(ImageSop imageSop) { return imageSop.Modality; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<PersonName[]>
							(
								"Dicom.GeneralSeries.OperatorsName",
								this,
								delegate(ImageSop imageSop) { return imageSop.OperatorsName; },
								DicomBasicResultFormatter.PersonNameListFormatter
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralSeries.PerformedProcedureStepDescription",
								this,
								new DicomTagAsStringRetriever(Dcm.PerformedProcedureStepDescription).GetTagValue,
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<PersonName[]>
							(
								"Dicom.GeneralSeries.PerformingPhysiciansName",
								this,
								delegate(ImageSop imageSop) { return imageSop.PerformingPhysiciansName; },
								DicomBasicResultFormatter.PersonNameListFormatter
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralSeries.ProtocolName",
								this,
								new DicomTagAsStringRetriever(Dcm.ProtocolName).GetTagValue,
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralSeries.SeriesDate",
								this,
								delegate(ImageSop imageSop) { return imageSop.SeriesDate; },
								DicomBasicResultFormatter.DateFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralSeries.SeriesTime",
								this,
								delegate(ImageSop imageSop) { return imageSop.SeriesTime; },
								DicomBasicResultFormatter.TimeFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralSeries.SeriesDescription",
								this,
								delegate(ImageSop imageSop) { return imageSop.SeriesDescription; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralSeries.SeriesNumber",
								this,
								delegate(ImageSop imageSop)
								{
									string str = String.Format("{0}/{1}",
										imageSop.SeriesNumber,
										imageSop.ParentSeries.ParentStudy.Series.Count);
									return str;
								},
								DicomBasicResultFormatter.RawStringFormat
							)
						);
				}

				return _annotationItems;
			}
		}
	}
}
