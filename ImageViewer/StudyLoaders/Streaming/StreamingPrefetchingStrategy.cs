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

using System.Diagnostics;
using System.Threading;
using ClearCanvas.ImageViewer.StudyManagement;
using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyLoaders.Streaming
{
	internal class StreamingPrefetchingStrategy : PrefetchingStrategy
	{
		private ViewerFrameEnumerator _unselectedImageBoxEnumerator;
		private BlockingThreadPool<Frame> _retrieveThreadPool;
		private SimpleBlockingThreadPool _decompressThreadPool;

		private int _activeLoadThreads = 0;
		private int _activeRetrieveThreads = 0;

		public StreamingPrefetchingStrategy()
			: base("CC_STREAMING", SR.DescriptionPrefetchingStrategy)
		{
		}

		protected override void OnStart()
		{
			InternalStart();
		}

		private void InternalStart()
		{
			int retrieveConcurrency = StreamingSettings.Default.RetrieveConcurrency;
			if (retrieveConcurrency == 0)
				return;

			_unselectedImageBoxEnumerator = new ViewerFrameEnumerator(base.ImageViewer, 
				Math.Max(StreamingSettings.Default.SelectedWeighting, 1), 
				Math.Max(StreamingSettings.Default.UnselectedWeighting, 0), 
				StreamingSettings.Default.ImageWindow);

			_retrieveThreadPool = new BlockingThreadPool<Frame>(_unselectedImageBoxEnumerator, RetrieveFrame);
			_retrieveThreadPool.ThreadPoolName = "Retrieve";
			_retrieveThreadPool.Concurrency = retrieveConcurrency;
			_retrieveThreadPool.ThreadPriority = ThreadPriority.BelowNormal;
			_retrieveThreadPool.Start();

			int decompressConcurrency = Math.Max(StreamingSettings.Default.DecompressConcurrency, 1);
			_decompressThreadPool = new SimpleBlockingThreadPool(decompressConcurrency);
			_decompressThreadPool.ThreadPoolName = "Decompress";
			_decompressThreadPool.ThreadPriority = ThreadPriority.BelowNormal;
			_decompressThreadPool.Start();
		}
		
		protected override void OnStop()
		{
			InternalStop();
		}

		private void InternalStop()
		{
			if (_retrieveThreadPool != null)
			{
				_retrieveThreadPool.Stop(false);
				_retrieveThreadPool = null;
			}

			if (_decompressThreadPool != null)
			{
				_decompressThreadPool.Stop(false);
				_decompressThreadPool = null;
			}

			if (_unselectedImageBoxEnumerator != null)
			{
				_unselectedImageBoxEnumerator.Dispose();
				_unselectedImageBoxEnumerator = null;
			}
		}

		private void RetrieveFrame(Frame frame)
		{
			try
			{
				//just return if the available memory is getting low - only retrieve and decompress on-demand now.
				if (SystemResources.GetAvailableMemory(SizeUnits.Megabytes) < StreamingSettings.Default.AvailableMemoryLimitMegabytes)
					return;

				Interlocked.Increment(ref _activeRetrieveThreads);

				string message = String.Format("Retrieving Frame (active threads: {0})", Thread.VolatileRead(ref _activeRetrieveThreads));
				Trace.WriteLine(message);

				StreamingSopDataSource dataSource = (StreamingSopDataSource)frame.ParentImageSop.DataSource;
				IStreamingSopFrameData frameData = (IStreamingSopFrameData)(dataSource.GetFrameData(frame.FrameNumber));

				frameData.RetrievePixelData();
				_decompressThreadPool.Enqueue(delegate { LoadFramePixelData(frame); });
			}
			catch(OutOfMemoryException)
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

		private void LoadFramePixelData(Frame frame)
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
