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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	public delegate void CreateVolumeProgressCallback(int currentFrame, int totalFrames);

	public sealed class CreateVolumeException : Exception
	{
		//TODO (cr Oct 2009): take out of resources
		public CreateVolumeException() : base(SR.MessageUnexpectedCreateVolumeException) { }
		public CreateVolumeException(string message) : base(message) {}
		public CreateVolumeException(string message, Exception innerException) : base(message, innerException) {}
	}

	partial class Volume
	{
		//TODO (cr Oct 2009): 'Create'
		public static Volume CreateVolume(IDisplaySet displaySet)
		{
			return CreateVolume(displaySet, null);
		}

		public static Volume CreateVolume(IDisplaySet displaySet, CreateVolumeProgressCallback callback)
		{
			Platform.CheckForNullReference(displaySet, "displaySet");
			List<Frame> frames = new List<Frame>();

			//TODO (cr Oct 2009): not guaranteed, check and throw exception
			foreach (IImageSopProvider imageSopProvider in displaySet.PresentationImages)
				frames.Add(imageSopProvider.Frame);
			return CreateVolume(frames, callback);
		}

		public static Volume CreateVolume(IEnumerable<Frame> frames)
		{
			return CreateVolume(frames, null);
		}

		public static Volume CreateVolume(IEnumerable<Frame> frames, CreateVolumeProgressCallback callback)
		{
			Platform.CheckForNullReference(frames, "frames");
			using (VolumeBuilder builder = new VolumeBuilder(frames, callback))
			{
				return builder.Build();
			}
		}

		public static Volume CreateVolume(IEnumerable<string> filenames)
		{
			return CreateVolume(filenames, null);
		}

		public static Volume CreateVolume(IEnumerable<string> filenames, CreateVolumeProgressCallback callback)
		{
			Platform.CheckForNullReference(filenames, "filenames");
			List<ImageSop> loadedImageSops = new List<ImageSop>();
			try
			{
				foreach (string fileName in filenames)
				{
					try
					{
						//TODO (cr Oct 2009): ImageSop.CreateFromFile(filename)
						loadedImageSops.Add(new ImageSop(new LocalSopDataSource(fileName)));
					}
					catch (Exception ex)
					{
						throw new CreateVolumeException(string.Format(SR.MessageUnsupportedDicomImageSop, fileName), ex);
					}
				}

				List<Frame> frames = new List<Frame>();
				foreach (ImageSop sop in loadedImageSops)
					frames.AddRange(sop.Frames);
				return CreateVolume(frames, callback);
			}
			finally
			{
				foreach (ImageSop sop in loadedImageSops)
					sop.Dispose();
			}
		}
	}
}