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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Compares two <see cref="Frame"/>s based on instance number and frame number.
	/// </summary>
	public class InstanceAndFrameNumberComparer : DicomFrameComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="InstanceAndFrameNumberComparer"/>.
		/// </summary>
		public InstanceAndFrameNumberComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="InstanceAndFrameNumberComparer"/>.
		/// </summary>
		public InstanceAndFrameNumberComparer(bool reverse)
			: base(reverse)
		{
		}

		private static IEnumerable<IComparable> GetCompareValues(Frame frame)
		{//Group be common study level attributes
            yield return frame.StudyInstanceUid;

            //Group by common series level attributes
            //This sorts "FOR PRESENTATION" images to the beginning (except in reverse, of course).
            yield return frame.ParentImageSop.PresentationIntentType == "FOR PRESENTATION" ? 0 : 1;
            yield return frame.ParentImageSop.SeriesNumber;
            yield return frame.ParentImageSop.SeriesDescription;
            yield return frame.SeriesInstanceUid;

			yield return frame.ParentImageSop.InstanceNumber;
			yield return frame.FrameNumber;
			//as a last resort.
			yield return frame.AcquisitionNumber;
		}

		/// <summary>
		/// Compares two <see cref="Frame"/>s based on instance number and frame number.
		/// </summary>
		public override int Compare(Frame x, Frame y)
		{
			return Compare(GetCompareValues(x), GetCompareValues(y));
		}
	}
}
