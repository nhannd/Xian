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

		/// <summary>
		/// Compares two <see cref="ImageSop"/>s based on instance number.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		protected override int Compare(Frame x, Frame y)
		{
			int imageNumber1 = x.ParentImageSop.InstanceNumber;
			int imageNumber2 = y.ParentImageSop.InstanceNumber;

			if (imageNumber1 < imageNumber2)
				return this.ReturnValue; // x < y
			else if (imageNumber1 > imageNumber2)
				return (-this.ReturnValue); // x > y
			else
			{
				int frameNumber1 = x.FrameNumber;
				int frameNumber2 = y.FrameNumber;

				if (frameNumber1 < frameNumber2)
					return this.ReturnValue;
				else if (frameNumber1 > frameNumber2)
					return (-this.ReturnValue);
				else
					return 0; // x == y
			}
		}
	}
}
