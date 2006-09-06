using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.ImagePlane
{
	//!! VM = 3

	internal class ImagePositionPatientAnnotationItem : DicomDoubleArrayAnnotationItem
	{
		public ImagePositionPatientAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("ImagePlane.ImagePositionPatient", ownerProvider)
		{
		}

		protected override double[] GetStoredDicomValues(DicomPresentationImage dicomPresentationImage)
		{
			double[] arrayDoubles = new double[3];
			
			arrayDoubles[0] = dicomPresentationImage.ImageSop.ImagePositionPatientX;
			arrayDoubles[1] = dicomPresentationImage.ImageSop.ImagePositionPatientY;
			arrayDoubles[2] = dicomPresentationImage.ImageSop.ImagePositionPatientZ;

			return arrayDoubles;
		}

		protected override string GetFinalString(double[] arrayDoubles)
		{
			//!! Need to decide what to do with this one.
			return base.GetFinalString(arrayDoubles);
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.ImagePositionPatient; }
		}
	}
}
