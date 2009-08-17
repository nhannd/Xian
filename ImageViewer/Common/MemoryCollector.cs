using System;
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Common
{
	public static partial class MemoryManager
	{
		private class MemoryCollector
		{
			private readonly object _waitObject = new object();
			private readonly TimeSpan _waitTimeout;

			public MemoryCollector(TimeSpan waitTimeout)
			{
				// -1 is acceptable because it's a special value for Infinite
				if (waitTimeout < TimeSpan.Zero && waitTimeout.TotalMilliseconds != -1)
					throw new ArgumentException("The specified timeout is less than zero.", "waitTimeout");

				_waitTimeout = waitTimeout;
			}

			public void Collect()
			{
				lock (_waitObject)
				{
					lock (_syncLock)
					{
						if (_collectionThread == null)
						{
							Platform.Log(LogLevel.Debug, "Collect called with no objects in the cache; returning immediately.");
							return;
						}
						else if (_waitTimeout == TimeSpan.Zero)
						{
							Platform.Log(LogLevel.Debug, "Collect called with zero wait time; returning immediately.");

							//don't wait if wait time is zero, just signal the collection
							Monitor.Pulse(_syncLock);
							return;
						}

						++_waitingClients;
						MemoryCollected += OnMemoryCollected;
						Monitor.Pulse(_syncLock);
					}

					Platform.Log(LogLevel.Debug, "Waiting for memory collection to complete.");

					Monitor.Wait(_waitObject, _waitTimeout);

					lock (_syncLock)
					{
						MemoryCollected -= OnMemoryCollected;
					}
				}
			}

			private void OnMemoryCollected(object sender, MemoryCollectedEventArgs args)
			{
				if (args.IsLast)
				{
					lock (_waitObject)
					{
						Platform.Log(LogLevel.Debug, "'Last' memory collection signal detected; releasing waiting thread.");
						Monitor.Pulse(_waitObject);
					}

					lock (_syncLock)
					{
						--_waitingClients;
					}
				}
			}
		}
	}
}
