using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom;
using System.Globalization;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom
{
	internal abstract class DicomDateAnnotationItem : DicomStringAnnotationItem
	{
		public DicomDateAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: base(identifier, ownerProvider)
		{
		}

		protected override string GetFinalString(string dicomString)
		{
			DateTime dateTime = DateTime.ParseExact(dicomString, "YYYYMMDD", new CultureInfo(""));
			dicomString = dateTime.ToShortDateString();
			return dicomString;
		}
	}
}
