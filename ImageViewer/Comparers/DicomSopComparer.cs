using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Base class for comparing <see cref="Sop"/>s.
	/// </summary>
	public abstract class DicomSopComparer : PresentationImageComparer, IComparer<Sop>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DicomSopComparer"/>.
		/// </summary>
		protected DicomSopComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomSopComparer"/>.
		/// </summary>
		protected DicomSopComparer(bool reverse)
			: base(reverse)
		{
		}

		private static IEnumerable<IComparable> GetCompareValues(ISopProvider provider)
		{
			yield return provider.Sop.StudyInstanceUid;
			yield return provider.Sop.SeriesInstanceUid;
		}

		#region IComparer<IPresentationImage> Members

		/// <summary>
		/// Compares two <see cref="IPresentationImage"/>s.
		/// </summary>
		public override int Compare(IPresentationImage x, IPresentationImage y)
		{
			if (x == y)
				return 0; //same reference object

			ISopProvider xProvider = x as ISopProvider;
			ISopProvider yProvider = y as ISopProvider;

			if (ReferenceEquals(xProvider, yProvider))
				return 0; //same object or both are null

			//at this point, at least one of x or y is non-null and they are not the same object

			if (xProvider == null)
				return -ReturnValue; // x > y (because we want x at the end for non-reverse sorting)
			if (yProvider == null)
				return ReturnValue; // x < y (because we want y at the end for non-reverse sorting)

			int compare = Compare(GetCompareValues(xProvider), GetCompareValues(yProvider));
			if (compare == 0 && !ReferenceEquals(xProvider.Sop, yProvider.Sop))
				compare = Compare(xProvider.Sop, yProvider.Sop);

			return compare;
		}

		#endregion

		/// <summary>
		/// Compares two <see cref="Sop"/>s.
		/// </summary>
		public abstract int Compare(Sop x, Sop y);
	}
}
