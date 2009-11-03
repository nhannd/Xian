using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common
{
	public class MemoryCollectedEventArgs : EventArgs
	{
		public MemoryCollectedEventArgs(int largeObjectContainersUnloadedCount,
			int largeObjectsCollectedCount, long bytesCollectedCount, TimeSpan elapsedTime, bool isLast)
		{
			ElapsedTime = elapsedTime;
			LargeObjectContainersUnloadedCount = largeObjectContainersUnloadedCount;
			LargeObjectsCollectedCount = largeObjectsCollectedCount;
			BytesCollectedCount = bytesCollectedCount;
			IsLast = isLast;
		}

		public readonly TimeSpan ElapsedTime;
		public readonly int LargeObjectContainersUnloadedCount;
		public readonly int LargeObjectsCollectedCount;
		public readonly long BytesCollectedCount;
		public readonly bool IsLast;

		//TODO (cr Oct 2009): guard against it being set back to false when already set to true?
		public bool NeedMoreMemory = false;
	}

	public class MemoryCollectionArgs
	{
		internal MemoryCollectionArgs(IEnumerable<ILargeObjectContainer> largeObjectContainers)
		{
			LargeObjectContainers = largeObjectContainers;
		}

		public readonly IEnumerable<ILargeObjectContainer> LargeObjectContainers;
	}

	public interface IMemoryManagementStrategy
	{
		void Collect(MemoryCollectionArgs collectionArgs);
		event EventHandler<MemoryCollectedEventArgs> MemoryCollected;
	}

	public abstract class MemoryManagementStrategy : IMemoryManagementStrategy
	{
		private class NullMemoryManagementStrategy : IMemoryManagementStrategy
		{
			#region IMemoryManagementStrategy Members

			public void Collect(MemoryCollectionArgs collectionArgs)
			{
			}

			public event EventHandler<MemoryCollectedEventArgs> MemoryCollected
			{
				add { }
				remove { }
			}

			#endregion
		}

		internal static readonly IMemoryManagementStrategy Null = new NullMemoryManagementStrategy();

		private event EventHandler<MemoryCollectedEventArgs> _memoryCollected;
		
		protected MemoryManagementStrategy()
		{
		}

		#region IMemoryManagementStrategy Members

		public abstract void Collect(MemoryCollectionArgs collectionArgs);

		public event EventHandler<MemoryCollectedEventArgs> MemoryCollected
		{
			add { _memoryCollected += value; }
			remove { _memoryCollected -= value; }
		}

		#endregion

		protected void OnMemoryCollected(MemoryCollectedEventArgs args)
		{
			try
			{
				EventsHelper.Fire(_memoryCollected, this, args);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Unexpected failure while firing memory collected event.");
			}
		}
	}
}
