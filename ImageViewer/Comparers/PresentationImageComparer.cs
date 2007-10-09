using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Base class for comparers that compare some aspect of
	/// <see cref="IPresentationImage"/>
	/// </summary>
	public abstract class PresentationImageComparer : ComparerBase, IComparer<IPresentationImage>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="PresentationImageComparer"/>.
		/// </summary>
		protected PresentationImageComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="PresentationImageComparer"/>.
		/// </summary>
		/// <param name="reverse"></param>
		protected PresentationImageComparer(bool reverse)
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
		public abstract int Compare(IPresentationImage x, IPresentationImage y);

		#endregion
	}
}
