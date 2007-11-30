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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Timers;

namespace ClearCanvas.Common.Utilities
{
	// TODO (Stewart): change this so it doesn't use a timer internally, but just uses a thread pool thread.

	/// <summary>
	/// A delegate for use by a <see cref="Timer"/> object.
	/// </summary>
	public delegate void TimerDelegate(object state);

	/// <summary>
	/// Implements a simple timer class that handles marshalling delegates back to the thread on which
	/// this object was allocated (usually the main UI thread).
	/// </summary>
	/// <remarks>
	/// This class <B>must</B> be instantiated from within a UI thread, otherwise an exception
	/// could be thrown upon construction (unless the thread has a custom <see cref="SynchronizationContext"/>).  
	/// This class relies on <see cref="SynchronizationContext.Current"/> being non-null in order to do the marshalling.
	/// </remarks>
	public sealed class Timer : IDisposable
	{
		private TimerDelegate _elapsedDelegate;
		private object _stateObject;
		private System.Timers.Timer _internalTimer;
		private SynchronizationContext _synchronizationContext;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="elapsedDelegate">The delegate to execute on a timer.</param>
		public Timer(TimerDelegate elapsedDelegate)
			: this(elapsedDelegate, null)
		{ 
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="elapsedDelegate">The delegate to execute on a timer.</param>
		/// <param name="stateObject">A user defined state object.</param>
		public Timer(TimerDelegate elapsedDelegate, object stateObject)
		{
			Platform.CheckForNullReference(SynchronizationContext.Current, "SynchronizationContext.Current");
			Platform.CheckForNullReference(elapsedDelegate, "elapsedDelegate");
			
			_synchronizationContext = SynchronizationContext.Current;

			_elapsedDelegate = elapsedDelegate;
			_stateObject = stateObject;
			_internalTimer = new System.Timers.Timer();
			_internalTimer.Elapsed += new ElapsedEventHandler(OnInternalTimerElapsed);
		}

		/// <summary>
		/// Enables/Disables the timer.
		/// </summary>
		public bool Enabled
		{
			get { return _internalTimer.Enabled; }
			set { _internalTimer.Enabled = false; }
		}

		/// <summary>
		/// Sets the timer interval in milliseconds.
		/// </summary>
		public double Interval
		{
			get { return _internalTimer.Interval; }
			set { _internalTimer.Interval = value; }
		}

		/// <summary>
		/// Starts the timer.
		/// </summary>
		public void Start()
		{
			_internalTimer.Start();
		}

		/// <summary>
		/// Stops the timer.
		/// </summary>
		public void Stop()
		{
			_internalTimer.Stop();
		}

		#region IDisposable Members

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		public void  Dispose()
		{
			try
			{
				if (_internalTimer != null)
				{
					_internalTimer.Dispose();
					_internalTimer = null;
				}

				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{

				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion

		private void OnInternalTimerElapsed(object sender, ElapsedEventArgs e)
		{
			if (_internalTimer != null)
				_synchronizationContext.Post(delegate(object o) { _elapsedDelegate(_stateObject); }, null);
		}
	}
}
