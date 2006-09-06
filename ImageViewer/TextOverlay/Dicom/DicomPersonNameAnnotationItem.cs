using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom
{
	public abstract class DicomPersonNameAnnotationItem : DicomStringAnnotationItem
	{
		public DicomPersonNameAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: base(identifier, ownerProvider)
		{ 
		}

		protected override string GetFinalString(string dicomString)
		{
			PatientsName patientsName = new PatientsName(dicomString);
			return (patientsName.FirstName + " " + patientsName.LastName);
		}
	}
}
