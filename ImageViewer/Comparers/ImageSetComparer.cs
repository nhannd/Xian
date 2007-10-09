using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Base class for comparers that compare some aspect of
	/// <see cref="IImageSet"/>.
	/// </summary>
	public abstract class ImageSetComparer : ComparerBase, IComparer<IImageSet>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ImageSetComparer"/>.
		/// </summary>
		protected ImageSetComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageSetComparer"/>.
		/// </summary>
		protected ImageSetComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IImageSet> Members

		/// <summary>
		/// Compares two <see cref="IImageSet"/>s.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public abstract int Compare(IImageSet x, IImageSet y);

		#endregion
	}
}
