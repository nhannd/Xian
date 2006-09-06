using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.ImagePlane
{
	internal class SliceLocationAnnotationItem : DicomDoubleAnnotationItem
	{
		public SliceLocationAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("ImagePlane.SliceLocation", ownerProvider)
		{
		}

		protected override double GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			return dicomPresentationImage.ImageSop.SliceLocation;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.SliceLocation; }
		}
	}
}
