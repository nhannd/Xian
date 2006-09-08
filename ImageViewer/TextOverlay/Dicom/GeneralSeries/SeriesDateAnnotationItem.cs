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

		protected override void GetStoredDicomValue(DicomPresentationImage dicomPresentationImage, out string dicomValue, out bool storedValueExists)
		{
			dicomValue = string.Empty;
			storedValueExists = false;

			//!! Uncomment once this item has been implemented in ImageSop class(es).
			//storedValueExists = true; 
			//dicomValue = dicomPresentationImage.ImageSop.SeriesDate;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.SeriesDate; }
		}
	}
}
