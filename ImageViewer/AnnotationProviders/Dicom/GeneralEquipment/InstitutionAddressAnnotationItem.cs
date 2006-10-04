using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.GeneralEquipment
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
