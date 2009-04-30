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
using ClearCanvas.ImageViewer.StudyManagement;
using vtk;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <summary>
	/// The VolumeSliceImageSop maintains a reference to the volume and slice matrix information so that
	/// it can generate its pixel data on demand.
	/// </summary>
	internal class VolumeSliceImageSop : ImageSop
	{
		private readonly Volume _volume;
		private readonly vtkMatrix4x4 _sliceMatrix;

		readonly FrameCollection _volumeSliceFrames = new FrameCollection();

		internal VolumeSliceImageSop(Volume vol, vtkMatrix4x4 resliceMatrix, DicomMessageBase dicomMessage)
			: base(dicomMessage)
		{
			_volume = vol;
			_sliceMatrix = resliceMatrix;

			// Ensure that we have our one and only frame
			VolumeSliceFrame frame = new VolumeSliceFrame(this, 1);
			_volumeSliceFrames.Add(frame);
		}

		// exposed to allow Frame to generate pixel data on demand
		internal Volume Volume
		{
			get { return _volume; }
		}

		// exposed to allow Frame to generate pixel data on demand
		internal vtkMatrix4x4 SliceMatrix
		{
			get { return _sliceMatrix; }
		}

		// exposed to allow VolumeSlicer to update header of generated Sop/Frame
		internal DicomFile DicomFile
		{
			get { return base.NativeDicomObject as DicomFile; }
		}

		//ggerade ToRes: See how this will work with Stewart's refactorings.
		//	I figured this was consistent with VolumeSliceImageSops being limited to one frame. I 
		//	hid (nope, tried to hide see below) the Frames collection and use this instead.
		// FollowUp: Hiding Frames didn't work, it needs to be overridden due to calls made by the framework.
		//	So this method is just the preferred way to access the only frame of this Sop.
		public VolumeSliceFrame Frame
		{
			get { return (VolumeSliceFrame)Frames[1]; }	
		}

		public override FrameCollection Frames
		{
			get
			{
				//ggerade: Original code from base class, left around for review purposes
				//if (_frames == null)
				//{
				//    lock (_syncLock)
				//    {
				//        CheckIsDisposed();
				//        if (_frames == null)
				//        {
				//            _frames = new FrameCollection();
//ggerade: In this Sop I want to control creation of frames (I think?), so I had to override this property.
// This seems like "side-effect" behavior anyway, doesn't it?
// FollowUp: I think I could make this all work with the base FrameCollection and methods, I just want to
//	review overall architecture with Stewart before undoing all of this
//							for (int i = 1; i <= this.NumberOfFrames; i++)
//								this.Frames.Add(CreateFrame(i));
				//        }
				//    }
				//}

				return _volumeSliceFrames;
			}
		}

		// Because I've overriden the creation of frames in the Frames.get method above, this should never be called
		protected override Frame CreateFrame(int index)
		{
			// Note: when thrown the exception was translated, making the fact that this happened not obvious...
			//	This assert should make it obvious.
			Debug.Assert(false, "VolumeSliceImageSop.CreateFrame called unexpectedly");
			throw new NotSupportedException();
		}
	}
}
