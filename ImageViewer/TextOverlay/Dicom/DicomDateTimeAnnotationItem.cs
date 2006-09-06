using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom;
using System.Globalization;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom
{
	public abstract class DicomDateTimeAnnotationItem : DicomStringAnnotationItem
	{
		public DicomDateTimeAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: base(identifier, ownerProvider)
		{
		}

		protected override string GetFinalString(string dicomString)
		{
			DateTime dateTime = DateTime.ParseExact(dicomString, "YYYYMMDDHHmmss.ffffff", new CultureInfo(""));
			dicomString = dateTime.ToString();
			return dicomString;
		}
	}
}
