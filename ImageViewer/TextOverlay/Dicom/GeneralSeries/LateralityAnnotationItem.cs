using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralSeries
{
	internal class LateralityNumberAnnotationItem : DicomStringAnnotationItem
	{
		public LateralityNumberAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralSeries.Laterality", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.Laterality;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.Laterality; }
		}
	}
}
