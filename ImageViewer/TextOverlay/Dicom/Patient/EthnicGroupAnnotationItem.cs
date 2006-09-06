using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.Patient
{
	internal class EthnicGroupAnnotationItem : DicomStringAnnotationItem
	{
		public EthnicGroupAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("Patient.EthnicGroup", ownerProvider)
		{
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.EthnicGroup; }
		}
	}
}
