using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralSeries
{
	internal class SeriesDescriptionAnnotationItem : DicomStringAnnotationItem
	{
		public SeriesDescriptionAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralSeries.SeriesDescription", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.SeriesDescription;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.SeriesDescription; }
		}
	}
}
