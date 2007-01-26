using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.Dicom;
using System.Globalization;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	internal abstract class DicomTimeAnnotationItem : DicomStringAnnotationItem
	{
		public DicomTimeAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: base(identifier, ownerProvider)
		{
		}

		protected override string GetFinalString(string dicomString)
		{
			if (String.IsNullOrEmpty(dicomString))
				return String.Empty;

			DateTime time;
			if (!TimeParser.Parse(dicomString, out time))
				return dicomString;

			return time.ToLongTimeString();
		}
	}
}
