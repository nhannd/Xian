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
		private List<IAnnotationItem> _annotationItems;

		public GeneralStudyAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralStudy")
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
								"Dicom.GeneralStudy.AccessionNumber",
								this,
								delegate(ImageSop imageSop) { return imageSop.AccessionNumber; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<PersonName>
							(
								"Dicom.GeneralStudy.ReferringPhysiciansName",
								this,
								delegate(ImageSop imageSop) { return imageSop.ReferringPhysiciansName; },
								DicomBasicResultFormatter.PersonNameFormatter
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralStudy.StudyDate",
								this,
								delegate(ImageSop imageSop) { return imageSop.StudyDate; },
								DicomBasicResultFormatter.DateFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralStudy.StudyTime",
								this,
								delegate(ImageSop imageSop) { return imageSop.StudyTime; },
								DicomBasicResultFormatter.TimeFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralStudy.StudyDescription",
								this,
								delegate(ImageSop imageSop) { return imageSop.StudyDescription; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralStudy.StudyId",
								this,
								new DicomTagAsStringRetriever(Dcm.StudyID).GetTagValue,
								DicomBasicResultFormatter.RawStringFormat
							)
						);
				}

				return _annotationItems;
			}
		}
	}
}
