using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.ImagePlane
{
	//!! VM = 2

	internal class PixelSpacingAnnotationItem : DicomDoubleArrayAnnotationItem
	{
		public PixelSpacingAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("ImagePlane.PixelSpacing", ownerProvider)
		{
		}

		protected override double[] GetStoredDicomValues(DicomPresentationImage dicomPresentationImage)
		{
			double[] arrayDoubles = new double[2];

			arrayDoubles[0] = dicomPresentationImage.ImageSop.PixelSpacingX;
			arrayDoubles[1] = dicomPresentationImage.ImageSop.PixelSpacingY;

			return arrayDoubles;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.PixelSpacing; }
		}
	}
}
