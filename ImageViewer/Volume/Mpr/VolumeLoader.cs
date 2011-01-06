#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	public delegate void CreateVolumeProgressCallback(int currentFrame, int totalFrames);

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

		public static Volume Create(IEnumerable<IFrameReference> frames)
		{
			return Create(frames, null);
		}

		public static Volume Create(IEnumerable<IFrameReference> frames, CreateVolumeProgressCallback callback)
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
						loadedImageSops.Add(new ImageSop(fileName));
					}
					catch (Exception ex)
					{
						throw new ArgumentException(string.Format("Files must be valid DICOM image SOPs. File: {0}", fileName), ex);
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