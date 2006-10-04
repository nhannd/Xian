using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.GeneralStudy
{
	internal class StudyIdAnnotationItem : DicomStringAnnotationItem
	{
		public StudyIdAnnotationItem(IAnnotationItemProvider ownerProvider)
			: base("GeneralStudy.StudyId", ownerProvider)
		{
		}

		protected override DcmTagKey DicomTag
		{
			get { return Dcm.StudyID; }
		}
	}
}
