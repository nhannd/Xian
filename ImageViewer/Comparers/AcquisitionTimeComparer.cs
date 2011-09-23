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
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Compares two <see cref="Frame"/>s based on acquisition date and time.
	/// </summary>
	public class AcquisitionTimeComparer : DicomFrameComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="AcquisitionTimeComparer"/>.
		/// </summary>
		public AcquisitionTimeComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="AcquisitionTimeComparer"/>.
		/// </summary>
		public AcquisitionTimeComparer(bool reverse)
			: base(reverse)
		{
		}

		private static IEnumerable<IComparable> GetCompareValues(Frame frame)
		{
			//Group be common study level attributes
            yield return frame.StudyInstanceUid;

            //Group by common series level attributes
            //This sorts "FOR PRESENTATION" images to the beginning (except in reverse, of course).
            yield return frame.ParentImageSop.PresentationIntentType == "FOR PRESENTATION" ? 0 : 1;
            yield return frame.ParentImageSop.SeriesNumber;
            yield return frame.ParentImageSop.SeriesDescription;
            yield return frame.SeriesInstanceUid;

			DateTime? datePart = null;
			TimeSpan? timePart = null;

			//then sort by acquisition datetime.
			DateTime? acquisitionDateTime = DateTimeParser.Parse(frame.AcquisitionDateTime);
			if (acquisitionDateTime != null)
			{
				datePart = acquisitionDateTime.Value.Date;
				timePart = acquisitionDateTime.Value.TimeOfDay;
			}
			else 
			{
				datePart = DateParser.Parse(frame.AcquisitionDate);
				if (datePart != null)
				{
					//only set the time part if there is a valid date part.
					DateTime? acquisitionTime = TimeParser.Parse(frame.AcquisitionTime);
					if (acquisitionTime != null)
						timePart = acquisitionTime.Value.TimeOfDay;
				}
			}

			yield return datePart;
			yield return timePart;

			//as a last resort.
			yield return frame.ParentImageSop.InstanceNumber;
			yield return frame.FrameNumber;
			yield return frame.AcquisitionNumber;
		}

		/// <summary>
		/// Compares two <see cref="Frame"/>s based on acquisition date and time.
		/// </summary>
		public override int Compare(Frame x, Frame y)
		{
			return Compare(GetCompareValues(x), GetCompareValues(y));
		}
	}
}
