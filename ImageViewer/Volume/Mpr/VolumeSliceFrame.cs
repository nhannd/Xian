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

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <summary>
	/// The VolumeSliceFrame is owned by the VolumeSliceImageSop which maintains references to
	/// the volume and slice matrix which are used to generate the pixel data for this frame.
	/// </summary>
	internal class VolumeSliceFrame : Frame
	{
		private byte[] _pixelData;

		public VolumeSliceFrame(VolumeSliceImageSop parentImageSop, int frameNumber)
			: base(parentImageSop, frameNumber)
		{
		}

		public void SetPixelData(byte[] pixelData)
		{
			_pixelData = pixelData;
		}

		protected override byte[] CreateNormalizedPixelData()
		{
			if (_pixelData == null)
			{
				_pixelData = VolumeSlicer.GenerateFramePixelData(ParentVolumeSliceImageSop.Volume,
				                                                 ParentVolumeSliceImageSop.SliceMatrix);
			}

			return _pixelData;
		}

		private VolumeSliceImageSop ParentVolumeSliceImageSop
		{
			get { return (VolumeSliceImageSop)ParentImageSop; }
		}
	}
}