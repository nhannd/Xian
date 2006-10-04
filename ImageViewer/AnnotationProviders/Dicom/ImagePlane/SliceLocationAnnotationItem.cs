using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.ImagePlane
{
	internal class SliceLocationAnnotationItem : DicomDoubleAnnotationItem
	{
		public SliceLocationAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("ImagePlane.SliceLocation", ownerProvider)
		{
		}

		protected override void GetStoredDicomValue(DicomPresentationImage dicomPresentationImage, out double storedDicomValue, out bool storedValueExists)
		{
			storedValueExists = false;
			storedDicomValue = 0.0;

			//!! Uncomment once this item has been implemented in ImageSop class(es).

			//storedValueExists = true;
			//storedDicomValue = dicomPresentationImage.ImageSop.SliceLocation;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.SliceLocation; }
		}
	}
}
