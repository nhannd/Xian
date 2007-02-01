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
	public class PatientStudyAnnotationItemProvider : AnnotationItemProvider
	{
		public PatientStudyAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.PatientStudy")
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
							"Dicom.PatientStudy.AdditionalPatientsHistory",
							this,
							delegate(ImageSop imageSop) { return imageSop.AdditionalPatientsHistory; },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.PatientStudy.Occupation",
							this,
							new DicomTagAsStringRetriever(Dcm.Occupation).GetTagValue,
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.PatientStudy.PatientsAge",
							this,
							new DicomTagAsStringRetriever(Dcm.PatientsAge).GetTagValue,
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<double>
						(
							"Dicom.PatientStudy.PatientsSize",
							this,
							new DicomTagAsDoubleRetriever(Dcm.PatientsSize).GetTagValue,
							delegate(double input)
							{
								DoubleFormatter formatter = new DoubleFormatter();
								return String.Format("{0} {1}", formatter.Format(input), SR.Label_metres);
							}
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<double>
						(
							"Dicom.PatientStudy.PatientsWeight",
							this,
							new DicomTagAsDoubleRetriever(Dcm.PatientsWeight).GetTagValue,
							delegate(double input)
							{
								DoubleFormatter formatter = new DoubleFormatter();
								return String.Format("{0} {1}", formatter.Format(input), SR.Label_kilograms);
							}
						)
					);

				return annotationItems;
			}
		}
	}
}
