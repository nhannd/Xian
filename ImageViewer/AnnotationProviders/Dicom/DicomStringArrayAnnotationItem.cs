using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	internal abstract class DicomStringArrayAnnotationItem : AnnotationItem
	{
		public DicomStringArrayAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: base("Dicom." + identifier, ownerProvider)
		{
		}

		protected abstract DcmTagKey DicomTag { get; }

		protected virtual string[] GetStoredDicomValues(DicomPresentationImage dicomPresentationImage)
		{
			return null;
		}

		protected virtual string GetFinalString(string[] dicomStrings)
		{
			//by default, return the values put back together 'dicom-encoded'.
			string dicomString = String.Empty;

			foreach (string value in dicomStrings)
			{
				if (dicomString.Length > 0)
					dicomString += "\\";

				dicomString += value;
			}

			return dicomString;
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage == null)
				return string.Empty;

			DicomPresentationImage dicomPresentationImage = presentationImage as DicomPresentationImage;
			if (dicomPresentationImage != null)
			{
				try
				{
					string[] dicomStrings = GetStoredDicomValues(dicomPresentationImage);
					if (dicomStrings != null)
						return GetFinalString(dicomStrings);
				}
				catch (Exception e)
				{
					Platform.Log(e);
				}

				try
				{
					//!! This needs to be rewritten once support is properly added in the DICOM module for VM > 1.

					List<string> dicomStrings = new List<string>();
					bool tagExists = true;
					string dicomString = String.Empty;
					dicomPresentationImage.ImageSop.GetTag(this.DicomTag, out dicomString, out tagExists);
					if (tagExists)
						dicomStrings.Add(dicomString);
					
					if (dicomStrings.Count > 0)
						return GetFinalString(dicomStrings.ToArray());
					

					//bool tagExists = true;
					//List<string> dicomStrings = new List<string>();
					//uint index = 0;

					//DcmTagKey dicomTag = this.DicomTag;

					//while (tagExists)
					//{
					//    string dicomString = String.Empty;
					//    try
					//    {
					//        dicomPresentationImage.ImageSop.GetTag(dicomTag, out dicomString, index, out tagExists);
					//    }
					//    catch
					//    {
					//        tagExists = false;
					//    }

					//    if (tagExists)
					//        dicomStrings.Add(dicomString);

					//    ++index;
					//}

					//return GetFinalString(dicomStrings.ToArray());
				}
				catch (Exception e)
				{
					Platform.Log(e);
				}
			}

			return String.Empty;
		}

	}
}
