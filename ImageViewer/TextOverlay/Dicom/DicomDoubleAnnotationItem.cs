using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom
{
	public abstract class DicomDoubleAnnotationItem : AnnotationItem
	{
		public DicomDoubleAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: base("Dicom." + identifier, ownerProvider)
		{
		}

		protected abstract DcmTagKey DicomTag { get; }

		protected virtual double GetStoredDicomValue(DicomPresentationImage dicomPresentationImage)
		{
			throw new Exception("No appropriate stored property exists.");
		}

		protected virtual string GetFinalString(double dicomDouble)
		{
			return dicomDouble.ToString();
		}

		public override string GetAnnotationText(PresentationImage presentationImage)
		{
			DicomPresentationImage dicomPresentationImage = (DicomPresentationImage)presentationImage;
			if (dicomPresentationImage != null)
			{
				try
				{
					double value = GetStoredDicomValue(dicomPresentationImage);
					return GetFinalString(value);
				}
				catch
				{
					try
					{
						bool tagExists = false;
						double dicomValue = 0.0;
						dicomPresentationImage.ImageSop.GetTag(this.DicomTag, out dicomValue, out tagExists);
						return GetFinalString(dicomValue);
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
