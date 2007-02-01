using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Reflection;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class PatientAnnotationItemProvider : AnnotationItemProvider
	{
		public PatientAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.Patient")
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
							"Dicom.Patient.EthnicGroup",
							this,
							new DicomTagAsStringRetriever(Dcm.EthnicGroup).GetTagValue,
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.Patient.PatientComments",
							this,
							new DicomTagAsStringRetriever(Dcm.PatientComments).GetTagValue,
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.Patient.PatientId",
							this,
							delegate(ImageSop imageSop){ return imageSop.PatientId; },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.Patient.PatientsBirthDate",
							this,
							delegate(ImageSop imageSop) { return imageSop.PatientsBirthDate; },
							DicomBasicResultFormatter.DateFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.Patient.PatientsBirthTime",
							this,
							new DicomTagAsStringRetriever(Dcm.PatientsBirthTime).GetTagValue,
							DicomBasicResultFormatter.TimeFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<PersonName>
						(
							"Dicom.Patient.PatientsName",
							this,
							delegate(ImageSop imageSop) { return imageSop.PatientsName; },
							DicomBasicResultFormatter.PersonNameFormatter
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.Patient.PatientsSex",
							this,
							delegate(ImageSop imageSop) { return imageSop.PatientsSex; },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				return annotationItems;
			}
		}
	}
}
