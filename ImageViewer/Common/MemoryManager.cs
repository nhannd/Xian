using System;
using ClearCanvas.Common;
using System.Threading;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common
{
	public sealed class MemoryManagementStrategyExtensionPoint : ExtensionPoint<IMemoryManagementStrategy>
	{ }

	public static partial class MemoryManager
	{
		public delegate void RetryableCommand();

		#region Private Fields

		private static readonly IMemoryManagementStrategy _strategy;
		
		private static readonly object _syncLock = new object();
		private static readonly List<ILargeObjectContainer> _containersToAdd = new List<ILargeObjectContainer>();
		private static readonly List<ILargeObjectContainer> _containersToRemove = new List<ILargeObjectContainer>();

		private static readonly LargeObjectContainerCache _containerCache = new LargeObjectContainerCache();
		private static volatile Thread _collectionThread;
		private static volatile bool _collecting = false;
		private static int _waitingClients = 0;
		private static event EventHandler<MemoryCollectedEventArgs> _memoryCollected;

		#endregion

		static MemoryManager()
		{
			//_strategy = MemoryManagementStrategy.Null;
			//return;
			
			IMemoryManagementStrategy strategy = null;

			try
			{
				strategy = (IMemoryManagementStrategy)new MemoryManagementStrategyExtensionPoint().CreateExtension();
			}
			catch (NotSupportedException)
			{
				Platform.Log(LogLevel.Debug, "No memory management strategy extension found.");
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Debug, e, "Creation of memory management strategy failed; using default.");
			}

			_strategy = strategy ?? new DefaultMemoryManagementStrategy();
			_strategy.MemoryCollected += OnMemoryCollected;
		}

		#region Properties

		public static long LargeObjectBytesCount
		{
			get { return _containerCache.LastLargeObjectBytesCount; }	
		}

		public static long LargeObjectContainerCount
		{
			get { return _containerCache.LastLargeObjectContainerCount; }
		}

		public static long LargeObjectCount
		{
			get { return _containerCache.LastLargeObjectCount; }
		}

		#endregion

		public static event EventHandler<MemoryCollectedEventArgs> MemoryCollected
		{
			add
			{
				lock (_syncLock)
				{
					_memoryCollected += value;
				}
			}
			remove
			{
				lock (_syncLock)
				{
					_memoryCollected -= value;
				}
			}
		}

		#region Methods

		private static void OnMemoryCollected(object sender, MemoryCollectedEventArgs args)
		{
			Delegate[] delegates;

			lock (_syncLock)
			{
				if (args.IsLast)
					_collecting = false;

				if (_memoryCollected != null)
					delegates = _memoryCollected.GetInvocationList();
				else
					delegates = new Delegate[0];
			}

			foreach (Delegate @delegate in delegates)
			{
				try
				{
					@delegate.DynamicInvoke(_strategy, args);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e, "Unexpected error encountered during memory collection notification.");
				}
			}
		}

		private static void StartCollectionThread()
		{
			lock (_syncLock)
			{
				if (_collectionThread == null)
				{
					Platform.Log(LogLevel.Debug, "Starting memory collection thread.");

					_collectionThread = new Thread(RunCollectionThread)
					                    	{
					                    		Priority = ThreadPriority.BelowNormal, 
												IsBackground = true
					                    	};

					_collectionThread.Start();
				}
			}
		}

		private static void RunCollectionThread()
		{
			const int waitTimeMilliseconds = 10000;

			while (true)
			{
				try
				{
					lock (_syncLock)
					{
						if (_waitingClients == 0)
							Monitor.Wait(_syncLock, waitTimeMilliseconds);

						if (Platform.IsLogLevelEnabled(LogLevel.Debug))
						{
							Platform.Log(LogLevel.Debug, "Adding {0} containers and removing {1} from large object container cache.", 
							_containersToAdd.Count, _containersToRemove.Count);
						}

						foreach (ILargeObjectContainer container in _containersToRemove)
							_containerCache.Remove(container);
						foreach (ILargeObjectContainer container in _containersToAdd)
							_containerCache.Add(container);

						_containersToRemove.Clear();
						_containersToAdd.Clear();

						if (_waitingClients == 0 && _containerCache.IsEmpty)
						{
							Platform.Log(LogLevel.Debug, "Exiting collection thread, container cache is empty.");
							_collectionThread = null;
							break;
						}
					}

					CodeClock clock = new CodeClock();
					clock.Start();

					_containerCache.CleanupDeadItems(true);

					clock.Stop();
					PerformanceReportBroker.PublishReport("Memory", "CleanupDeadItems", clock.Seconds);

					_strategy.Collect(new MemoryCollectionArgs(_containerCache));
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e, "Unexpected error occurred while collecting large objects.");
				}
				finally
				{
					if (_collecting)
					{
						Platform.Log(LogLevel.Debug, "Memory management strategy failed to fire 'complete' event; firing to avoid deadlocks.");
						OnMemoryCollected(null, new MemoryCollectedEventArgs(0, 0, 0, TimeSpan.Zero, true));
					}
				}
			}
		}

		public static void Add(ILargeObjectContainer container)
		{
			lock(_syncLock)
			{
				_containersToRemove.Remove(container);

				if (!_containersToAdd.Contains(container))
					_containersToAdd.Add(container);

				StartCollectionThread();
			}
		}

		public static void Remove(ILargeObjectContainer container)
		{
			lock (_syncLock)
			{
				_containersToAdd.Remove(container);

				if (!_containersToRemove.Contains(container))
					_containersToRemove.Add(container);
			}
		}

		#region Collect

		public static void Collect()
		{
			Collect(false);
		}

		public static void Collect(bool wait)
		{
			if (wait)
				Collect(Timeout.Infinite);
			else
				Collect(0);
		}

		public static void Collect(int waitTimeoutMilliseconds)
		{
			Collect(TimeSpan.FromMilliseconds(waitTimeoutMilliseconds));
		}

		public static void Collect(TimeSpan waitTimeout)
		{
			CodeClock clock = new CodeClock();
			clock.Start();

			new MemoryCollector(waitTimeout).Collect();

			clock.Stop();
			if (Platform.IsLogLevelEnabled(LogLevel.Debug)) 
			{
				Platform.Log(LogLevel.Debug, 
					"Waited a total of {0} for memory to be collected; max wait time was {1} seconds.", clock, waitTimeout.TotalSeconds);
			}
		}

		#endregion

		#region Execute

		//TODO: unsure of how/where this method is being used - review.
		public static void Execute(RetryableCommand retryableCommand, int maxWaitTimeMilliseconds)
		{
			Execute(retryableCommand, TimeSpan.FromMilliseconds(maxWaitTimeMilliseconds));
		}

		public static void Execute(RetryableCommand retryableCommand, TimeSpan maxWaitTime)
		{
			new RetryableCommandExecutor(retryableCommand, maxWaitTime).Execute();
		}


		#endregion

		public static T[] Allocate<T>(int count, int maxWaitTimeMilliseconds)
		{
			return Allocate<T>(count, TimeSpan.FromMilliseconds(maxWaitTimeMilliseconds));
		}

		//TODO: unsure of how/where this method is being used - review.
		public static T[] Allocate<T>(int count, TimeSpan maxWaitTime)
		{
			T[] returnValue = null;
			Execute(delegate { returnValue = new T[count]; }, maxWaitTime);

			if (returnValue == null)
				returnValue = new T[count];

			return returnValue;
		}

		#endregion
	}
}
