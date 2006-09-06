using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralImage
{
	internal class DerivationDescriptionAnnotationItem : DicomStringAnnotationItem
	{
		public DerivationDescriptionAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralImage.DerivationDescription", ownerProvider)
		{
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.DerivationDescription; }
		}
	}
}
