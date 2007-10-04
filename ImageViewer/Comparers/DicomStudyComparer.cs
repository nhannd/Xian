using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	public abstract class DicomStudyComparer : ImageSetComparer
	{
		protected DicomStudyComparer()
		{
		}

		protected DicomStudyComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IImageSet> Members

		public override int Compare(IImageSet x, IImageSet y)
		{
			if (x.DisplaySets.Count == 0 ||
				y.DisplaySets.Count == 0)
				return 0;

			IDisplaySet displaySet1 = x.DisplaySets[0];
			IDisplaySet displaySet2 = y.DisplaySets[0];

			if (displaySet1.PresentationImages.Count == 0 ||
				displaySet2.PresentationImages.Count == 0)
				return 0;

			IImageSopProvider provider1 = displaySet1.PresentationImages[0] as IImageSopProvider;
			IImageSopProvider provider2 = displaySet2.PresentationImages[0] as IImageSopProvider;

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

		protected abstract int Compare(ImageSop x, ImageSop y);
	}
}
