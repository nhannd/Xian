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

using System.Collections.Generic;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Base class for comparers that compare some aspect of
	/// DICOM series.
	/// </summary>
	public abstract class DicomSeriesComparer : DisplaySetComparer, IComparer<Series>, IComparer<Sop>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DicomSeriesComparer"/>.
		/// </summary>
		protected DicomSeriesComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomSeriesComparer"/>.
		/// </summary>
		protected DicomSeriesComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IDisplaySet> Members

		/// <summary>
		/// Compares two <see cref="IDisplaySet"/>s.
		/// </summary>
		public override int Compare(IDisplaySet x, IDisplaySet y)
		{
			if (x.PresentationImages.Count == 0 || y.PresentationImages.Count == 0)
				return 0;

			IImageSopProvider provider1 = x.PresentationImages[0] as IImageSopProvider;
			IImageSopProvider provider2 = y.PresentationImages[0] as IImageSopProvider;

			if (provider1 == null)
			{
				if (provider2 == null)
					return 0; // x == y
				else
					return -ReturnValue; // x > y (because we want it at the end for non-reverse sorting)
			}
			else
			{
				if (provider2 == null)
					return ReturnValue; // x < y (because we want it at the end for non-reverse sorting)
			}

			return Compare(provider1.ImageSop, provider2.ImageSop);
		}

		#endregion

		/// <summary>
		/// Compares two <see cref="Series"/>.
		/// </summary>
		/// <remarks>Simply calls <see cref="Compare(ClearCanvas.ImageViewer.StudyManagement.Sop,ClearCanvas.ImageViewer.StudyManagement.Sop)"/>,
		/// passing the first <see cref="Sop"/> in each <see cref="Series"/>.</remarks>
		public int Compare(Series x, Series y)
		{
			if (x.Sops.Count == 0 || y.Sops.Count == 0)
				return 0;

			return Compare(x.Sops[0], y.Sops[0]);
		}

		/// <summary>
		/// Compares two <see cref="ImageSop"/>s.
		/// </summary>
		/// <remarks>
		/// The relevant DICOM series property to be compared
		/// is taken from the <see cref="ImageSop"/>.
		/// </remarks>
		public abstract int Compare(Sop x, Sop y);
	}
}
