using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	public abstract class PresentationImageComparer : ComparerBase, IComparer<IPresentationImage>
	{
		public PresentationImageComparer()
		{
		}

		public PresentationImageComparer(bool reverse) : base(reverse)
		{
		}

		#region IComparer<IPresentationImage> Members

		public abstract int Compare(IPresentationImage x, IPresentationImage y);

		#endregion
	}
}
