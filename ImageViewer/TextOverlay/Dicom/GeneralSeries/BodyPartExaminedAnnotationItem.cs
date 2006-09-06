using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralSeries
{
	internal class BodyPartExaminedAnnotationItem : DicomStringAnnotationItem
	{
		public BodyPartExaminedAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralSeries.BodyPartExamined", ownerProvider)
		{
		}

		protected override string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.BodyPartExamined;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.BodyPartExamined; }
		}
	}
}
