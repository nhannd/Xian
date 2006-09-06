using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralEquipment
{
	internal class InstitutionNameAnnotationItem : DicomStringAnnotationItem
	{
		public InstitutionNameAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralEquipment.InstitutionName", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.InstitutionName;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.InstitutionName; }
		}
	}
}
