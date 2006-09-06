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

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.StationName;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.StationName; }
		}
	}
}
