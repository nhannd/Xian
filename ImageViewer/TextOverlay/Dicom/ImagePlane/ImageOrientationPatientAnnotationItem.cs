using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.ImagePlane
{
	//!! VM = 6

	internal class ImageOrientationPatientAnnotationItem : DicomOrientationAnnotationItem
	{
		public ImageOrientationPatientAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("ImagePlane.ImageOrientationPatient", ownerProvider)
		{
		}

		protected override double[] GetStoredDicomValues(DicomPresentationImage dicomPresentationImage)
		{
			double[] arrayDoubles = new double[6];

			arrayDoubles[0] = dicomPresentationImage.ImageSop.ImageOrientationPatientRowX;
			arrayDoubles[1] = dicomPresentationImage.ImageSop.ImageOrientationPatientRowY;
			arrayDoubles[2] = dicomPresentationImage.ImageSop.ImageOrientationPatientRowZ;
			arrayDoubles[3] = dicomPresentationImage.ImageSop.ImageOrientationPatientColumnX;
			arrayDoubles[4] = dicomPresentationImage.ImageSop.ImageOrientationPatientColumnY;
			arrayDoubles[5] = dicomPresentationImage.ImageSop.ImageOrientationPatientColumnZ;

			return arrayDoubles;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.ImageOrientationPatient; }
		}
	}
}
