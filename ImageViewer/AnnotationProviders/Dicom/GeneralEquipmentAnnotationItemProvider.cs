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
	public class GeneralEquipmentAnnotationItemProvider : AnnotationItemProvider
	{
		private List<IAnnotationItem> _annotationItems;

		public GeneralEquipmentAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralEquipment")
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
								"Dicom.GeneralEquipment.DateOfLastCalibration",
								this,
								new DicomTagAsStringRetriever(DicomTags.DateofLastCalibration).GetTagValue,
								DicomBasicResultFormatter.DateFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralEquipment.TimeOfLastCalibration",
								this,
								new DicomTagAsStringRetriever(DicomTags.TimeofLastCalibration).GetTagValue,
								DicomBasicResultFormatter.TimeFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralEquipment.DeviceSerialNumber",
								this,
								new DicomTagAsStringRetriever(DicomTags.DeviceSerialNumber).GetTagValue,
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralEquipment.InstitutionAddress",
								this,
								new DicomTagAsStringRetriever(DicomTags.InstitutionAddress).GetTagValue,
								DicomBasicResultFormatter.RawStringFormat
							)
						);


					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralEquipment.InstitutionalDepartmentName",
								this,
								delegate(ImageSop imageSop) { return imageSop.InstitutionalDepartmentName; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralEquipment.InstitutionName",
								this,
								delegate(ImageSop imageSop) { return imageSop.InstitutionName; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralEquipment.Manufacturer",
								this,
								delegate(ImageSop imageSop) { return imageSop.Manufacturer; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralEquipment.ManufacturersModelName",
								this,
								delegate(ImageSop imageSop) { return imageSop.ManufacturersModelName; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string>
							(
								"Dicom.GeneralEquipment.StationName",
								this,
								delegate(ImageSop imageSop) { return imageSop.StationName; },
								DicomBasicResultFormatter.RawStringFormat
							)
						);

					_annotationItems.Add
						(
							new DicomAnnotationItem<string[]>
							(
								"Dicom.GeneralEquipment.SoftwareVersions",
								this,
								new DicomTagAsStringArrayRetriever(DicomTags.SoftwareVersions).GetTagValue,
								DicomBasicResultFormatter.StringListFormat
							)
						);
				}
				
				return _annotationItems;
			}
		}
	}
}
