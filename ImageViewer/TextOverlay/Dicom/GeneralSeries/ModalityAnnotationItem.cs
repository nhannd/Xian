using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralSeries
{
	internal class ModalityAnnotationItem : DicomStringAnnotationItem
	{
		public ModalityAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralSeries.Modality", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.Modality;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.Modality; }
		}
	}
}
