using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Reflection;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom.GeneralStudy
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class GeneralStudyAnnotationItemProvider : AnnotationItemProvider
	{
		public GeneralStudyAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.GeneralStudy")
		{
		}

		protected override List<IAnnotationItem> AnnotationItems
		{
			get
			{
				List<IAnnotationItem> annotationItems = new List<IAnnotationItem>();

				annotationItems.Add((IAnnotationItem)new AccessionNumberAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new ReferringPhysiciansNameAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new StudyDateAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new StudyDescriptionAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new StudyIdAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new StudyTimeAnnotationItem(this));

				return annotationItems;
			}
		}
	}
}
