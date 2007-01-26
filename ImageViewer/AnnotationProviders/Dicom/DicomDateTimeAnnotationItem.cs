using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom;
using System.Globalization;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	internal abstract class DicomDateTimeAnnotationItem : DicomStringAnnotationItem
	{
		public DicomDateTimeAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: base(identifier, ownerProvider)
		{
		}

		protected override string GetFinalString(string dicomString)
		{
			if (String.IsNullOrEmpty(dicomString))
				return String.Empty;

			DateTime dateTime;
			if (DateTimeParser.Parse(dicomString, out dateTime))
				return dicomString;

			return dateTime.ToString();
		}
	}
}
