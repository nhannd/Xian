using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralImage
{
	internal class InstanceNumberAnnotationItem : DicomStringAnnotationItem
	{
		public InstanceNumberAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralImage.InstanceNumber", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.InstanceNumber;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.InstanceNumber; }
		}
	}
}
