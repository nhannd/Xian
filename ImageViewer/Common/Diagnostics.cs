using System;
using System.Threading;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common
{
	public static class Diagnostics
	{
		#region Memory

		private static long _totalLargeObjectMemoryBytes = 0;

		private static readonly object _syncLock = new object();
		private static event EventHandler _totalLargeObjectBytesChanged;

		public static long TotalLargeObjectBytes
		{
			get
			{
				//synchronize changes, but not reads
				return Thread.VolatileRead(ref _totalLargeObjectMemoryBytes);
			}	
		}

		public static event EventHandler TotalLargeObjectBytesChanged
		{
			add
			{
				lock(_syncLock)
				{
					_totalLargeObjectBytesChanged += value;
				}
			}
			remove
			{
				lock (_syncLock)
				{
					_totalLargeObjectBytesChanged -= value;
				}
			}
		}

		public static void OnLargeObjectAllocated(long bytes)
		{
			lock(_syncLock)
			{
				//synchronize changes, but not reads
				_totalLargeObjectMemoryBytes += bytes;
				EventsHelper.Fire(_totalLargeObjectBytesChanged, null, EventArgs.Empty);
			}
		}

		public static void OnLargeObjectReleased(long bytes)
		{
			lock (_syncLock)
			{
				//synchronize changes, but not reads
				_totalLargeObjectMemoryBytes -= bytes;
				EventsHelper.Fire(_totalLargeObjectBytesChanged, null, EventArgs.Empty);
			}
		}

		#endregion
	}
}
