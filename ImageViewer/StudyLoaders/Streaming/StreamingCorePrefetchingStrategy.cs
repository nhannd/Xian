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
	internal class StreamingCorePrefetchingStrategy : ICorePrefetchingStrategy
	{
		private int _activeRetrieveThreads = 0;
		private int _activeDecompressThreads = 0;

		public bool CanRetrieveFrame(Frame frame)
		{
			if (!(frame.ParentImageSop.DataSource is StreamingSopDataSource))
				return false;

			StreamingSopDataSource dataSource = (StreamingSopDataSource) frame.ParentImageSop.DataSource;
			IStreamingSopFrameData frameData = dataSource.GetFrameData(frame.FrameNumber);
			return !frameData.PixelDataRetrieved;
		}

		public void RetrieveFrame(Frame frame)
		{
			Interlocked.Increment(ref _activeRetrieveThreads);

			try
			{
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

		public bool CanDecompressFrame(Frame frame)
		{
			if (!(frame.ParentImageSop.DataSource is StreamingSopDataSource))
				return false;

			StreamingSopDataSource dataSource = (StreamingSopDataSource) frame.ParentImageSop.DataSource;
			IStreamingSopFrameData frameData = dataSource.GetFrameData(frame.FrameNumber);
			return frameData.PixelDataRetrieved;
		}

		public void DecompressFrame(Frame frame)
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