using System;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Uses a <see cref="Timer"/> internally to delay-publish an event.
	/// </summary>
	/// <remarks>
	/// This class <B>must</B> be instantiated from within a UI thread; see <see cref="Timer"/> for more details.
	/// </remarks>
	/// <seealso cref="Timer"/>
	public class DelayedEventPublisher : IDisposable
	{
		private Timer _timer;
		private readonly EventHandler _trueEventHandler;

		private readonly int _timeoutMilliseconds;

		private DateTime _lastPublishTime;
		private object _lastSender;
		private EventArgs _lastArgs;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="trueEventHandler">The 'true' event handler; calls to which are delayed until 
		/// the default timeout of 350 milliseconds has elapsed with no calls to <see cref="Publish(object, EventArgs)"/>.</param>
		public DelayedEventPublisher(EventHandler trueEventHandler)
			: this(trueEventHandler, 350)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="trueEventHandler">The 'true' event handler; calls to which are delayed until 
		/// <paramref name="timeoutMilliseconds"/> has elapsed with no calls to <see cref="Publish(object, EventArgs)"/>.</param>
		/// <param name="timeoutMilliseconds">The time after which, if <see cref="Publish(object, EventArgs)"/> has not been called, 
		/// to publish the delayed event via <paramref name="trueEventHandler"/>.</param>
		public DelayedEventPublisher(EventHandler trueEventHandler, int timeoutMilliseconds)
		{
			Platform.CheckForNullReference(trueEventHandler, "trueEventHandler");
			Platform.CheckPositive(timeoutMilliseconds, "timeoutMilliseconds");

			_trueEventHandler = trueEventHandler;
			_timeoutMilliseconds = Math.Max(10, timeoutMilliseconds);

			_timer = new Timer(OnTimer);
			_timer.IntervalMilliseconds = 10;
		}

		private void OnTimer(object nothing)
		{
			if (!_timer.Enabled)
				return;

			if (Platform.Time.Subtract(_lastPublishTime).TotalMilliseconds >= _timeoutMilliseconds)
				Publish();
		}

		private void Publish()
		{
			_timer.Stop();

			EventsHelper.Fire(_trueEventHandler, _lastSender, _lastArgs);
			_lastSender = null;
			_lastArgs = null;
		}

		/// <summary>
		/// Cancels the currently pending delay-published event, if one exists.
		/// </summary>
		public void Cancel()
		{
			_timer.Stop();
			_lastSender = null;
			_lastArgs = null;
		}

		/// <summary>
		/// Called to immediately publish the currently pending
		/// delay-published event; the method does nothing if there
		/// is no event pending.
		/// </summary>
		public void PublishNow()
		{
			if (_timer.Enabled)
				Publish();
		}

		/// <summary>
		/// Called to immediately publish the event with the input
		/// parameters; if there is a pending delay-published event,
		/// it is discarded.
		/// </summary>
		public void PublishNow(object sender, EventArgs args)
		{
			_lastPublishTime = Platform.Time;
			_lastSender = sender;
			_lastArgs = args;

			Publish();
		}

		/// <summary>
		/// Delay-publishes an event with the input parameters.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Repeated calls to <see cref="Publish(object, EventArgs)"/> will cause
		/// only the most recent event parameters to be remembered until the delay
		/// timeout has expired, at which time only those event parameters will
		/// be used to publish the delayed event.
		/// </para>
		/// <para>
		/// When a delayed event is published, the <see cref="DelayedEventPublisher"/>
		/// goes into an idle state.  The next call to <see cref="Publish(object, EventArgs)"/>
		/// starts the delayed publishing process over again.
		/// </para>
		/// </remarks>
		public void Publish(object sender, EventArgs args)
		{
			_lastPublishTime = Platform.Time;
			_lastSender = sender;
			_lastArgs = args;

			_timer.Start();
		}

		/// <summary>
		/// Disposes the <see cref="DelayedEventPublisher"/>.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && _timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}
		}

		#region IDisposable Members

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion
	}
}
