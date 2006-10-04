using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Reflection;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.PatientStudy
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class PatientStudyAnnotationItemProvider : AnnotationItemProvider
	{
		public PatientStudyAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.PatientStudy")
		{
		}

		protected override List<IAnnotationItem> AnnotationItems
		{
			get
			{
				List<IAnnotationItem> annotationItems = new List<IAnnotationItem>();

				annotationItems.Add((IAnnotationItem)new AdditionalPatientsHistoryAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new OccupationAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new PatientsAgeAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new PatientsSizeAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new PatientsWeightAnnotationItem(this));

				return annotationItems;
			}
		}
	}
}
