#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Diagnostics;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.StudyManagement;
using System;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	internal class StreamingPrefetchingStrategy : WeightedWindowPrefetchingStrategy
	{
		private int _activeLoadThreads = 0;
		private int _activeRetrieveThreads = 0;

		public StreamingPrefetchingStrategy()
			: base("CC_STREAMING", SR.DescriptionPrefetchingStrategy)
		{
			base.RetrieveConcurrency = StreamingSettings.Default.RetrieveConcurrency;
			base.DecompressConcurrency = StreamingSettings.Default.DecompressConcurrency;
			base.ImageWindow = StreamingSettings.Default.ImageWindow;
			base.SelectedWeighting = StreamingSettings.Default.SelectedWeighting;
			base.UnselectedWeighting = StreamingSettings.Default.UnselectedWeighting;
		}

		protected override bool CanRetrieveFrame(Frame frame)
		{
			if (!(frame.ParentImageSop.DataSource is StreamingSopDataSource))
				return false;

			StreamingSopDataSource dataSource = (StreamingSopDataSource)frame.ParentImageSop.DataSource;
			IStreamingSopFrameData frameData = dataSource.GetFrameData(frame.FrameNumber);
			return !frameData.PixelDataRetrieved;
		}

		protected override void RetrieveFrame(Frame frame, out bool decompress)
		{
			decompress = false;
			try
			{
				//just return if the available memory is getting low - only retrieve and decompress on-demand now.
				if (SystemResources.GetAvailableMemory(SizeUnits.Megabytes) < StreamingSettings.Default.AvailableMemoryLimitMegabytes)
					return;

				Interlocked.Increment(ref _activeRetrieveThreads);

				string message = String.Format("Retrieving Frame (active threads: {0})", Thread.VolatileRead(ref _activeRetrieveThreads));
				Trace.WriteLine(message);

				IStreamingSopDataSource dataSource = (IStreamingSopDataSource)frame.ParentImageSop.DataSource;
				IStreamingSopFrameData frameData = dataSource.GetFrameData(frame.FrameNumber);

				frameData.RetrievePixelData();
				decompress = true;
			}
			catch (OutOfMemoryException)
			{
				Platform.Log(LogLevel.Error, "Out of memory trying to retrieve pixel data.  Prefetching will not resume unless memory becomes available.");
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Error retrieving frame pixel data.");
			}
			finally
			{
				Interlocked.Decrement(ref _activeRetrieveThreads);
			}
		}

		protected override void LoadFramePixelData(Frame frame)
		{
			try
			{
				//just return if the available memory is getting low - only retrieve and decompress on-demand now.
				if (SystemResources.GetAvailableMemory(SizeUnits.Megabytes) < StreamingSettings.Default.AvailableMemoryLimitMegabytes)
					return;

				Interlocked.Increment(ref _activeLoadThreads);

				string message = String.Format("Loading Frame (active threads: {0})", Thread.VolatileRead(ref _activeLoadThreads));
				Trace.WriteLine(message);

				//TODO: try to trigger header retrieval for data luts?
				frame.GetNormalizedPixelData();
			}
			catch (OutOfMemoryException)
			{
				Platform.Log(LogLevel.Error, "Out of memory trying to decompress pixel data.  Prefetching will not resume unless memory becomes available.");
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Error loading frame pixel data.");
			}
			finally
			{
				Interlocked.Decrement(ref _activeLoadThreads);
			}
		}
	}
}