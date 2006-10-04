using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.GeneralEquipment
{
	internal class DeviceSerialNumberAnnotationItem : DicomStringAnnotationItem
	{
		public DeviceSerialNumberAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralEquipment.DeviceSerialNumber", ownerProvider)
		{
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.DeviceSerialNumber; }
		}
	}
}
