using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralEquipment
{
	//!! VM = 1..N

	internal class SoftwareVersionsAnnotationItem : DicomStringArrayAnnotationItem
	{
		public SoftwareVersionsAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralEquipment.SoftwareVersions", ownerProvider)
		{
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.SoftwareVersions; }
		}
	}
}
