using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClearCanvas.Common;
using System.Threading;
using ClearCanvas.Common.Utilities;
using System.Collections;

namespace ClearCanvas.ImageViewer.Common
{
	public class MemoryCollectedEventArgs : EventArgs
	{
		public MemoryCollectedEventArgs(int collectionNumber, int totalLargeObjectsCollected, long totalBytesCollected, TimeSpan elapsedTime, bool isComplete)
		{
			CollectionNumber = collectionNumber;
			this.ElapsedTime = elapsedTime;
			this.TotalLargeObjectsCollected = totalLargeObjectsCollected;
			this.TotalBytesCollected = totalBytesCollected;
			this.IsComplete = isComplete;
		}

		public readonly int CollectionNumber;
		public readonly TimeSpan ElapsedTime;
		public readonly int TotalLargeObjectsCollected;
		public readonly long TotalBytesCollected;
		public readonly bool IsComplete;
	}

	public interface IMemoryManagementStrategy
	{
		void Add(ILargeObjectContainer container);
		void Remove(ILargeObjectContainer container);
		int Count { get; }
		
		void Collect();
		event EventHandler<MemoryCollectedEventArgs> MemoryCollected;
	}

	internal class NullMemoryManagementStrategy : IMemoryManagementStrategy
	{
		#region IMemoryManagementStrategy Members

		public void Add(ILargeObjectContainer container)
		{
		}

		public void Remove(ILargeObjectContainer container)
		{
		}

		public int Count { get { return 0; } }

		public void Collect()
		{
		}

		public event EventHandler<MemoryCollectedEventArgs> MemoryCollected
		{
			add {}
			remove {}
		}

		#endregion
	}

	public abstract class MemoryManagementStrategy : IMemoryManagementStrategy
	{
		private event EventHandler<MemoryCollectedEventArgs> _memoryCollected;

		internal static readonly IMemoryManagementStrategy Null = new NullMemoryManagementStrategy();
		
		protected MemoryManagementStrategy()
		{
		}

		#region IMemoryManagementStrategy Members

		public abstract void Add(ILargeObjectContainer container);

		public abstract void Remove(ILargeObjectContainer container);

		public abstract int Count { get; }
		
		public abstract void Collect();

		public event EventHandler<MemoryCollectedEventArgs> MemoryCollected
		{
			add { _memoryCollected += value; }
			remove { _memoryCollected -= value; }
		}

		#endregion

		protected void OnMemoryCollected(MemoryCollectedEventArgs args)
		{
			ThreadPool.QueueUserWorkItem(NotifyMemoryCollected, args);
		}

		private void NotifyMemoryCollected(object memoryCollectedEventArgs)
		{
			EventsHelper.Fire(_memoryCollected, this, (MemoryCollectedEventArgs)memoryCollectedEventArgs);
		}
	}

	internal class DefaultMemoryManagementStrategy : MemoryManagementStrategy
	{
		private class Item
		{
			private WeakReference _reference;

			public Item(ILargeObjectContainer container)
			{
				_reference = new WeakReference(container);
			}

			public ILargeObjectContainer LargeObjectContainer
			{
				get
				{
					try
					{
						ILargeObjectContainer container = _reference.Target as ILargeObjectContainer;
						if (container == null)
							_reference = null;

						return container;
					}
					catch(InvalidOperationException)
					{
						_reference = null;
						return null;
					}
				}
			}
		}

		private readonly Dictionary<string, Item> _largeObjectContainers;

		private IEnumerator<KeyValuePair<string, Item>> _largeObjectEnumerator;
		private bool _enumerationComplete;

		private DateTime _lastCollectionTime;
		private Queue<Item> _collectionCandidates;

		public DefaultMemoryManagementStrategy()
		{
			_largeObjectContainers = new Dictionary<string, Item>();
			_lastCollectionTime = Platform.Time;
		}

		#region IMemoryManagementStrategy Members

		public override void Add(ILargeObjectContainer container)
		{
			string identifier = container.Identifier;
			Item item;
			if (!_largeObjectContainers.TryGetValue(identifier, out item))
			{
				item = new Item(container);
				_largeObjectEnumerator = null;
				_largeObjectContainers.Add(identifier, item);
			}
			else
			{
				ILargeObjectContainer existing = item.LargeObjectContainer;
				if (existing == null || !ReferenceEquals(container, existing))
				{
					_largeObjectEnumerator = null;
					_largeObjectContainers[identifier] = new Item(container);
				}
			}
		}

		public override void Remove(ILargeObjectContainer container)
		{
			_largeObjectEnumerator = null;
			_largeObjectContainers.Remove(container.Identifier);
		}

		public override int Count
		{
			get { return _largeObjectContainers.Count; }
		}

		public override void Collect()
		{
			try
			{
				DoCollect();
				CleanupDeadItems();
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Memory management strategy failed to collect.");
			}
		}

		#endregion

		private void AddCollectionCandidates(int numberToAdd)
		{
			if (_collectionCandidates == null)
				_collectionCandidates = new Queue<Item>();

			if (!_enumerationComplete)
			{
				int i = 0;
				if (_largeObjectEnumerator == null)
					_largeObjectEnumerator = _largeObjectContainers.GetEnumerator();

				while (i++ < numberToAdd && !(_enumerationComplete = !_largeObjectEnumerator.MoveNext()))
				{
					Item item = _largeObjectEnumerator.Current.Value;
					if (item != null)
						_collectionCandidates.Enqueue(item);
				}
			}

			if (_enumerationComplete)
				_largeObjectEnumerator = null;
		}

		private void CleanupDeadItems()
		{
			List<string> keysToRemove = new List<string>();
			_largeObjectEnumerator = null;

			foreach (KeyValuePair<string, Item> item in _largeObjectContainers)
			{
				if (item.Value.LargeObjectContainer == null)
					keysToRemove.Add(item.Key);
			}

			foreach (string keyToRemove in keysToRemove)
				_largeObjectContainers.Remove(keyToRemove);

			_largeObjectEnumerator = null;
		}

		private long EstimateTotalLargeObjectBytes()
		{
			long largeObjectBytes = 0;
			bool alreadyLogged = false;
			foreach (KeyValuePair<string, Item> item in _largeObjectContainers)
			{
				ILargeObjectContainer container = item.Value.LargeObjectContainer;
				if (container != null)
				{
					try
					{
						largeObjectBytes += container.TotalBytesHeld;
					}
					catch(Exception e)
					{
						if (!alreadyLogged)
						{
							alreadyLogged = true;
							Platform.Log(LogLevel.Debug, e, "Unexpected error while estimating large object bytes.");
						}
					}
				}
			}

			return largeObjectBytes;
		}

		private void DoCollect()
		{
			_enumerationComplete = false;

			const long OneGigabyte = 1000 * 1024 * 1024;
			const long FiveHundredMB = 500 * 1024 * 1024;

			long estimatedLargeObjectBytes = Process.GetCurrentProcess().VirtualMemorySize64;
			long highWatermark = FiveHundredMB;
			if (estimatedLargeObjectBytes < highWatermark)
				return;

			long lowWatermark = highWatermark / 2;
			long bytesToCollect = highWatermark - lowWatermark;

			long totalBytesCollected = 0;
			int totalLargeObjectsCollected = 0;
			RegenerationCost maxCost = RegenerationCost.Low;
			long startTicks = Platform.Time.Ticks;

			int i = 0;
			while (maxCost <= RegenerationCost.High)
			{
				DateTime currentTime = Platform.Time;
				DateTime lastCollectionTime = _lastCollectionTime;
				TimeSpan timeSinceLastCollection = currentTime - lastCollectionTime;
				TimeSpan maxTimeSinceLastCollection = TimeSpan.FromMilliseconds(timeSinceLastCollection.TotalMilliseconds / 3);

				while (maxTimeSinceLastCollection < timeSinceLastCollection)
				{
					int batchSize = Math.Min(100, Count);

					while (!_enumerationComplete)
					{
						CodeClock clock = new CodeClock();
						clock.Start();

						AddCollectionCandidates(batchSize);

						long bytesCollected;
						int largeObjectsCollected;
						Collect(maxCost, maxTimeSinceLastCollection, out bytesCollected, out largeObjectsCollected);
						totalBytesCollected += bytesCollected;
						totalLargeObjectsCollected += largeObjectsCollected;

						if (totalBytesCollected > 0)
							GC.Collect();

						clock.Stop();

						TimeSpan elapsed = TimeSpan.FromTicks(Platform.Time.Ticks - startTicks);

						MemoryCollectedEventArgs args = new MemoryCollectedEventArgs(i + 1,
							largeObjectsCollected, bytesCollected, elapsed, false);

						Platform.Log(LogLevel.Info, "Large object collection #{0}: freed {1} MB in {2}",
							args.CollectionNumber, args.TotalBytesCollected / 1024F / 1024F, args.ElapsedTime);

						base.OnMemoryCollected(args);

						if (totalBytesCollected > bytesToCollect)
							break;

						++i;
						batchSize *= 5;
					}

					if (totalBytesCollected > bytesToCollect)
						break;

					maxTimeSinceLastCollection = maxTimeSinceLastCollection + maxTimeSinceLastCollection;
				}

				if (totalBytesCollected > bytesToCollect)
					break;

				++maxCost;
			}

			GC.Collect();

			TimeSpan totalElapsed = TimeSpan.FromTicks(Platform.Time.Ticks - startTicks);

			MemoryCollectedEventArgs finalArgs = new MemoryCollectedEventArgs(i + 1,
				totalLargeObjectsCollected, totalBytesCollected, totalElapsed, true);

			Platform.Log(LogLevel.Info, "Large object collection summary: freed {0} MB in {1} in {2} iterations",
				finalArgs.TotalBytesCollected / 1024F / 1024F, finalArgs.ElapsedTime, finalArgs.CollectionNumber);
			
			OnMemoryCollected(finalArgs);

			_collectionCandidates = null;
			_lastCollectionTime = Platform.Time;
		}

		private void Collect(RegenerationCost maxCost, TimeSpan maxTimeSinceLastAccess, out long bytesCollected, out int largeObjectsCollected)
		{
			bytesCollected = 0;
			largeObjectsCollected = 0;

			while(_collectionCandidates.Count > 0)
			{
				try
				{
					Item item = _collectionCandidates.Dequeue();
					ILargeObjectContainer container = item.LargeObjectContainer;
					if (container != null && !container.IsLocked)
					{
						if (container.RegenerationCost <= maxCost)
						{
							TimeSpan timeSinceLastAccess = Platform.Time - container.LastAccessTime;
							if (timeSinceLastAccess > maxTimeSinceLastAccess)
							{
								long totalBytesHeldBefore = container.TotalBytesHeld;
								int largeObjectsBefore = container.LargeObjectCount;

								container.Unload();
								
								long totalBytesHeldAfter = container.TotalBytesHeld;
								int largeObjectsAfter = container.LargeObjectCount;

								largeObjectsCollected += (largeObjectsBefore - largeObjectsAfter);

								bytesCollected += (totalBytesHeldBefore - totalBytesHeldAfter);
							}
						}
					}
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Debug, e, "Failed to unload large object data.");
				}
			}
		}
	}
}
