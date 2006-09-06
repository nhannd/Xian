using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralImage
{
	internal class QualityControlImageAnnotationItem : DicomYesNoAnnotationItem
	{
		public QualityControlImageAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralImage.QualityControlImage", ownerProvider)
		{
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.QualityControlImage; }
		}
	}
}
