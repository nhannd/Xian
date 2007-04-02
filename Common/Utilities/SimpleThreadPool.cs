using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ClearCanvas.Common.Utilities
{
	public delegate void SimpleThreadPoolDelegate();

	public class SimpleThreadPool
	{
		public enum StartStopState { Started, Stopped };

		private event EventHandler<ItemEventArgs<StartStopState>> _startStopEvent;
		
		private EventWaitHandle _stopSignal;
		private EventWaitHandle _jobsRemainingSignal;
		private EventWaitHandle[] _waitHandles;
		private List<Thread> _threads;
		private uint _sleepTime;

		private int _concurrency;
		private ThreadPriority _threadPriority = ThreadPriority.BelowNormal;
		private bool _allowInactiveAdd;

		private object _syncLock = new object();
		private List<SimpleThreadPoolDelegate> _jobQueue;

		public SimpleThreadPool(int concurrency, ThreadPriority priority)
		 : this(concurrency)
		{
			_threadPriority = priority;
		}

		public SimpleThreadPool(int concurrency)
		{
			_concurrency = concurrency;
			_sleepTime = 0;

			_stopSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
			_jobsRemainingSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
			_waitHandles = new EventWaitHandle[] { _stopSignal, _jobsRemainingSignal };

			_threads = new List<Thread>();
			_jobQueue = new List<SimpleThreadPoolDelegate>();
		}

		protected SimpleThreadPool()
		{ 
		}

		public event EventHandler<ItemEventArgs<StartStopState>> StartStopEvent
		{
			add { _startStopEvent += value; }
			remove { _startStopEvent -= value; }
		}

		public uint SleepTime
		{
			get { return _sleepTime; }
			set { _sleepTime = value; }
		}

		public bool AllowInactiveAdd
		{
			get { return _allowInactiveAdd; }
			set { _allowInactiveAdd = value; }
		}

		public bool Active
		{
			get { return (_threads.Count > 0); }
		}

		public bool StopSignalled
		{
			get { return _stopSignal.WaitOne(0, false); }
		}

		public void Start()
		{
			if (this.Active)
				throw new InvalidOperationException(SR.ExceptionThreadPoolAlreadyStarted);

			_jobsRemainingSignal.Reset();
			_stopSignal.Reset();

			for (int i = 0; i < _concurrency; ++i)
			{
				ThreadStart threadStart = new ThreadStart(this.RunThread);
				Thread thread = new Thread(threadStart);
				thread.IsBackground = true;
				thread.Priority = ThreadPriority.BelowNormal;

				thread.Start();
				_threads.Add(thread);
			}

			EventsHelper.Fire(_startStopEvent, this, new ItemEventArgs<StartStopState>(StartStopState.Started));
		}

		public void Stop()
		{
			_stopSignal.Set();

			foreach (Thread thread in _threads)
				thread.Join();

			_threads.Clear();

			EventsHelper.Fire(_startStopEvent, this, new ItemEventArgs<StartStopState>(StartStopState.Stopped));
		}

		public void AddFront(SimpleThreadPoolDelegate task)
		{
			if (!_allowInactiveAdd && this.StopSignalled)
				throw new InvalidOperationException(SR.ExceptionThreadPoolNotStarted);

			lock (_syncLock)
			{
				_jobQueue.Insert(0, task);
				_jobsRemainingSignal.Set();
			}
		}

		public void AddEnd(SimpleThreadPoolDelegate task)
		{
			if (!_allowInactiveAdd && this.StopSignalled)
				throw new InvalidOperationException(SR.ExceptionThreadPoolNotStarted);

			lock (_syncLock)
			{
				_jobQueue.Add(task);
				_jobsRemainingSignal.Set();
			}
		}

		private SimpleThreadPoolDelegate Next()
		{
			lock (_syncLock)
			{
				if (_jobQueue.Count == 0)
				{
					_jobsRemainingSignal.Reset();
					return null;
				}

				SimpleThreadPoolDelegate job = _jobQueue[0];
				_jobQueue.RemoveAt(0);
				return job;
			}
		}

		private void RunThread()
		{
			while (true)
			{
				EventWaitHandle.WaitAny(_waitHandles);

				try
				{
					SimpleThreadPoolDelegate task = Next();
					if (task != null)
						task();
				}
				catch (Exception e)
				{
					Platform.Log(e);
				}

				if (this.StopSignalled)
					break;

				Thread.Sleep((int)_sleepTime);
			}
		}
	}
}
