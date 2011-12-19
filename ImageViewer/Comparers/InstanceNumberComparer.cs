#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Compares two <see cref="Sop"/>s based on Instance Number.
	/// </summary>
	public class InstanceNumberComparer : DicomSopComparer
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public InstanceNumberComparer()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public InstanceNumberComparer(bool reverse)
			: base(reverse)
		{
		}

		private static IEnumerable<IComparable> GetCompareValues(Sop sop)
		{
            //Group be common study level attributes
            yield return sop.StudyInstanceUid;

            //Group by common series level attributes
            //This sorts "FOR PRESENTATION" images to the beginning (except in reverse, of course).
            if (!sop.IsImage)
                yield return 1;
            else
                yield return ((ImageSop)sop).PresentationIntentType == "FOR PRESENTATION" ? 0 : 1;

            yield return sop.SeriesNumber;
            yield return sop.SeriesDescription;
            yield return sop.SeriesInstanceUid;

			yield return sop.InstanceNumber;
			yield return sop[DicomTags.AcquisitionNumber].GetInt32(0, 0);
		}

		/// <summary>
		/// Compares 2 <see cref="Sop"/>s based on Instance Number.
		/// </summary>
		public override int Compare(Sop x, Sop y)
		{
			return Compare(GetCompareValues(x), GetCompareValues(y));
		}
	}
}