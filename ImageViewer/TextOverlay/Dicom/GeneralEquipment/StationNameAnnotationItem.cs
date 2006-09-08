using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralEquipment
{
	internal class StationNameAnnotationItem : DicomStringAnnotationItem
	{
		public StationNameAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralEquipment.StationName", ownerProvider)
		{
		}

		protected override void GetStoredDicomValue(DicomPresentationImage dicomPresentationImage, out string dicomValue, out bool storedValueExists)
		{
			dicomValue = string.Empty;
			storedValueExists = false;

			//!! Uncomment once this item has been implemented in ImageSop class(es).
			//storedValueExists = true; 
			//dicomValue = dicomPresentationImage.ImageSop.StationName;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.StationName; }
		}
	}
}
