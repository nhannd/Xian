#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Diagnostics;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	internal class StreamingPrefetchingStrategy : WeightedWindowPrefetchingStrategy
	{
		private int _activeRetrieveThreads = 0;
		private int _activeDecompressThreads = 0;

		public StreamingPrefetchingStrategy()
			: base("CC_STREAMING", SR.DescriptionPrefetchingStrategy)
		{
			Enabled = StreamingSettings.Default.RetrieveConcurrency > 0;
			RetrievalThreadConcurrency = Math.Max(StreamingSettings.Default.RetrieveConcurrency, 1);
			DecompressionThreadConcurrency = Math.Max(StreamingSettings.Default.DecompressConcurrency, 1);
			FrameLookAheadSize = StreamingSettings.Default.ImageWindow >= 0 ? (int?) StreamingSettings.Default.ImageWindow : null;
			SelectedFrameWeight = Math.Max(StreamingSettings.Default.SelectedWeighting, 1);
			UnselectedFrameWeight = Math.Max(StreamingSettings.Default.UnselectedWeighting, 0);
		}

		protected override bool CanRetrieveFrame(Frame frame)
		{
			if (!(frame.ParentImageSop.DataSource is StreamingSopDataSource))
				return false;

			StreamingSopDataSource dataSource = (StreamingSopDataSource) frame.ParentImageSop.DataSource;
			IStreamingSopFrameData frameData = dataSource.GetFrameData(frame.FrameNumber);
			return !frameData.PixelDataRetrieved;
		}

		protected override void RetrieveFrame(Frame frame)
		{
			Interlocked.Increment(ref _activeRetrieveThreads);
			try
			{
				string message = String.Format("Retrieving Frame (active threads: {0})", Thread.VolatileRead(ref _activeRetrieveThreads));
				Trace.WriteLine(message);

				IStreamingSopDataSource dataSource = (IStreamingSopDataSource) frame.ParentImageSop.DataSource;
				IStreamingSopFrameData frameData = dataSource.GetFrameData(frame.FrameNumber);

				frameData.RetrievePixelData();
			}
			catch (OutOfMemoryException)
			{
				Platform.Log(LogLevel.Error, "Out of memory trying to retrieve pixel data.");
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

		protected override bool CanDecompressFrame(Frame frame)
		{
			if (!(frame.ParentImageSop.DataSource is StreamingSopDataSource))
				return false;

			StreamingSopDataSource dataSource = (StreamingSopDataSource) frame.ParentImageSop.DataSource;
			IStreamingSopFrameData frameData = dataSource.GetFrameData(frame.FrameNumber);
			return frameData.PixelDataRetrieved;
		}

		protected override void DecompressFrame(Frame frame)
		{
			Interlocked.Increment(ref _activeDecompressThreads);
			try
			{
				string message = String.Format("Decompressing Frame (active threads: {0})", Thread.VolatileRead(ref _activeDecompressThreads));
				Trace.WriteLine(message);

				//TODO: try to trigger header retrieval for data luts?
				frame.GetNormalizedPixelData();
			}
			catch (OutOfMemoryException)
			{
				Platform.Log(LogLevel.Error, "Out of memory trying to decompress pixel data.");
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Error decompressing frame pixel data.");
			}
			finally
			{
				Interlocked.Decrement(ref _activeDecompressThreads);
			}
		}
	}
}