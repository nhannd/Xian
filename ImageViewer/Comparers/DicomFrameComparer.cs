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
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Base class for comparers that compare some aspect of
	/// <see cref="ImageSop"/>.
	/// </summary>
	public abstract class DicomFrameComparer : PresentationImageComparer
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

		#region IComparer<IPresentationImage> Members

		/// <summary>
		/// Compares two <see cref="IPresentationImage"/>s.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public override int Compare(IPresentationImage x, IPresentationImage y)
		{
			IImageSopProvider image1 = x as IImageSopProvider;
			IImageSopProvider image2 = y as IImageSopProvider;

			//just push non-DICOM images down to one end of the list.
			if (image1 == null)
			{
				if (image2 == null)
					return 0; // x == y
				else
					return (-this.ReturnValue); // x > y (because we want it at the end for non-reverse sorting)
			}
			else
			{
				if (image2 == null)
					return this.ReturnValue; // x < y (because we want it at the end for non-reverse sorting)
			}

			int studyUIDCompare = String.Compare(image1.ImageSop.StudyInstanceUID, image2.ImageSop.StudyInstanceUID);
			
			if (studyUIDCompare < 0)
			{
				return this.ReturnValue; // x < y
			}
			else if (studyUIDCompare == 0)
			{
				int seriesUIDCompare = String.Compare(image1.ImageSop.SeriesInstanceUID, image2.ImageSop.SeriesInstanceUID);

				if (seriesUIDCompare < 0)
				{
					return this.ReturnValue; // x < y
				}
				else if (seriesUIDCompare == 0)
				{
					return Compare(image1.Frame, image2.Frame);
				}
			}

			return (-this.ReturnValue); // x > y
		}

		#endregion

		/// <summary>
		/// Compares two <see cref="ImageSop"/>s.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected abstract int Compare(Frame x, Frame y);
	}
}
