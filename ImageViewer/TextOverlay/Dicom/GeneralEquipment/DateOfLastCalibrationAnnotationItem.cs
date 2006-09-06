using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralEquipment
{
	internal class DateOfLastCalibrationAnnotationItem : DicomDateAnnotationItem
	{
		public DateOfLastCalibrationAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralEquipment.DateOfLastCalibration", ownerProvider)
		{
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.DateOfLastCalibration; }
		}
	}
}
