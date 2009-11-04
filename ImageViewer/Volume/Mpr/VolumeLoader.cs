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
		public CreateVolumeException() : base("An unexpected exception was encountered while creating the volume.") { }
		public CreateVolumeException(string message) : base(message) {}
		public CreateVolumeException(string message, Exception innerException) : base(message, innerException) {}
	}

	partial class Volume
	{
		public static Volume Create(IDisplaySet displaySet)
		{
			return Create(displaySet, null);
		}

		public static Volume Create(IDisplaySet displaySet, CreateVolumeProgressCallback callback)
		{
			Platform.CheckForNullReference(displaySet, "displaySet");
			List<Frame> frames = new List<Frame>();

			foreach (IPresentationImage image in displaySet.PresentationImages)
			{
				IImageSopProvider imageSopProvider = image as IImageSopProvider;
				if (imageSopProvider == null)
					throw new ArgumentException("Images must be valid IImageSopProviders.");
				frames.Add(imageSopProvider.Frame);
			}
			return Create(frames, callback);
		}

		public static Volume Create(IEnumerable<Frame> frames)
		{
			return Create(frames, null);
		}

		public static Volume Create(IEnumerable<Frame> frames, CreateVolumeProgressCallback callback)
		{
			Platform.CheckForNullReference(frames, "frames");
			using (VolumeBuilder builder = new VolumeBuilder(frames, callback))
			{
				return builder.Build();
			}
		}

		public static Volume Create(IEnumerable<string> filenames)
		{
			return Create(filenames, null);
		}

		public static Volume Create(IEnumerable<string> filenames, CreateVolumeProgressCallback callback)
		{
			Platform.CheckForNullReference(filenames, "filenames");
			List<ImageSop> loadedImageSops = new List<ImageSop>();
			try
			{
				foreach (string fileName in filenames)
				{
					try
					{
						loadedImageSops.Add(ImageSop.Create(fileName));
					}
					catch (Exception ex)
					{
						throw new CreateVolumeException(string.Format(SR.MessageUnsupportedDicomImageSop, fileName), ex);
					}
				}

				List<Frame> frames = new List<Frame>();
				foreach (ImageSop sop in loadedImageSops)
					frames.AddRange(sop.Frames);
				return Create(frames, callback);
			}
			finally
			{
				foreach (ImageSop sop in loadedImageSops)
					sop.Dispose();
			}
		}
	}
}