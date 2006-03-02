using System;

namespace ClearCanvas.Common
{
	/// <summary>
	/// A simple high resolution stopwatch class that can be used to profile code.
	/// </summary>
	/// <remarks>
	/// On Windows, this class will internally use the Win32 high resolution performance counter.
	/// On other platforms, a default portable clock is used.
	///
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
		
		private IPerformanceClock _clock;

		/// <summary>
		/// Initializes a new instance of the <see cref="Counter"/> class.
		/// </summary>
		public Counter()
		{
			if(Platform.IsWin32Platform)
			{
				_clock = new Win32Clock();
			}
			else
			{
				_clock = new DefaultClock();
			}
		}

		/// <summary>
		/// Starts the stopwatch
		/// </summary>
		public void Start()
		{
			startCount = _clock.Count;
		}
		

		/// <summary>
		/// Stops the stopwatch
		/// </summary>
		public void Stop()
		{
			long stopCount = _clock.Count;
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
		/// Gets the number of seconds elapsed between start and stop.
		/// </summary>
		/// <value>The number of seconds elapsed between start and stop.</value>
		public float Seconds
		{
			get
			{
				return((float) elapsedCount / (float) _clock.Frequency);
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
	}
}
