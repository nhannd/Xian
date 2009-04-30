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

using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	//TODO: share a 'model' data source between all the slices and remove it from Volume class.
	//class ModelVolumeSliceSopDataSource : DicomMessageSopDataSource
	//{
	//    private readonly Volume _volume;
	//    private readonly DicomMessageBase _modelDicom;
	//}

	class VolumeSliceSopDataSource : DicomMessageSopDataSource 
	{
		private readonly VolumeSlicer _volumeSlicer;
		private readonly Matrix _resliceMatrix;

		internal VolumeSliceSopDataSource(DicomMessageBase sourceMessage, VolumeSlicer slicer, Matrix resliceMatrix)
			: base(sourceMessage)
		{
			_volumeSlicer = slicer;
			_resliceMatrix = new Matrix(resliceMatrix);
		}

		protected override byte[] CreateFrameNormalizedPixelData(int frameNumber)
		{
			return _volumeSlicer.GenerateFrameNormalizedPixelData(_resliceMatrix);
		}
	}
}
