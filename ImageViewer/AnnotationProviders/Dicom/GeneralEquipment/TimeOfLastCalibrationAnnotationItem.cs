using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.GeneralEquipment
{
	internal class TimeOfLastCalibrationAnnotationItem : DicomTimeAnnotationItem
	{
		public TimeOfLastCalibrationAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralEquipment.TimeOfLastCalibration", ownerProvider)
		{
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.TimeOfLastCalibration; }
		}
	}
}
