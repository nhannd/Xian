using System;
using System.Threading;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common
{
	public static class MemoryHelper
	{
		public static long _totalLargeObjectMemoryBytes = 0;

		private static readonly object _syncLock = new object();
		private static event EventHandler _totalLargeObjectMemoryChanged;

		public static long TotalLargeObjectMemoryBytes
		{
			get
			{
				//synchronize changes, but not reads
				return Thread.VolatileRead(ref _totalLargeObjectMemoryBytes);
			}	
		}

		public static event EventHandler TotalLargeObjectMemoryChanged
		{
			add
			{
				lock(_syncLock)
				{
					_totalLargeObjectMemoryChanged += value;
				}
			}
			remove
			{
				lock (_syncLock)
				{
					_totalLargeObjectMemoryChanged -= value;
				}
			}
		}

		public static void OnLargeObjectAllocated(long bytes)
		{
			//Anything less than this doesn't go on the LOH anyway.
			if (bytes < 85000)
				return;

			lock(_syncLock)
			{
				//synchronize changes, but not reads
				_totalLargeObjectMemoryBytes += bytes;
				EventsHelper.Fire(_totalLargeObjectMemoryChanged, null, EventArgs.Empty);
			}
		}

		public static void OnLargeObjectReleased(long bytes)
		{
			//Anything less than this doesn't go on the LOH anyway.
			if (bytes < 85000)
				return;

			lock (_syncLock)
			{
				//synchronize changes, but not reads
				_totalLargeObjectMemoryBytes -= bytes;
				EventsHelper.Fire(_totalLargeObjectMemoryChanged, null, EventArgs.Empty);
			}
		}
	}
}
