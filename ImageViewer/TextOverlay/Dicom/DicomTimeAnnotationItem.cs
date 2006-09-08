using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom;
using System.Globalization;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom
{
	public abstract class DicomTimeAnnotationItem : DicomStringAnnotationItem
	{
		public DicomTimeAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: base(identifier, ownerProvider)
		{
		}

		protected override string GetFinalString(string dicomString)
		{
			DateTime dateTime = DateTime.ParseExact(dicomString, "HHmmss.FFFFFF", new CultureInfo(""));
			return dateTime.ToShortTimeString();
		}
	}
}
