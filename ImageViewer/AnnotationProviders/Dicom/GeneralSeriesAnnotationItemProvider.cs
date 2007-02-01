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
		public GeneralSeriesAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralSeries")
		{
		}

		protected override List<IAnnotationItem> AnnotationItems
		{
			get
			{
				List<IAnnotationItem> annotationItems = new List<IAnnotationItem>();

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralSeries.BodyPartExamined",
							this,
							delegate(ImageSop imageSop) { return imageSop.BodyPartExamined; },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralSeries.Laterality",
							this,
							delegate(ImageSop imageSop) { return imageSop.Laterality; },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralSeries.Modality",
							this,
							delegate(ImageSop imageSop) { return imageSop.Modality; },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<PersonName[]>
						(
							"Dicom.GeneralSeries.OperatorsName",
							this,
							delegate(ImageSop imageSop) { return imageSop.OperatorsName; },
							DicomBasicResultFormatter.PersonNameListFormatter
						)
					);
				
				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralSeries.PerformedProcedureStepDescription",
							this,
							new DicomTagAsStringRetriever(Dcm.PerformedProcedureStepDescription).GetTagValue,
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<PersonName[]>
						(
							"Dicom.GeneralSeries.PerformingPhysiciansName",
							this,
							delegate(ImageSop imageSop) { return imageSop.PerformingPhysiciansName; },
							DicomBasicResultFormatter.PersonNameListFormatter
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralSeries.ProtocolName",
							this,
							new DicomTagAsStringRetriever(Dcm.ProtocolName).GetTagValue,
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralSeries.SeriesDate",
							this,
							delegate(ImageSop imageSop) { return imageSop.SeriesDate; },
							DicomBasicResultFormatter.DateFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralSeries.SeriesTime",
							this,
							delegate(ImageSop imageSop) { return imageSop.SeriesTime; },
							DicomBasicResultFormatter.TimeFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralSeries.SeriesDescription",
							this,
							delegate(ImageSop imageSop) { return imageSop.SeriesDescription; },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralSeries.SeriesNumber",
							this,
							delegate(ImageSop imageSop) { return imageSop.SeriesNumber.ToString(); },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				return annotationItems;
			}
		}
	}
}
