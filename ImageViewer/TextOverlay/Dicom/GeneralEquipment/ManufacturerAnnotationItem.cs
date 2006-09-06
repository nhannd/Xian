using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralEquipment
{
	internal class ManufacturerAnnotationItem : DicomStringAnnotationItem
	{
		public ManufacturerAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralEquipment.Manufacturer", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.Manufacturer;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.Manufacturer; }
		}
	}
}
