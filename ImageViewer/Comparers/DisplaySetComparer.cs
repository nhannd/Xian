using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Comparers
{
	public abstract class DisplaySetComparer : ComparerBase, IComparer<IDisplaySet>
	{
		protected DisplaySetComparer()
		{
		}

		protected DisplaySetComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IDisplaySet> Members

		public abstract int Compare(IDisplaySet x, IDisplaySet y);

		#endregion
	}
}
