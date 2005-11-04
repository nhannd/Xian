using System;

namespace ClearCanvas.Common
{
	/// <summary>
	/// A simple high resolution stopwatch class that can be used to profile code.
	/// </summary>
	/// <remarks>
	/// A convenient stopwatch wrapping of the Win32 high resolution performance counter
	/// that can be used to profile code.  Taken from an MSDN article.
	/// </remarks>
	/// <example>
	/// <code>
	/// Counter counter = new Counter();
	/// counter.Start();
	/// 
	/// // Code to be timed
	/// 
	/// counter.Stop();
	/// Trace.Write(counter.ToString());
	/// </code>
	/// </example>
	public class Counter 
	{
		long elapsedCount = 0;
		long startCount = 0;

		/// <summary>
		/// Initializes a new instance of the <b>Counter</b> class.
		/// </summary>
		public Counter()
		{
		}

		/// <summary>
		/// Starts the stopwatch
		/// </summary>
		public void Start()
		{
			startCount = 0;
			QueryPerformanceCounter(ref startCount);
		}

		/// <summary>
		/// Stops the stopwatch
		/// </summary>
		public void Stop()
		{
			long stopCount = 0;
			QueryPerformanceCounter(ref stopCount);

			elapsedCount += (stopCount - startCount);
		}

		/// <summary>
		/// Clears (resets) the stopwatch
		/// </summary>
		public void Clear()
		{
			elapsedCount = 0;
		}

		/// <summary>
		/// Number of seconds elapsed between start and stop.
		/// </summary>
		public float Seconds
		{
			get
			{
				long freq = 0;
				QueryPerformanceFrequency(ref freq);
				return((float) elapsedCount / (float) freq);
			}
		}

		/// <summary>
		/// Number of seconds elapsed between start and stop.
		/// </summary>
		/// <returns>Formatted string containing number of seconds elapsed.</returns>
		public override string ToString()
		{
			return String.Format("{0} seconds", Seconds);
		}

		static long Frequency 
		{
			get 
			{
				long freq = 0;
				QueryPerformanceFrequency(ref freq);
				return freq;
			}
		}

		static long Value 
		{
			get 
			{
				long count = 0;
				QueryPerformanceCounter(ref count);
				return count;
			}
		}

		[System.Runtime.InteropServices.DllImport("KERNEL32")]
		private static extern bool QueryPerformanceCounter(  ref long lpPerformanceCount);

		[System.Runtime.InteropServices.DllImport("KERNEL32")]
		private static extern bool QueryPerformanceFrequency( ref long lpFrequency);                     
	}
}
