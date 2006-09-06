using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralEquipment
{
	internal class SpatialResolutionAnnotationItem : DicomStringAnnotationItem
	{
		public SpatialResolutionAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralEquipment.SpatialResolution", ownerProvider)
		{
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.SpatialResolution; }
		}
	}
}
