using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralEquipment
{
	internal class ManufacturersModelNameAnnotationItem : DicomStringAnnotationItem
	{
		public ManufacturersModelNameAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralEquipment.ManufacturersModelName", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.ManufacturersModelName;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.ManufacturersModelName; }
		}
	}
}
