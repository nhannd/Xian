using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Memory and storage size units
	/// </summary>
	public enum SizeUnits
	{
		/// <summary>
		/// Bytes
		/// </summary>
		Bytes,
		/// <summary>
		/// Kilobytes
		/// </summary>
		Kilobytes,
		/// <summary>
		/// Megabytes
		/// </summary>
		Megabytes,
		/// <summary>
		/// Gigabytes
		/// </summary>
		Gigabytes
	}

	/// <summary>
	/// Provides convenience methods for querying system resources.
	/// </summary>
	public static class SystemResources
	{
		private static volatile PerformanceCounter _memoryPerformanceCounter;
		private static readonly object _syncRoot = new object();

		private static PerformanceCounter MemoryPerformanceCounter
		{
			get
			{
				if (_memoryPerformanceCounter == null)
				{
					lock (_syncRoot)
					{
						if (_memoryPerformanceCounter == null)
							_memoryPerformanceCounter = new PerformanceCounter("Memory", "Available Bytes");
					}
				}

				return _memoryPerformanceCounter;
			}
		}

		/// <summary>
		/// Gets the available physical memory.
		/// </summary>
		/// <param name="units"></param>
		/// <returns></returns>
		public static long GetAvailableMemory(SizeUnits units)
		{
			long availableBytes = Convert.ToInt64(MemoryPerformanceCounter.NextValue());

			if (units == SizeUnits.Bytes)
				return availableBytes;
			else if (units == SizeUnits.Kilobytes)
				return availableBytes / 1024;
			else if (units == SizeUnits.Megabytes)
				return availableBytes / 1048576;
			else
				return availableBytes / 1073741824;
		}
	}
}
