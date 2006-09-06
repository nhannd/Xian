using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom
{
	public abstract class DicomStringAnnotationItem : AnnotationItem
	{
		public DicomStringAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: base("Dicom." + identifier, ownerProvider)
		{
		}

		protected abstract DcmTagKey DicomTag { get; }
		
		protected virtual string GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			throw new Exception("No appropriate stored property exists.");
		}

		protected virtual string GetFinalString(string dicomString)
		{
			return dicomString;
		}

		public override string GetAnnotationText(PresentationImage presentationImage)
		{
			DicomPresentationImage dicomPresentationImage = (DicomPresentationImage)presentationImage;
			if (dicomPresentationImage != null)
			{
				try
				{
					string dicomString = GetStoredDicomValue(dicomPresentationImage);
					return this.GetFinalString(dicomString);
				}
				catch
				{
					try
					{
						bool tagExists = false;
						string dicomString = String.Empty;
						dicomPresentationImage.ImageSop.GetTag(this.DicomTag, out dicomString, out tagExists);
						return this.GetFinalString(dicomString);
					}
					catch
					{
					}
				}
			}

			return String.Empty;
		}

	}
}
