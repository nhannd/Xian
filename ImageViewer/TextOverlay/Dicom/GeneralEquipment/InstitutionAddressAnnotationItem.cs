using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralEquipment
{
	internal class InstitutionAddressAnnotationItem : DicomStringAnnotationItem
	{
		public InstitutionAddressAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralEquipment.InstitutionAddress", ownerProvider)
		{
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.InstitutionAddress; }
		}
	}
}
