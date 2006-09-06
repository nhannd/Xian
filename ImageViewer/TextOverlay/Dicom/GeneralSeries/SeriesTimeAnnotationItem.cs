using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralSeries
{
	internal class SeriesTimeAnnotationItem : DicomTimeAnnotationItem
	{
		public SeriesTimeAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralSeries.SeriesTime", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.SeriesTime;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.SeriesTime; }
		}
	}
}
