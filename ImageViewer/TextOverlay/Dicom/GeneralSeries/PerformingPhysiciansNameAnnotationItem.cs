using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralSeries
{
	//!! VM = 1..N

	internal class PerformingPhysiciansNameAnnotationItem : DicomStringArrayAnnotationItem
	{
		public PerformingPhysiciansNameAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralSeries.PerformingPhysiciansName", ownerProvider)
		{
		}

		protected override string[] GetStoredDicomValues(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.PerformingPhysiciansName.Split('\\');
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.PerformingPhysiciansName; }
		}
	}
}
