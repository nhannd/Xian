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
		private List<IAnnotationItem> _annotationItems;

		public PatientAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.Patient")
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
								"Dicom.Patient.EthnicGroup",
								this,
								new DicomTagAsStringRetriever(Dcm.EthnicGroup).GetTagValue,
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.Patient.PatientComments",
								this,
								new DicomTagAsStringRetriever(Dcm.PatientComments).GetTagValue,
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.Patient.PatientId",
								this,
								delegate(ImageSop imageSop) { return imageSop.PatientId; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.Patient.PatientsBirthDate",
								this,
								delegate(ImageSop imageSop) { return imageSop.PatientsBirthDate; },
								DicomBasicResultFormatter.DateFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.Patient.PatientsBirthTime",
								this,
								new DicomTagAsStringRetriever(Dcm.PatientsBirthTime).GetTagValue,
								DicomBasicResultFormatter.TimeFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<PersonName>
							(
								"Dicom.Patient.PatientsName",
								this,
								delegate(ImageSop imageSop) { return imageSop.PatientsName; },
								DicomBasicResultFormatter.PersonNameFormatter
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.Patient.PatientsSex",
								this,
								delegate(ImageSop imageSop) { return imageSop.PatientsSex; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);
				}

				return _annotationItems;
			}
		}
	}
}
