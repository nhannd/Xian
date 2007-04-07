using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	public abstract class DisplaySetComparer : ComparerBase, IComparer<IDisplaySet>
	{
		public DisplaySetComparer()
		{
		}

		public DisplaySetComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IDisplaySet> Members

		public abstract int Compare(IDisplaySet x, IDisplaySet y);

		#endregion
	}
}
