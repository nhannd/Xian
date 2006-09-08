using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.ImagePlane
{
	internal class SliceThicknessAnnotationItem : DicomDoubleAnnotationItem
	{
		public SliceThicknessAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("ImagePlane.SliceThickness", ownerProvider)
		{
		}

		protected override void GetStoredDicomValue(DicomPresentationImage dicomPresentationImage, out double storedDicomValue, out bool storedValueExists)
		{
			storedValueExists = false;
			storedDicomValue = 0.0;

			//!! Uncomment once this item has been implemented in ImageSop class(es).

			//storedValueExists = true;
			//storedDicomValue = dicomPresentationImage.ImageSop.SliceThickness;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.SliceThickness; }
		}
	}
}
