using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralSeries
{
	internal class ProtocolNameAnnotationItem : DicomStringAnnotationItem
	{
		public ProtocolNameAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralSeries.ProtocolName", ownerProvider)
		{
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.ProtocolName; }
		}
	}
}
