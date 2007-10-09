using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Base class for comparers that compare some aspect of
	/// DICOM series.
	/// </summary>
	public abstract class DicomSeriesComparer : DisplaySetComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DicomSeriesComparer"/>.
		/// </summary>
		protected DicomSeriesComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomSeriesComparer"/>.
		/// </summary>
		protected DicomSeriesComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IDisplaySet> Members

		/// <summary>
		/// Compares two <see cref="IDisplaySet"/>s.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override int Compare(IDisplaySet x, IDisplaySet y)
		{
			if (x.PresentationImages.Count == 0 ||
				y.PresentationImages.Count == 0)
				return 0;

			IImageSopProvider provider1 = x.PresentationImages[0] as IImageSopProvider;
			IImageSopProvider provider2 = y.PresentationImages[0] as IImageSopProvider;

			if (provider1 == null)
			{
				if (provider2 == null)
					return 0; // x == y
				else
					return -1; // x > y (because we want it at the end for non-reverse sorting)
			}
			else
			{
				if (provider2 == null)
					return 1; // x < y (because we want it at the end for non-reverse sorting)
			}

			return Compare(provider1.ImageSop, provider2.ImageSop);
		}

		#endregion

		/// <summary>
		/// Compares two <see cref="ImageSop"/>s.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		/// <remarks>
		/// The relevant DICOM series property to be compared
		/// is taken from the <see cref="ImageSop"/>.
		/// </remarks>
		protected abstract int Compare(ImageSop x, ImageSop y);
	}
}
