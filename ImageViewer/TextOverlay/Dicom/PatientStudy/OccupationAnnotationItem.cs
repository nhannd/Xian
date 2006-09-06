using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.PatientStudy
{
	internal class OccupationAnnotationItem : DicomStringAnnotationItem
	{
		public OccupationAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("PatientStudy.Occupation", ownerProvider)
		{
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.Occupation; }
		}
	}
}
