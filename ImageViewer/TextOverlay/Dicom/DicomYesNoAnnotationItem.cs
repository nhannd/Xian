using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom
{
	public abstract class DicomYesNoAnnotationItem : DicomStringAnnotationItem
	{
		public DicomYesNoAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: base(identifier, ownerProvider)
		{
		}

		protected override string GetFinalString(string dicomString)
		{
			if (Convert.ToByte(dicomString) == 0)
				return "No";
			else
				return "Yes";
		}
	}
}
