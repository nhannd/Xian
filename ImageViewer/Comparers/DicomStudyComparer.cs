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

using System.Diagnostics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Base class for comparers that compare some aspect of
	/// DICOM studies.
	/// </summary>
	public abstract class DicomStudyComparer : ImageSetComparer
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
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
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

			IImageSopProvider provider1 = displaySet1.PresentationImages[0] as IImageSopProvider;
			IImageSopProvider provider2 = displaySet2.PresentationImages[0] as IImageSopProvider;

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

			return Compare(provider1.ImageSop, provider2.ImageSop);
		}

		#endregion

		/// <summary>
		/// Compares two <see cref="ImageSop"/>s.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		/// <remarks>
		/// The relevant DICOM series property to be compared
		/// is taken from the <see cref="ImageSop"/>.
		/// </remarks>
		protected abstract int Compare(ImageSop x, ImageSop y);
	}
}
