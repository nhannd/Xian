using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Comparers
{
	public abstract class ImageSetComparer : ComparerBase, IComparer<IImageSet>
	{
		protected ImageSetComparer()
		{
		}

		protected ImageSetComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IImageSet> Members

		public abstract int Compare(IImageSet x, IImageSet y);

		#endregion
	}
}
