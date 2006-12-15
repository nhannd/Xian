using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	public abstract class DicomYesNoAnnotationItem : DicomStringAnnotationItem
	{
		public DicomYesNoAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: base(identifier, ownerProvider)
		{
		}

		protected override string GetFinalString(string dicomString)
		{
			if (String.IsNullOrEmpty(dicomString))
				return String.Empty;

#if MONO
			if (Convert.ToByte(dicomString) == 0)
				return SR.BoolNo;
			else
				return SR.BoolYes;
#else
			byte value;
			if (!byte.TryParse(dicomString, out value))
				return dicomString;
			
			if (value == 0)
				return SR.BoolNo;
			else
				return SR.BoolYes;
#endif
		}
	}
}
