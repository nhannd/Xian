using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Timers;

namespace ClearCanvas.Common.Utilities
{
	public delegate void TimerDelegate(object state);

	/// <summary>
	/// Implements a simple timer class that handles marshalling delegates back to a UI Thread (usually the main thread).
	/// </summary>
	/// <remarks>
	/// This class <B>must</B> be instantiated from within a UI Thread, otherwise an exception
	/// could be thrown upon construction (unless the thread has a custom <see cref="SynchronizationContext"/>.  
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
		/// <param name="elapsedDelegate">the delegate to execute on a timer.</param>
		public Timer(TimerDelegate elapsedDelegate)
			: this(elapsedDelegate, null)
		{ 
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="elapsedDelegate">the delegate to execute on a timer.</param>
		/// <param name="stateObject">a user defined state object.</param>
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
		/// Timer disposal.
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

				Platform.Log(e);
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
