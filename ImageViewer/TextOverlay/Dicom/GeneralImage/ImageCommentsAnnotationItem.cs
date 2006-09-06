using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralImage
{
	internal class ImageCommentsAnnotationItem : DicomStringAnnotationItem
	{
		public ImageCommentsAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralImage.ImageComments", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.ImageComments;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.ImageComments; }
		}
	}
}
