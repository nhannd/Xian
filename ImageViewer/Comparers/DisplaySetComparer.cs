using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Base class for comparers that compare some aspect of
	/// <see cref="IDisplaySet"/>.
	/// </summary>
	public abstract class DisplaySetComparer : ComparerBase, IComparer<IDisplaySet>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DisplaySetComparer"/>.
		/// </summary>
		protected DisplaySetComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DisplaySetComparer"/>.
		/// </summary>
		protected DisplaySetComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IDisplaySet> Members

		/// <summary>
		/// Compares two <see cref="IDisplaySet"/>s.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public abstract int Compare(IDisplaySet x, IDisplaySet y);

		#endregion
	}
}
