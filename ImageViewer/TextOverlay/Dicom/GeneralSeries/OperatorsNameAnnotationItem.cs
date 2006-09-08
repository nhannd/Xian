using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralSeries
{
	//!! VM = 1..N

	internal class OperatorsNameAnnotationItem : DicomStringArrayAnnotationItem
	{
		public OperatorsNameAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralSeries.OperatorsName", ownerProvider)
		{
		}

		protected override string[] GetStoredDicomValues(DicomPresentationImage dicomPresentationImage)
		{
			return null;

			//!! Uncomment once this item has been implemented in ImageSop class(es).
			//return dicomPresentationImage.ImageSop.OperatorsName.Split('\\');
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.OperatorsName; }
		}
	}
}
