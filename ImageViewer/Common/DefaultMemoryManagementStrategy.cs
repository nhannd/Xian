using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common
{
	internal class DefaultMemoryManagementStrategy : MemoryManagementStrategy
	{
		private const string x86Architecture = "x86";

		private const long OneKilobyte = 1024;
		private const long OneMegabyte = 1024 * OneKilobyte;
		private const long OneGigabyte = 1024 * OneMegabyte;
		private const long TwoGigabytes = 2 * OneGigabyte;
		private const long ThreeGigabytes = 3 * OneGigabyte;
		private const long EightGigabytes = 8 * OneGigabyte;

		private IEnumerator<ILargeObjectContainer> _largeObjectEnumerator;
		
		private RegenerationCost _regenerationCost;
		private DateTime _collectionStartTime;
		private DateTime _lastCollectionTime;
		private TimeSpan _timeSinceLastCollection;
		private TimeSpan _maxTimeSinceLastAccess;
		private TimeSpan _maxTimeSinceLastAccessDecrement;

		private int _totalNumberOfCollections;
		private long _totalBytesCollected;
		private int _totalLargeObjectsCollected;
		private int _totalContainersUnloaded;

		public DefaultMemoryManagementStrategy()
		{
			_lastCollectionTime = Platform.Time;
		}

		#region IMemoryManagementStrategy Members

		public override void Collect(MemoryCollectionArgs collectionArgs)
		{
			_largeObjectEnumerator = collectionArgs.LargeObjectContainers.GetEnumerator();

			_regenerationCost = RegenerationCost.Low;

			_collectionStartTime = Platform.Time;
			_timeSinceLastCollection = _collectionStartTime - _lastCollectionTime;
			TimeSpan thirtySeconds = TimeSpan.FromSeconds(30);
			if (_timeSinceLastCollection < thirtySeconds)
			{
				Platform.Log(LogLevel.Debug, "Time since last collection is less than 30 seconds; adjusting to 30 seconds.");
				_timeSinceLastCollection = thirtySeconds;
			}

			_maxTimeSinceLastAccess = _timeSinceLastCollection;
			_maxTimeSinceLastAccessDecrement = TimeSpan.FromSeconds(_timeSinceLastCollection.TotalSeconds / 3);

			_totalNumberOfCollections = 0;
			_totalBytesCollected = 0;
			_totalLargeObjectsCollected = 0;
			_totalContainersUnloaded = 0;

			try
			{
				CodeClock clock = new CodeClock();
				clock.Start();

				Collect();

				clock.Stop();
				PerformanceReportBroker.PublishReport("Memory", "Collect", clock.Seconds);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Default memory management strategy failed to collect.");
			}
			finally
			{
				DateTime collectionEndTime = Platform.Time;
				if (_totalContainersUnloaded > 0)
					_lastCollectionTime = collectionEndTime;

				_largeObjectEnumerator = null;

				TimeSpan totalElapsed = collectionEndTime - _collectionStartTime;

				MemoryCollectedEventArgs finalArgs = new MemoryCollectedEventArgs(
					_totalContainersUnloaded, _totalLargeObjectsCollected, _totalBytesCollected, totalElapsed, true);

				Platform.Log(LogLevel.Info, "Large object collection summary: freed {0} MB in {1} seconds and {2} iterations.",
							 finalArgs.BytesCollectedCount / (float)OneMegabyte, finalArgs.ElapsedTime.TotalSeconds, _totalNumberOfCollections);

				OnMemoryCollected(finalArgs);
			}
		}

		#endregion

		private bool Is3GigEnabled()
		{
			//TODO: can't actually figure this out - 1GB of image memory is pretty good :)
			return false;
		}

		private long GetProcessVirtualMemorySizeBytes()
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			try
			{
				Process currentProcess = Process.GetCurrentProcess();
				currentProcess.Refresh();

				//Private bytes is what's reported as VM size in task manager.
				//return currentProcess.PrivateMemorySize64;

				//actual virtual memory size of the process.
				return currentProcess.VirtualMemorySize64;
			}
			catch (PlatformNotSupportedException)
			{
				//This is unlikely to occur, but will give a reasonable estimate.
				return GetTotalLargeObjectBytes();
			}
			finally
			{
				clock.Stop();
				PerformanceReportBroker.PublishReport("Memory", "GetProcessVirtualMemorySize", clock.Seconds);
			}
		}

		private long GetTotalLargeObjectBytes()
		{
			long largeObjectBytes = 0;
			bool alreadyLogged = false;

			_largeObjectEnumerator.Reset();
			while (_largeObjectEnumerator.MoveNext())
			{
				try
				{
					largeObjectBytes += _largeObjectEnumerator.Current.BytesHeldCount;
				}
				catch (Exception e)
				{
					if (!alreadyLogged)
					{
						alreadyLogged = true;
						Platform.Log(LogLevel.Warn, e, "Unexpected error while estimating large object bytes.");
					}
				}
			}

			_largeObjectEnumerator.Reset();

			return largeObjectBytes;
		}

		private long GetMaxVirtualMemorySizeBytes()
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			long currentVirtualMemorySize = GetProcessVirtualMemorySizeBytes();
			long maxTheoreticalVirtualMemorySize = currentVirtualMemorySize + SystemResources.GetAvailableMemory(SizeUnits.Bytes);
			long maxVirtualMemorySizeBytes;

			if (Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") == x86Architecture)
			{
				long maxSystemVirtualMemorySize = Is3GigEnabled() ? ThreeGigabytes : TwoGigabytes;
				maxVirtualMemorySizeBytes = Math.Min(maxTheoreticalVirtualMemorySize, maxSystemVirtualMemorySize);
			}
			else
			{
				//let's not get greedy :)
				maxVirtualMemorySizeBytes = Math.Min(EightGigabytes, maxTheoreticalVirtualMemorySize);
			}

			clock.Stop();
			PerformanceReportBroker.PublishReport("Memory", "GetMaxVirtualMemorySizeBytes", clock.Seconds);

			return maxVirtualMemorySizeBytes;
		}

		private long GetVirtualMemoryHighWatermarkBytes()
		{
			if (MemoryManagementSettings.Default.HighWatermarkMegaBytes < 0)
				return GetMaxVirtualMemorySizeBytes() / 2;
			else
				return MemoryManagementSettings.Default.HighWatermarkMegaBytes * OneMegabyte;
		}

		private long GetMemoryLowWatermarkBytes(long highWatermark)
		{
			if (MemoryManagementSettings.Default.LowWatermarkMegaBytes < 0)
			{
				//High Watermark = 1GB, unload ~250MB
				//High Watermark = 2GB, unload ~500MB
				//return highWatermark/2;

				//free up 1/4 of the total large object memory.
				const float twentyFivePercent = 0.25F;
				return (long)(highWatermark - MemoryManager.LargeObjectBytesCount * twentyFivePercent);
			}
			else
			{
				const float ninetyPercent = 0.9F;
				return Math.Min(MemoryManagementSettings.Default.LowWatermarkMegaBytes * OneMegabyte, (long)(highWatermark * ninetyPercent));
			}
		}

		private IEnumerable<ILargeObjectContainer> GetNextBatchOfContainersToCollect(int batchSize)
		{
			int i = 0;
			while (_regenerationCost <= RegenerationCost.High)
			{
				bool timeGreaterThanOrEqualToZero = true;
				while (timeGreaterThanOrEqualToZero)
				{
					while (_largeObjectEnumerator.MoveNext())
					{
						ILargeObjectContainer container = _largeObjectEnumerator.Current;
						if (!container.IsLocked && container.RegenerationCost == _regenerationCost)
						{
							TimeSpan timeSinceLastAccess = _collectionStartTime - container.LastAccessTime;
							if (timeSinceLastAccess >= _maxTimeSinceLastAccess)
							{
								yield return container;

								if (++i == batchSize)
									yield break;
							}
						}
					}

					_largeObjectEnumerator.Reset();

					// check this before doing the decrement for the odd case where the decrement takes 
					// the time slightly past zero when it should be exactly zero.
					timeGreaterThanOrEqualToZero = _maxTimeSinceLastAccess >= TimeSpan.Zero;
					_maxTimeSinceLastAccess -= _maxTimeSinceLastAccessDecrement;
				}

				_maxTimeSinceLastAccess = _timeSinceLastCollection;
				++_regenerationCost;
			}
		}

		private void Collect()
		{
			long processVirtualMemorySize = GetProcessVirtualMemorySizeBytes();
			long highWatermark = GetVirtualMemoryHighWatermarkBytes();
			long lowWatermark = GetMemoryLowWatermarkBytes(highWatermark);

			long bytesAboveHighWatermark = processVirtualMemorySize - highWatermark;
			long bytesToCollect = processVirtualMemorySize - lowWatermark;
			
			bool continueCollecting;
			int collectionNumber = 0;

			if (Platform.IsLogLevelEnabled(LogLevel.Debug))
			{
				Platform.Log(LogLevel.Debug,
					"Process Virtual Memory (MB): {0}, High Watermark (MB): {1}, Low Watermark (MB): {2}, MB Above: {3}, MB To Collect: {4}",
					processVirtualMemorySize / (float)OneMegabyte,
					highWatermark / (float)OneMegabyte,
					lowWatermark / (float)OneMegabyte,
					bytesAboveHighWatermark / (float)OneMegabyte,
					bytesToCollect / (float)OneMegabyte);
			}

			bool needMoreMemorySignalled = false;
			if (bytesAboveHighWatermark <= 0)
			{
				Platform.Log(LogLevel.Debug,
					"Memory is not above high watermark; firing collected event to check if more memory is required.");

				MemoryCollectedEventArgs args = new MemoryCollectedEventArgs(0, 0, 0, TimeSpan.Zero, false);
				OnMemoryCollected(args);
				continueCollecting = needMoreMemorySignalled = args.NeedMoreMemory;
			}
			else
			{
				continueCollecting = true;
				Platform.Log(LogLevel.Debug, "Memory *is* above high watermark; collecting ...");
			}

			int batchSize = 10;

			while (continueCollecting)
			{
				CodeClock clock = new CodeClock();
				clock.Start();

				long bytesCollected = 0;
				int largeObjectsCollected = 0;
				int containersUnloaded = 0;
				int i = 0;

				foreach (ILargeObjectContainer container in GetNextBatchOfContainersToCollect(batchSize))
				{
					++i;

					try
					{
						long bytesHeldBefore = container.BytesHeldCount;
						int largeObjectsHeldBefore = container.LargeObjectCount;

						container.Unload();

						long bytesHeldAfter = container.BytesHeldCount;
						int largeObjectsHeldAfter = container.LargeObjectCount;

						++containersUnloaded;
						largeObjectsCollected += (largeObjectsHeldBefore - largeObjectsHeldAfter);

						long bytesDifference = (bytesHeldBefore - bytesHeldAfter);
						bytesCollected += bytesDifference;
						_totalBytesCollected += bytesDifference;
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Warn, e, "An unexpected error occurred while attempting to collect large object memory.");
					}

					//when needMoreMemorySignalled is true, we need to be more aggressive and keep collecting.
					if (!needMoreMemorySignalled && _totalBytesCollected >= bytesToCollect)
						break;
				}

				batchSize *= 2;

				_totalContainersUnloaded += containersUnloaded;
				_totalLargeObjectsCollected += largeObjectsCollected;
				++_totalNumberOfCollections;

				GC.Collect();

				clock.Stop();

				continueCollecting = i > 0;

				if (continueCollecting)
				{
					PerformanceReportBroker.PublishReport("Memory", "CollectionIteration", clock.Seconds);

					MemoryCollectedEventArgs args = new MemoryCollectedEventArgs(
						containersUnloaded, largeObjectsCollected, bytesCollected, TimeSpan.FromSeconds(clock.Seconds), false);

					OnMemoryCollected(args);

					needMoreMemorySignalled = args.NeedMoreMemory;
					continueCollecting = needMoreMemorySignalled || _totalBytesCollected < bytesToCollect;

					if (Platform.IsLogLevelEnabled(LogLevel.Debug))
					{
						Platform.Log(LogLevel.Debug, 
							"Large object collection #{0}: freed {1} MB in {2}, Containers Unloaded: {3}, Large Objects Collected: {4}, Need More Memory: {5}, Last Batch: {6}",
							++collectionNumber, args.BytesCollectedCount / (float)OneMegabyte, clock,
							containersUnloaded, largeObjectsCollected, needMoreMemorySignalled, i);
					}
				}
			}
		}
	}
}
