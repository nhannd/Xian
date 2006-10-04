using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.GeneralSeries
{
	internal class SeriesDescriptionAnnotationItem : DicomStringAnnotationItem
	{
		public SeriesDescriptionAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralSeries.SeriesDescription", ownerProvider)
		{
		}

		protected override void GetStoredDicomValue(DicomPresentationImage dicomPresentationImage, out string dicomValue, out bool storedValueExists)
		{
			storedValueExists = true;
			dicomValue = dicomPresentationImage.ImageSop.SeriesDescription;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.SeriesDescription; }
		}
	}
}
