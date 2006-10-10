using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	public abstract class DicomStringAnnotationItem : AnnotationItem
	{
		public DicomStringAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: base("Dicom." + identifier, ownerProvider)
		{
		}

		protected abstract DcmTagKey DicomTag { get; }
		
		protected virtual void GetStoredDicomValue(DicomPresentationImage dicomPresentationImage, out string dicomValue, out bool storedValueExists)
		{
			storedValueExists = false;
			dicomValue = String.Empty;
		}

		protected virtual string GetFinalString(string dicomString)
		{
			return dicomString;
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage == null)
				return string.Empty;

			DicomPresentationImage dicomPresentationImage = (DicomPresentationImage)presentationImage;
			if (dicomPresentationImage != null)
			{
				try
				{
					string dicomString = String.Empty;
					bool storedValueExists = true;
					GetStoredDicomValue(dicomPresentationImage, out dicomString, out storedValueExists);
					if (storedValueExists)
						return this.GetFinalString(dicomString);
				}
				catch (Exception e)
				{
					Platform.Log(e);
				}

				try
				{
					string dicomString = String.Empty;
					bool tagExists = true;
					dicomPresentationImage.ImageSop.GetTag(this.DicomTag, out dicomString, out tagExists);
					if (tagExists)
						return this.GetFinalString(dicomString);
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
