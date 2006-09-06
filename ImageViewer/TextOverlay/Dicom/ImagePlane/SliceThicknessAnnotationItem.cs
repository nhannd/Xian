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

		protected override double GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.SliceThickness;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.SliceThickness; }
		}
	}
}
