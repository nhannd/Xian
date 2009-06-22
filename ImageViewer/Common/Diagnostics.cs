using System;
using System.Threading;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common
{
	/// <summary>
	/// Static helper class for use when debugging.
	/// </summary>
	public static class Diagnostics
	{
		#region Memory

		private static long _totalLargeObjectMemoryBytes = 0;

		private static readonly object _syncLock = new object();
		private static event EventHandler _totalLargeObjectBytesChanged;

		/// <summary>
		/// Gets the running total byte count of large objects held in memory.
		/// </summary>
		public static long TotalLargeObjectBytes
		{
			get
			{
				//synchronize changes, but not reads
				return Thread.VolatileRead(ref _totalLargeObjectMemoryBytes);
			}	
		}

		/// <summary>
		/// Occurs when <see cref="TotalLargeObjectBytes"/> has changed.
		/// </summary>
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

		/// <summary>
		/// Called when a large object is allocated.
		/// </summary>
		/// <remarks>
		/// Although it is not necessary to call this method when you allocate a large object,
		/// such as a byte array for pixel data, it is recommended that you do so in order
		/// for this class to provide accurate data.
		/// </remarks>
		public static void OnLargeObjectAllocated(long bytes)
		{
			lock(_syncLock)
			{
				//synchronize changes, but not reads
				_totalLargeObjectMemoryBytes += bytes;
				EventsHelper.Fire(_totalLargeObjectBytesChanged, null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Called when a large object is released.
		/// </summary>
		/// <remarks>
		/// You should call this method exactly once when you are certain the large object
		/// in question is no longer in use.
		/// </remarks>
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
