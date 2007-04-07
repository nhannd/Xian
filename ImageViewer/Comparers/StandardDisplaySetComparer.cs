using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	public abstract class StandardDisplaySetComparer : DisplaySetComparer
	{
		public StandardDisplaySetComparer()
		{
		}

		public StandardDisplaySetComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IDisplaySet> Members

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

		protected abstract int Compare(ImageSop x, ImageSop y);
	}
}
