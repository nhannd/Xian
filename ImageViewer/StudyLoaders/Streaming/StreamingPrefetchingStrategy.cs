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

				((StreamingSopDataSource)frame.ParentImageSop.DataSource).RetrievePixelData(frame.FrameNumber);
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
