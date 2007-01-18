using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	public abstract class DicomDoubleAnnotationItem : AnnotationItem
	{
		public DicomDoubleAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: base("Dicom." + identifier, ownerProvider)
		{
		}

		protected abstract DcmTagKey DicomTag { get; }

		protected virtual void GetStoredDicomValue(DicomPresentationImage dicomPresentationImage, out double storedDicomValue, out bool storedValueExists)
		{
			storedValueExists = false;
			storedDicomValue = 0.0;
		}

		protected virtual string GetFinalString(double dicomDouble)
		{
			return dicomDouble.ToString();
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
					double dicomValue;
					bool dicomValueExists;
					GetStoredDicomValue(dicomPresentationImage, out dicomValue, out dicomValueExists);
					if (dicomValueExists)
						return GetFinalString(dicomValue);
				}
				catch (Exception e)
				{
					Platform.Log(e);
				}

				try
				{
					double dicomValue;
					bool tagExists;
					dicomPresentationImage.ImageSop.GetTag(this.DicomTag, out dicomValue, out tagExists);
					return GetFinalString(dicomValue);
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
