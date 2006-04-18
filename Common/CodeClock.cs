using System;

namespace ClearCanvas.Common
{
	/// <summary>
	/// A simple stopwatch class that can be used to profile code.  To ensure portability, use this
    /// class instead of the <see cref="System.Diagnostics.Stopwatch" /> class which has not yet been
    /// implemented in Mono.
	/// </summary>
	/// <remarks>
	/// On Windows, this class will internally use the Win32 high resolution performance counter.
	/// On other platforms, a default portable clock is used.
	/// </remarks>
	/// <example>
	/// <code>
    /// CodeClock clock = new CodeClock();
    /// clock.Start();
	///
	/// // Code to be timed
	///
    /// clock.Stop();
    /// Trace.Write(clock.ToString());
	/// </code>
	/// </example>
	public class CodeClock
	{
		long elapsedCount = 0;
		long startCount = 0;
		
		private IPerformanceCounter _clock;

		/// <summary>
        /// Initializes a new instance of the <see cref="CodeClock"/> class.
		/// </summary>
		public CodeClock()
		{
			if(Platform.IsWin32Platform)
			{
				_clock = new Win32PerformanceCounter();
			}
			else
			{
				_clock = new DefaultPerformanceCounter();
			}
		}

		/// <summary>
		/// Starts the clock
		/// </summary>
		public void Start()
		{
			startCount = _clock.Count;
		}
		

		/// <summary>
		/// Stops the clock
		/// </summary>
		public void Stop()
		{
			long stopCount = _clock.Count;
			elapsedCount += (stopCount - startCount);
		}

		/// <summary>
		/// Clears (resets) the clock
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
