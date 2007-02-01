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
		public GeneralEquipmentAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralEquipment")
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
							"Dicom.GeneralEquipment.DateOfLastCalibration",
							this,
							new DicomTagAsStringRetriever(Dcm.DateOfLastCalibration).GetTagValue,
							DicomBasicResultFormatter.DateFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralEquipment.TimeOfLastCalibration",
							this,
							new DicomTagAsStringRetriever(Dcm.TimeOfLastCalibration).GetTagValue,
							DicomBasicResultFormatter.TimeFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralEquipment.DeviceSerialNumber",
							this,
							new DicomTagAsStringRetriever(Dcm.DeviceSerialNumber).GetTagValue,
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralEquipment.InstitutionAddress",
							this,
							new DicomTagAsStringRetriever(Dcm.InstitutionAddress).GetTagValue,
							DicomBasicResultFormatter.RawStringFormat
						)
					);


				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralEquipment.InstitutionalDepartmentName",
							this,
							delegate(ImageSop imageSop) { return imageSop.InstitutionalDepartmentName; },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralEquipment.InstitutionName",
							this,
							delegate(ImageSop imageSop) { return imageSop.InstitutionName; },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralEquipment.Manufacturer",
							this,
							delegate(ImageSop imageSop) { return imageSop.Manufacturer; },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralEquipment.ManufacturersModelName",
							this,
							delegate(ImageSop imageSop) { return imageSop.ManufacturersModelName; },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string>
						(
							"Dicom.GeneralEquipment.StationName",
							this,
							delegate(ImageSop imageSop) { return imageSop.StationName; },
							DicomBasicResultFormatter.RawStringFormat
						)
					);

				annotationItems.Add
					(
						new DicomAnnotationItem<string[]>
						(
							"Dicom.GeneralEquipment.SoftwareVersions",
							this,
							new DicomTagAsStringArrayRetriever(Dcm.SoftwareVersions).GetTagValue,
							DicomBasicResultFormatter.StringListFormat
						)
					);
				
				return annotationItems;
			}
		}
	}
}
