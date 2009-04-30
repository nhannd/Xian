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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Base class for comparing <see cref="Frame"/>s.
	/// </summary>
	public abstract class DicomFrameComparer : PresentationImageComparer, IComparer<Frame>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DicomFrameComparer"/>.
		/// </summary>
		protected DicomFrameComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DicomFrameComparer"/>.
		/// </summary>
		protected DicomFrameComparer(bool reverse)
			: base(reverse)
		{
		}

		private static IEnumerable<IComparable> GetCompareValues(IImageSopProvider provider)
		{
			yield return provider.ImageSop.StudyInstanceUID;
			yield return provider.ImageSop.SeriesInstanceUID;
		}

		#region IComparer<IPresentationImage> Members

		/// <summary>
		/// Compares two <see cref="IPresentationImage"/>s.
		/// </summary>
		public override int Compare(IPresentationImage x, IPresentationImage y)
		{
			if (x == y)
				return 0; //same reference object

			IImageSopProvider xProvider = x as IImageSopProvider;
			IImageSopProvider yProvider = y as IImageSopProvider;

			if (ReferenceEquals(xProvider, yProvider))
				return 0; //same object or both are null

			//at this point, at least one of x or y is non-null and they are not the same object

			if (xProvider == null)
				return -ReturnValue; // x > y (because we want x at the end for non-reverse sorting)
			if (yProvider == null)
				return ReturnValue; // x < y (because we want y at the end for non-reverse sorting)

			int compare = Compare(GetCompareValues(xProvider), GetCompareValues(yProvider));
			if (compare == 0 && !ReferenceEquals(xProvider.Frame, yProvider.Frame))
				compare = Compare(xProvider.Frame, yProvider.Frame);

			return compare;
		}

		#endregion

		/// <summary>
		/// Compares two <see cref="Frame"/>s.
		/// </summary>
		public abstract int Compare(Frame x, Frame y);
	}
}
