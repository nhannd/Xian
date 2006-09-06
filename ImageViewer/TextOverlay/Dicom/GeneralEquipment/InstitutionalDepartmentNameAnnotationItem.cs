using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralEquipment
{
	internal class InstitutionalDepartmentNameAnnotationItem : DicomStringAnnotationItem
	{
		public InstitutionalDepartmentNameAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralEquipment.InstitutionalDepartmentName", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.InstitutionalDepartmentName;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.InstitutionalDepartmentName; }
		}
	}
}
