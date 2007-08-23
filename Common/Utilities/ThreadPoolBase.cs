using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ClearCanvas.Common.Utilities
{
	public abstract class ThreadPoolBase
	{
		public static readonly int MinConcurrency = 1;
		public static readonly int MaxConcurrency = 100;

		public enum StartStopState { Starting, Started, Stopping, Stopped };

		private readonly object _startStopSyncLock = new object();
		private StartStopState _state;
		private bool _completeBeforeStop;
		
		private List<Thread> _threads;
		private event EventHandler<ItemEventArgs<StartStopState>> _startStopEvent;

		private int _concurrency = MinConcurrency;
		private ThreadPriority _threadPriority;

		public ThreadPoolBase(int concurrency)
			: this()
		{
			this.Concurrency = concurrency;
		}

		protected ThreadPoolBase()
		{
			_state = StartStopState.Stopped;
			_completeBeforeStop = false;
			_threads = new List<Thread>();
			_threadPriority = ThreadPriority.Normal;
		}

		public event EventHandler<ItemEventArgs<StartStopState>> StartStopStateChangedEvent
		{
			add 
			{
				lock (_startStopSyncLock)
				{
					_startStopEvent += value;
				}
			}
			remove 
			{
				lock (_startStopSyncLock)
				{
					_startStopEvent -= value;
				}
			}
		}
		
		public bool Active
		{
			get 
			{
				lock (_startStopSyncLock)
				{
					return _state != StartStopState.Stopped;
				}
			}
		}

		public int Concurrency
		{
			get { return _concurrency; }
			set
			{
				if (this.Active)
					throw new InvalidOperationException(String.Format(SR.ExceptionThreadPoolMustBeStopped, "Concurrency"));

				Platform.CheckPositive(value, "Concurrency");
				Platform.CheckArgumentRange(value, MinConcurrency, MaxConcurrency, "Concurrency");

				_concurrency = value;
			}
		}

		public ThreadPriority ThreadPriority
		{
			get { return _threadPriority; }
			set
			{
				if (this.Active)
					throw new InvalidOperationException(String.Format(SR.ExceptionThreadPoolMustBeStopped, "ThreadPriority"));

				_threadPriority = value;
			}
		}


		protected object StartStopSyncLock
		{
			get { return _startStopSyncLock; }
		}

		protected bool CompleteBeforeStop
		{
			get 
			{
				lock (_startStopSyncLock)
				{
					return _completeBeforeStop;
				}
			}
		}

		protected StartStopState State
		{
			get
			{
				lock (_startStopSyncLock)
				{
					return _state;
				}
			}
		}

		protected virtual bool OnStart()
		{
			//lock other threads out of the Start function by checking/setting the state.
			lock (_startStopSyncLock)
			{
				if (_state != StartStopState.Stopped)
					return false;

				_state = StartStopState.Starting;
				EventsHelper.Fire(_startStopEvent, this, new ItemEventArgs<StartStopState>(_state));
			}

			return true;
		}

		protected virtual void OnStarted()
		{
			lock (_startStopSyncLock)
			{
				_state = StartStopState.Started;
				EventsHelper.Fire(_startStopEvent, this, new ItemEventArgs<StartStopState>(_state));
			}
		}

		protected virtual bool OnStop(bool completeBeforeStop)
		{
			//lock other threads out of the Stop function by checking/setting the state.
			lock (_startStopSyncLock)
			{
				if (_state != StartStopState.Started)
					return false;

				_completeBeforeStop = completeBeforeStop;
				_state = StartStopState.Stopping;
				EventsHelper.Fire(_startStopEvent, this, new ItemEventArgs<StartStopState>(_state));
			}
			
			return true;
		}

		protected virtual void OnStopped()
		{
			lock (_startStopSyncLock)
			{
				_state = StartStopState.Stopped;
				EventsHelper.Fire(_startStopEvent, this, new ItemEventArgs<StartStopState>(_state));
			}
		}

		public void Start()
		{
			if (!OnStart())
				return;

			for (int i = 0; i < _concurrency; ++i)
			{
				ThreadStart threadStart = new ThreadStart(this.RunThread);
				Thread thread = new Thread(threadStart);
                thread.Name = String.Format("Pool {0}", thread.ManagedThreadId);
				thread.IsBackground = true;
				thread.Priority = _threadPriority;

				thread.Start();
				_threads.Add(thread);
			}

			OnStarted();
		}

		public void Stop()
		{
			Stop(false);
		}

		public void Stop(bool completeBeforeStop)
		{
			if (!OnStop(completeBeforeStop))
				return;

			foreach (Thread thread in _threads)
				thread.Join();

			_threads.Clear();

			OnStopped();
		}

		protected abstract void RunThread();
	}
}