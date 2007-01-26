using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	internal abstract class DicomPersonNameAnnotationItem : DicomStringAnnotationItem
	{
		public DicomPersonNameAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: base(identifier, ownerProvider)
		{ 
		}

		protected override string GetFinalString(string dicomString)
		{
			if (String.IsNullOrEmpty(dicomString))
				return String.Empty;

			PersonName patientsName = new PersonName(dicomString);
			return (patientsName.FirstName + " " + patientsName.LastName);
		}
	}
}
