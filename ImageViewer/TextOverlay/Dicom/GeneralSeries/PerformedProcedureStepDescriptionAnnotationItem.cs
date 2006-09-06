using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralSeries
{
	internal class PerformedProcedureStepDescriptionAnnotationItem : DicomStringAnnotationItem
	{
		public PerformedProcedureStepDescriptionAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralSeries.PerformedProcedureStepDescription", ownerProvider)
		{
		}
		
		protected override DcmTagKey DicomTag
		{
			get { return Dcm.PerformedProcedureStepDescription; }
		}
	}
}
