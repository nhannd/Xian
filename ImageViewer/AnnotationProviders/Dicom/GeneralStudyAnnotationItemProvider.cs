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
	public class GeneralStudyAnnotationItemProvider : AnnotationItemProvider
	{
		public GeneralStudyAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralStudy")
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
							"Dicom.GeneralStudy.AccessionNumber",
							this,
							delegate(ImageSop imageSop) { return imageSop.AccessionNumber; },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<PersonName>
						(
							"Dicom.GeneralStudy.ReferringPhysiciansName",
							this,
							delegate(ImageSop imageSop) { return imageSop.ReferringPhysiciansName; },
							DicomBasicResultFormatter.PersonNameFormatter
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralStudy.StudyDate",
							this,
							delegate(ImageSop imageSop) { return imageSop.StudyDate; },
							DicomBasicResultFormatter.DateFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralStudy.StudyTime",
							this,
							delegate(ImageSop imageSop) { return imageSop.StudyTime; },
							DicomBasicResultFormatter.TimeFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralStudy.StudyDescription",
							this,
							delegate(ImageSop imageSop) { return imageSop.StudyDescription; },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralStudy.StudyId",
							this,
							new DicomTagAsStringRetriever(Dcm.StudyID).GetTagValue,
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				return annotationItems;
			}
		}
	}
}
