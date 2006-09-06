using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralImage
{
	internal class LossyImageCompressionAnnotationItem : DicomYesNoAnnotationItem
	{
		public LossyImageCompressionAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralImage.LossyImageCompression", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.LossyImageCompression;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.LossyImageCompression; }
		}
	}
}
