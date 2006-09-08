using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralImage
{
	internal class PatientOrientationAnnotationItem : DicomStringArrayAnnotationItem
	{
		//!! VM = 2

		public PatientOrientationAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralImage.PatientOrientation", ownerProvider)
		{
		}

		protected override string[] GetStoredDicomValues(DicomPresentationImage dicomPresentationImage)
		{
			return null;

			//!! Uncomment once this item has been implemented in ImageSop class(es).
			//string[] arrayStrings = new string[2];
			//arrayStrings[0] = dicomPresentationImage.ImageSop.PatientOrientationRows;
			//arrayStrings[1] = dicomPresentationImage.ImageSop.PatientOrientationColumns;
			//return arrayStrings;
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.PatientOrientation; }
		}
	}
}
