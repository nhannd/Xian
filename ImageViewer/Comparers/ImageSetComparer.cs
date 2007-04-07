using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	public abstract class ImageSetComparer : ComparerBase, IComparer<IImageSet>
	{
		public ImageSetComparer()
		{
		}

		public ImageSetComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IImageSet> Members

		public abstract int Compare(IImageSet x, IImageSet y);

		#endregion
	}
}
