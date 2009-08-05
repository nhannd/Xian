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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <summary>
	/// A standard 3-plane slice view of an MPR <see cref="Volume"/>.
	/// </summary>
	public sealed class Mpr3PlaneDisplaySet : DisplaySet
	{
		private static readonly IVolumeSlicerParams[] _planes = new IVolumeSlicerParams[] {VolumeSlicerParams.Identity, VolumeSlicerParams.OrthogonalX, VolumeSlicerParams.OrthogonalY};
		private IVolumeReference _volume;

		public Mpr3PlaneDisplaySet(Volume volume) : this(volume, null, null) {}

		public Mpr3PlaneDisplaySet(Volume volume, string name) : this(volume, name, DicomUid.GenerateUid().UID) {}

		public Mpr3PlaneDisplaySet(Volume volume, string name, string uid)
			: base(name, uid)
		{
			Platform.CheckForNullReference(volume, "volume");

			_volume = volume.CreateTransientReference();

			base.Description = "3-Plane";

			this.Reslice();
		}

		public Volume Volume
		{
			get { return _volume.Volume; }
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_volume.Dispose();
				_volume = null;
			}
			base.Dispose(disposing);
		}

		private void Reslice()
		{
			List<IPresentationImage> images = new List<IPresentationImage>(this.PresentationImages);
			this.PresentationImages.Clear();
			foreach (IPresentationImage image in images)
				image.Dispose();

			int instanceNumber = 0;
			foreach (IVolumeSlicerParams slicerParams in _planes)
			{
				using (VolumeSlicer slicer = new VolumeSlicer(_volume.Volume, slicerParams, base.Uid))
				{
					foreach (VolumeSliceSopDataSource dataSource in slicer.CreateSlices())
					{
						// we're appending multiple slicings, so override the instance number that the slicer generates
						dataSource[DicomTags.InstanceNumber].SetInt32(0, ++instanceNumber);

						ImageSop imageSop = new ImageSop(dataSource);
						foreach (IPresentationImage image in PresentationImageFactory.Create(imageSop))
						{
							this.PresentationImages.Add(image);
						}
					}
				}
			}
		}
	}
}