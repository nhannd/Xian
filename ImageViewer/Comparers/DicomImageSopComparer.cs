using System;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	public abstract class DicomImageSopComparer : PresentationImageComparer
	{
		protected DicomImageSopComparer()
		{
		}

		protected DicomImageSopComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IPresentationImage> Members

		public override int Compare(IPresentationImage x, IPresentationImage y)
		{
			IImageSopProvider image1 = x as IImageSopProvider;
			IImageSopProvider image2 = y as IImageSopProvider;

			//just push non-DICOM images down to one end of the list.
			if (image1 == null)
			{
				if (image2 == null)
					return 0; // x == y
				else
					return (-this.ReturnValue); // x > y (because we want it at the end for non-reverse sorting)
			}
			else
			{
				if (image2 == null)
					return this.ReturnValue; // x < y (because we want it at the end for non-reverse sorting)
			}

			int studyUIDCompare = String.Compare(image1.ImageSop.StudyInstanceUID, image2.ImageSop.StudyInstanceUID);
			
			if (studyUIDCompare < 0)
			{
				return this.ReturnValue; // x < y
			}
			else if (studyUIDCompare == 0)
			{
				int seriesUIDCompare = String.Compare(image1.ImageSop.SeriesInstanceUID, image2.ImageSop.SeriesInstanceUID);

				if (seriesUIDCompare < 0)
				{
					return this.ReturnValue; // x < y
				}
				else if (seriesUIDCompare == 0)
				{
					return Compare(image1.ImageSop, image2.ImageSop);
				}
			}

			return (-this.ReturnValue); // x > y
		}

		#endregion

		protected abstract int Compare(ImageSop x, ImageSop y);
	}
}
