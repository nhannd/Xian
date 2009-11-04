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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <summary>
	/// A basic, immutable, single-plane slice view of an MPR <see cref="Volume"/>.
	/// </summary>
	public class MprStaticSliceSet : MprSliceSet, IMprStandardSliceSet
	{
		private readonly IVolumeSlicerParams _slicerParams;

		public MprStaticSliceSet(Volume volume, IVolumeSlicerParams slicerParams)
			: base(volume)
		{
			Platform.CheckForNullReference(slicerParams, "slicerParams");
			_slicerParams = slicerParams;

			base.Description = slicerParams.Description;
			this.Reslice();
		}

		public IVolumeSlicerParams SlicerParams
		{
			get { return _slicerParams; }
		}

		bool IMprStandardSliceSet.IsReadOnly
		{
			get { return true; }
		}

		IVolumeSlicerParams IMprStandardSliceSet.SlicerParams
		{
			get { return this.SlicerParams; }
			set { throw new NotSupportedException(); }
		}

		event EventHandler IMprStandardSliceSet.SlicerParamsChanged
		{
			add { }
			remove { }
		}

		protected void Reslice()
		{
			base.SuspendSliceSopsChangedEvent();
			try
			{
				base.ClearAndDisposeSops();

				using (VolumeSlicer slicer = new VolumeSlicer(base.Volume, _slicerParams, base.Uid))
				{
					foreach (ISopDataSource dataSource in slicer.CreateSlices())
					{
						base.SliceSops.Add(new MprSliceSop(dataSource));
					}
				}
			}
			finally
			{
				base.ResumeSliceSopsChangedEvent(true);
			}
		}

		public static MprStaticSliceSet CreateIdentitySliceSet(Volume volume)
		{
			return new MprStaticSliceSet(volume, VolumeSlicerParams.Identity);
		}
	}
}