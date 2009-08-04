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
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	// TODO JY
	public class VolumeSliceSopDataSource : StandardSopDataSource 
	{
		private readonly VolumeSlicer _volumeSlicer;
		private readonly Matrix _resliceMatrix;
		private readonly IDicomAttributeProvider _volumeSopDataSourcePrototype;
		private readonly DicomAttributeCollection _instanceAttributes;

		internal VolumeSliceSopDataSource(IDicomAttributeProvider sourceMessage, VolumeSlicer slicer, Matrix resliceMatrix)
		{
			_volumeSlicer = slicer;
			_resliceMatrix = new Matrix(resliceMatrix);
			_volumeSopDataSourcePrototype = sourceMessage;
			_instanceAttributes = new DicomAttributeCollection(); 
		}

		public override DicomAttribute this[DicomTag tag]
		{
			get
			{
				DicomAttribute attribute;
				if (_volumeSopDataSourcePrototype.TryGetAttribute(tag, out attribute))
					return attribute;
				return _instanceAttributes[tag];
			}
		}

		public override DicomAttribute this[uint tag]
		{
			get
			{
				DicomAttribute attribute;
				if (_volumeSopDataSourcePrototype.TryGetAttribute(tag, out attribute))
					return attribute;
				return _instanceAttributes[tag];
			}
		}

		public override bool TryGetAttribute(DicomTag tag, out DicomAttribute attribute)
		{
			if (_volumeSopDataSourcePrototype.TryGetAttribute(tag, out attribute))
				return true;
			return _instanceAttributes.TryGetAttribute(tag, out attribute);
		}

		public override bool TryGetAttribute(uint tag, out DicomAttribute attribute)
		{
			if (_volumeSopDataSourcePrototype.TryGetAttribute(tag, out attribute))
				return true;
			return _instanceAttributes.TryGetAttribute(tag, out attribute);
		}

		protected override StandardSopFrameData CreateFrameData(int frameNumber)
		{
			return new VolumeSliceSopFrameData(frameNumber, this);
		}

		protected class VolumeSliceSopFrameData : StandardSopFrameData
		{
			public VolumeSliceSopFrameData(int frameNumber, VolumeSliceSopDataSource parent) : base(frameNumber, parent) {}

			public new VolumeSliceSopDataSource Parent
			{
				get { return (VolumeSliceSopDataSource) base.Parent; }
			}

			protected override byte[] CreateNormalizedPixelData()
			{
				return this.Parent._volumeSlicer.GenerateFrameNormalizedPixelData(this.Parent._resliceMatrix);
			}

			protected override void OnUnloaded()
			{
				base.OnUnloaded();
			}

			protected override byte[] CreateNormalizedOverlayData(int overlayGroupNumber, int overlayFrameNumber)
			{
				Debug.Assert(false, "We should never get here... we don't support overlays in the volume (yet)!!!");
				return new byte[0];
			}
		}
	}
}
