using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.OffisWrapper;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.TextOverlay.Dicom
{
	public abstract class DicomDoubleArrayAnnotationItem : AnnotationItem
	{
		public DicomDoubleArrayAnnotationItem(string identifier, IAnnotationItemProvider ownerProvider)
			: base("Dicom." + identifier, ownerProvider)
		{
		}

		protected abstract DcmTagKey DicomTag { get; }

		protected virtual double[] GetStoredDicomValues(DicomPresentationImage dicomPresentationImage)
		{
			throw new Exception("No appropriate stored property(s) exists.");
		}

		protected virtual string GetFinalString(double[] arrayDoubles)
		{
			//by default, return the values put back together 'dicom-encoded'.
			string dicomString = String.Empty;
			foreach (double value in arrayDoubles)
			{
				if (dicomString.Length > 0)
					dicomString += "\\";

				dicomString += value.ToString();
			}

			return dicomString;
		}

		public override string GetAnnotationText(PresentationImage presentationImage)
		{
			DicomPresentationImage dicomPresentationImage = (DicomPresentationImage)presentationImage;
			if (dicomPresentationImage != null)
			{
				try
				{
					double[] dicomDoubles = GetStoredDicomValues(dicomPresentationImage);
					return GetFinalString(dicomDoubles);
				}
				catch
				{
					try
					{
						//!! This needs to be rewritten once support is properly added in the DICOM module for VM > 1.
						throw new NotSupportedException();
						
						//bool tagExists = true;
						//List<double> dicomDoubles = new List<double>();
						//uint index = 0;

						//DcmTagKey dicomTag = this.DicomTag;

						//while (tagExists)
						//{
						//    double dicomDouble = 0.0;
						//    try
						//    {
						//        dicomPresentationImage.ImageSop.GetTag(dicomTag, out dicomDouble, index, out tagExists);
						//    }
						//    catch
						//    {
						//        tagExists = false;
						//    }

						//    if (tagExists)
						//        dicomDoubles.Add(dicomDouble);

						//    ++index;
						//}

						//return GetFinalString(dicomDoubles.ToArray());
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
