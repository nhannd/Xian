using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Comparers
{
	public abstract class PresentationImageComparer : ComparerBase, IComparer<IPresentationImage>
	{
		protected PresentationImageComparer()
		{
		}

		protected PresentationImageComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IPresentationImage> Members

		public abstract int Compare(IPresentationImage x, IPresentationImage y);

		#endregion
	}
}
