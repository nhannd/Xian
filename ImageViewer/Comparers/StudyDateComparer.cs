#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Compares two <see cref="ImageSop"/>s based on study date.
	/// </summary>
	public class StudyDateComparer : DicomStudyComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="StudyDateComparer"/>.
		/// </summary>
		/// <remarks>
		/// By default, the <see cref="ComparerBase.Reverse"/> property is set
		/// to true because, normally, we want the image sets sorted with the
		/// most recent studies at the beginning.
		/// </remarks>
		public StudyDateComparer()
			: this(true)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="StudyDateComparer"/>.
		/// </summary>
		public StudyDateComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IDisplaySet> Members

		/// <summary>
		/// Compares two <see cref="ImageSop"/>s based on study date.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override int Compare(ImageSop x, ImageSop y)
		{
			DateTime? studyDate1 = DateParser.Parse(x.StudyDate);
			DateTime? studyDate2 = DateParser.Parse(y.StudyDate);

			if (studyDate1 == null)
			{
				if (studyDate2 == null)
					return 0;
				else
					return ReturnValue; // x < y, we want null dates at the beginning for non-reverse sorting.
			}
			else
			{
				if (studyDate2 == null)
					return -ReturnValue; // x > y, we want null dates at the beginning for non-reverse sorting.
			}

			if ((DateTime)studyDate1 < (DateTime)studyDate2)
				return this.ReturnValue;
			else if ((DateTime)studyDate1 > (DateTime)studyDate2)
				return -this.ReturnValue;
			else
				return 0;
		}

		#endregion
	}
}
