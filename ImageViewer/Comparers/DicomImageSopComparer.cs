using System;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Base class for comparers that compare some aspect of
	/// <see cref="ImageSop"/>.
	/// </summary>
	public abstract class DicomImageSopComparer : PresentationImageComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DicomImageSopComparer"/>.
		/// </summary>
		protected DicomImageSopComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomImageSopComparer"/>.
		/// </summary>
		protected DicomImageSopComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IPresentationImage> Members

		/// <summary>
		/// Compares two <see cref="IPresentationImage"/>s.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Compares two <see cref="ImageSop"/>s.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected abstract int Compare(ImageSop x, ImageSop y);
	}
}
