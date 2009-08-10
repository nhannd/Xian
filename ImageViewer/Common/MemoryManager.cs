using System;
using ClearCanvas.Common;
using System.Threading;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Common
{
	public sealed class MemoryManagementStrategyExtensionPoint : ExtensionPoint<IMemoryManagementStrategy>
	{ }

	public static class MemoryManager
	{
		public delegate void RetryableCommand();

		private static readonly IMemoryManagementStrategy _strategy;
		
		private static readonly object _syncLock = new object();
		private static readonly List<ILargeObjectContainer> _containersToAdd = new List<ILargeObjectContainer>();
		private static readonly List<ILargeObjectContainer> _containersToRemove = new List<ILargeObjectContainer>();

		private static volatile Thread _collectionThread;
		private static volatile bool _collecting = false;
		private static int _waitingClients = 0;
		private static event EventHandler<MemoryCollectedEventArgs> _memoryCollected;

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
				Platform.Log(LogLevel.Warn, e, "Creation of memory management strategy failed; using default.");
			}

			_strategy = strategy ?? new DefaultMemoryManagementStrategy();
			_strategy.MemoryCollected += OnMemoryCollected;
		}

		#region Methods

		static void OnMemoryCollected(object sender, MemoryCollectedEventArgs args)
		{
			Delegate[] delegates;

			lock (_syncLock)
			{
				if (args.IsComplete)
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
					Platform.Log(LogLevel.Debug, e, "Error encountered during memory collection notification.");
				}
			}
		}

		private static void StartCollectionThread()
		{
			lock (_syncLock)
			{
				if (_collectionThread == null)
				{
					_collectionThread = new Thread(RunCollectionThread);
					_collectionThread.Priority = ThreadPriority.BelowNormal;
					_collectionThread.IsBackground = true;
					_collectionThread.Start();
				}
			}
		}

		private static void RunCollectionThread()
		{
			const int waitTimeMilliseconds = 5000;

			while (true)
			{
				try
				{
					lock (_syncLock)
					{
						if (_waitingClients == 0)
							Monitor.Wait(_syncLock, waitTimeMilliseconds);

						foreach (ILargeObjectContainer container in _containersToAdd)
							_strategy.Add(container);
						foreach (ILargeObjectContainer container in _containersToRemove)
							_strategy.Remove(container);

						_containersToRemove.Clear();
						_containersToAdd.Clear();

						if (_strategy.Count == 0)
						{
							_collectionThread = null;
							break;
						}
					}

					_strategy.Collect();
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Debug, e, "Unexpected failure occurred while collecting large objects.");
				}
				finally
				{
					if (_collecting)
					{
						Platform.Log(LogLevel.Debug, "Memory management strategy failed to fire 'complete' event; firing to avoid infinite loops.");
						OnMemoryCollected(null, new MemoryCollectedEventArgs(0, 0, 0, TimeSpan.FromTicks(0), true));
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
				if (!_containersToRemove.Contains(container))
					_containersToRemove.Add(container);

				_containersToAdd.Remove(container);
			}
		}

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

		public static void Collect(TimeSpan waitTimeout)
		{
			Collect((int)waitTimeout.TotalMilliseconds);
		}

		public static void Collect(int waitTimeMilliseconds)
		{
			object waitObject = new object();

			EventHandler<MemoryCollectedEventArgs> collected =
				delegate(object sender, MemoryCollectedEventArgs args)
					{
						if (args.IsComplete)
						{
							lock (waitObject)
							{
								Monitor.Pulse(waitObject);
							}
						}
					};

			lock (waitObject)
			{
				lock (_syncLock)
				{
					++_waitingClients;
					MemoryCollected += collected;
					Monitor.Pulse(_syncLock);
				}

				Monitor.Wait(waitObject, waitTimeMilliseconds);

				lock (_syncLock)
				{
					--_waitingClients;
					MemoryCollected -= collected;
				}
			}
		}

		public static void Execute(RetryableCommand retryableCommand, int maxWaitTimeMilliseconds)
		{
			Execute(retryableCommand, TimeSpan.FromMilliseconds(maxWaitTimeMilliseconds));
		}

		public static void Execute(RetryableCommand retryableCommand, TimeSpan maxWaitTime)
		{
			Platform.CheckForNullReference(retryableCommand, "retryableCommand");
			Platform.CheckNonNegative((int) maxWaitTime.TotalMilliseconds, "maxWaitTime");

			try
			{
				retryableCommand();
			}
			catch (OutOfMemoryException)
			{
				object waitObject = new object();
				EventHandler<MemoryCollectedEventArgs> collected = delegate
						{
							lock (waitObject)
							{
								Monitor.Pulse(waitObject);
							}
						};

				long begin = Platform.Time.Ticks;
				TimeSpan waitTimeRemaining = maxWaitTime;

				lock (waitObject)
				{
					lock (_syncLock)
					{
						++_waitingClients;
						MemoryCollected += collected;
						Monitor.Pulse(_syncLock);
					}

					do
					{
						Monitor.Wait(waitObject, waitTimeRemaining);

						try
						{
							retryableCommand();
							break;
						}
						catch (OutOfMemoryException)
						{
							TimeSpan elapsed = TimeSpan.FromTicks(Platform.Time.Ticks - begin);
							waitTimeRemaining = maxWaitTime - elapsed;
							if (waitTimeRemaining.TotalMilliseconds <= 0)
								throw;
						}
					} while (true);

					lock (_syncLock)
					{
						--_waitingClients;
						MemoryCollected -= collected;
					}
				}
			}
		}

		public static T[] Allocate<T>(int count)
		{
			return Allocate<T>(count, 0, 0);
		}

		public static T[] Allocate<T>(int count, int maxRetries, int maxWaitTimeMilliseconds)
		{
			T[] returnValue = null;
			Execute(delegate { returnValue = new T[count]; }, maxWaitTimeMilliseconds);

			if (returnValue == null)
				returnValue = new T[count];

			return returnValue;
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
	}
}
