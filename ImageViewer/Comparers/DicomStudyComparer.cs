#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Diagnostics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Base class for comparers that compare some aspect of
	/// DICOM studies.
	/// </summary>
	public abstract class DicomStudyComparer : ImageSetComparer, IComparer<Study>, IComparer<Sop>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DicomStudyComparer"/>.
		/// </summary>
		protected DicomStudyComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomStudyComparer"/>.
		/// </summary>
		protected DicomStudyComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IImageSet> Members

		/// <summary>
		/// Compares two <see cref="IImageSet"/>s.
		/// </summary>
		public override int Compare(IImageSet x, IImageSet y)
		{
			if (x.DisplaySets.Count == 0 || y.DisplaySets.Count == 0)
			{
				Debug.Assert(false, "All image sets being sorted must have at least one display set with at least one image in order for them to be sorted properly.");
				return 0;
			}

			IDisplaySet displaySet1 = x.DisplaySets[0];
			IDisplaySet displaySet2 = y.DisplaySets[0];

			if (displaySet1.PresentationImages.Count == 0 || displaySet2.PresentationImages.Count == 0)
			{
				Debug.Assert(false, "All image sets being sorted must have at least one display set with at least one image in order for them to be sorted properly.");
				return 0;
			}

			ISopProvider provider1 = displaySet1.PresentationImages[0] as ISopProvider;
			ISopProvider provider2 = displaySet2.PresentationImages[0] as ISopProvider;

			if (provider1 == null)
			{
				if (provider2 == null)
					return 0; // x == y
				else
					return -this.ReturnValue; // x > y (because we want it at the end for non-reverse sorting)
			}
			else
			{
				if (provider2 == null)
					return this.ReturnValue; // x < y (because we want it at the end for non-reverse sorting)
			}

			return Compare(provider1.Sop, provider2.Sop);
		}

		#endregion

		/// <summary>
		/// Compares two <see cref="Study"/> instances.
		/// </summary>
		/// <remarks>Simply calls <see cref="Compare(ClearCanvas.ImageViewer.StudyManagement.Sop,ClearCanvas.ImageViewer.StudyManagement.Sop)"/>,
		/// passing the first <see cref="Sop"/> in each <see cref="Study"/>.</remarks>
		public int Compare(Study x, Study y)
		{
			if (x.Series.Count == 0 || y.Series.Count == 0)
				return 0;

			if (x.Series[0].Sops.Count == 0 || y.Series[0].Sops.Count == 0)
				return 0;

			return Compare(x.Series[0].Sops[0], y.Series[0].Sops[0]);
		}

		/// <summary>
		/// Compares two <see cref="ImageSop"/>s.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		/// <remarks>
		/// The relevant DICOM study property to be compared
		/// is taken from the <see cref="ImageSop"/>.
		/// </remarks>
		public abstract int Compare(Sop x, Sop y);
	}
}
