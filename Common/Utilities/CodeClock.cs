#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;

namespace ClearCanvas.Common.Utilities
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
			return String.Format(SR.FormatSeconds, Seconds);
		}
	}
}
