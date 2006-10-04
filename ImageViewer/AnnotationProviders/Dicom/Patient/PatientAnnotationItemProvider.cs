using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Reflection;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom.Patient
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class PatientAnnotationItemProvider : AnnotationItemProvider
	{
		public PatientAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.Patient")
		{
		}

		protected override List<IAnnotationItem> AnnotationItems
		{
			get
			{
				List<IAnnotationItem> annotationItems = new List<IAnnotationItem>();

				annotationItems.Add((IAnnotationItem)new EthnicGroupAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new PatientCommentsAnnotationItem(this)); 
				annotationItems.Add((IAnnotationItem)new PatientIdAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new PatientsBirthDateAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new PatientsBirthTimeAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new PatientsNameAnnotationItem(this));
				annotationItems.Add((IAnnotationItem)new PatientsSexAnnotationItem(this)); 

				return annotationItems;
			}
		}
	}
}
