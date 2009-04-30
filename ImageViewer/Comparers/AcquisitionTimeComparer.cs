#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
