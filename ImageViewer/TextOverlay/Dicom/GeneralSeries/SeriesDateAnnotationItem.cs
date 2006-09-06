using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralSeries
{
	internal class SeriesDateAnnotationItem : DicomDateAnnotationItem
	{
		public SeriesDateAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralSeries.SeriesDate", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.SeriesDate;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.SeriesDate; }
		}
	}
}
