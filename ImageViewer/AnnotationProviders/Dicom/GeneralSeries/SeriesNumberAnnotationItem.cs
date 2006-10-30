using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.GeneralSeries
{
	internal class SeriesNumberAnnotationItem : DicomStringAnnotationItem
	{
		public SeriesNumberAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralSeries.SeriesNumber", ownerProvider)
		{
		}

		protected override void GetStoredDicomValue(DicomPresentationImage dicomPresentationImage, out string dicomValue, out bool storedValueExists)
		{
			storedValueExists = true;
			dicomValue = dicomPresentationImage.ImageSop.SeriesNumber.ToString();
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.SeriesNumber; }
		}
	}
}
