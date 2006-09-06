using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralImage
{
	//!! VM = 1..N

	internal class LossyImageCompressionRatioAnnotationItem : DicomStringArrayAnnotationItem
	{
		public LossyImageCompressionRatioAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralImage.LossyImageCompressionRatio", ownerProvider)
		{
		}

		protected override string[] GetStoredDicomValues(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.LossyImageCompressionRatio.Split('\\');
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.LossyImageCompressionRatio; }
		}
	}
}
