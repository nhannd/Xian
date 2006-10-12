using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom;
using System.Globalization;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.Desktop.Configuration.User;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	internal abstract class DicomDateAnnotationItem : DicomStringAnnotationItem
	{
		public DicomDateAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: base(identifier, ownerProvider)
		{
		}

		protected override string GetFinalString(string dicomString)
		{
			if (String.IsNullOrEmpty(dicomString))
				return String.Empty;

			DateTime date;
			if (!DateParser.Parse(dicomString, out date))
				return dicomString;

			return date.ToString(DateFormatSettings.DateFormat);
		}
	}
}
