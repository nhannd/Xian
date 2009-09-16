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
using System.Diagnostics;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Compares two <see cref="ImageSop"/>s based on study date.
	/// </summary>
	public class StudyDateComparer : ImageSetComparer
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

		private IEnumerable<IComparable> GetCompareValues(IImageSet imageSet)
		{
			DateTime? studyDate = null;
			DateTime? studyTime = null;

			IDicomImageSetDescriptor descriptor = imageSet.Descriptor as IDicomImageSetDescriptor;
			if (descriptor == null)
			{
				if (imageSet.DisplaySets.Count == 0)
				{
					Debug.Assert(false, "All image sets being sorted must have at least one display set with at least one image in order for them to be sorted properly.");
				}
				else if (imageSet.DisplaySets[0].PresentationImages.Count == 0)
				{
					Debug.Assert(false, "All image sets being sorted must have at least one display set with at least one image in order for them to be sorted properly.");
				}
				else 
				{
					ISopProvider provider = imageSet.DisplaySets[0].PresentationImages[0] as ISopProvider;
					if (provider != null)
					{
						studyDate = DateParser.Parse(provider.Sop.StudyDate);
						studyTime = TimeParser.Parse(provider.Sop.StudyTime);
					}
				}
			}
			else
			{
				studyDate = DateParser.Parse(descriptor.SourceStudy.StudyDate);
				studyTime = TimeParser.Parse(descriptor.SourceStudy.StudyTime);
			}

			yield return studyDate;
			yield return studyTime;
			yield return imageSet.Name;
		}

		public override int Compare(IImageSet x, IImageSet y)
		{
			return Compare(GetCompareValues(x), GetCompareValues(y));
		}
	}
}
