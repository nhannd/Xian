#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Compares two <see cref="ImageSop"/>s based on series number.
	/// </summary>
	public class SeriesNumberComparer : DicomSeriesComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SeriesNumberComparer"/>.
		/// </summary>
		public SeriesNumberComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="SeriesNumberComparer"/>.
		/// </summary>
		public SeriesNumberComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IDisplaySet> Members

		/// <summary>
		/// Compares two <see cref="ImageSop"/>s based on series number.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override int Compare(Sop x, Sop y)
		{
			int seriesNumber1 = x.SeriesNumber;
			int seriesNumber2 = y.SeriesNumber;

			if (seriesNumber1 < seriesNumber2)
				return this.ReturnValue;
			else if (seriesNumber1 > seriesNumber2)
				return -this.ReturnValue;
			else
				return 0;
		}

		#endregion
	}
}
