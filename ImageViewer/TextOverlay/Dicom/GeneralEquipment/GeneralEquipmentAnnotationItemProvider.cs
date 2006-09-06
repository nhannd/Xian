using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Reflection;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralEquipment
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

				annotationItems.Add((IAnnotationItem)new DateOfLastCalibrationAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new DeviceSerialNumberAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new InstitutionAddressAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new InstitutionalDepartmentNameAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new InstitutionNameAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new ManufacturerAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new ManufacturersModelNameAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new SoftwareVersionsAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new SpatialResolutionAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new StationNameAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new TimeOfLastCalibrationAnnotationItem(this));

				return annotationItems;
			}
		}
	}
}
