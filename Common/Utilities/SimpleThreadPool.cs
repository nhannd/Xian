using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ClearCanvas.Common.Utilities
{
	public interface IJob
	{
		void Execute();
	}

	public class StartStopEventArgs : EventArgs
	{ 
		public enum StartStopState { Started, Stopped };

		private StartStopState _state;

		public StartStopEventArgs(StartStopState state)
		{
			_state = state;
		}

		public StartStopState State
		{
			get { return _state; }
		}
	}

	public class SimpleThreadPool<T>
		where T : IJob
	{
		private event EventHandler<StartStopEventArgs> _startStopEvent;
		
		private EventWaitHandle _stopSignal;
		private EventWaitHandle _jobsRemainingSignal;
		private EventWaitHandle[] _waitHandles;
		private List<Thread> _threads;

		private int _concurrency;
		private ThreadPriority _threadPriority = ThreadPriority.BelowNormal;

		private object _syncLock = new object();
		private Queue<T> _queue;

		public SimpleThreadPool(int concurrency, ThreadPriority priority)
		 : this(concurrency)
		{
			_threadPriority = priority;
		}

		public SimpleThreadPool(int concurrency)
		{
			_concurrency = concurrency;

			_stopSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
			_jobsRemainingSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
			_waitHandles = new EventWaitHandle[] { _stopSignal, _jobsRemainingSignal };

			_threads = new List<Thread>();
			_queue = new Queue<T>();
		}

		private SimpleThreadPool()
		{ 
		}

		public event EventHandler<StartStopEventArgs> StartStopEvent
		{
			add { _startStopEvent += value; }
			remove { _startStopEvent -= value; }
		}

		public bool Active
		{
			get { return (_threads.Count > 0); }
		}

		public void Start()
		{
			if (_threads.Count > 0)
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

			EventsHelper.Fire(_startStopEvent, this, new StartStopEventArgs(StartStopEventArgs.StartStopState.Started));
		}

		public void Stop()
		{
			_stopSignal.Set();

			foreach (Thread thread in _threads)
				thread.Join();

			_threads.Clear();
			
			EventsHelper.Fire(_startStopEvent, this, new StartStopEventArgs(StartStopEventArgs.StartStopState.Stopped));
		}

		public virtual void Push(T task)
		{
			if (_threads.Count <= 0)
				throw new InvalidOperationException(SR.ExceptionThreadPoolNotStarted);

			lock (_syncLock)
			{
				_queue.Enqueue(task);
				_jobsRemainingSignal.Set();
			}
		}

		private T Pop()
		{
			lock (_syncLock)
			{
				if (_queue.Count == 0)
				{
					_jobsRemainingSignal.Reset();
					return default(T);
				}

				return _queue.Dequeue();
			}
		}

		private void RunThread()
		{
			while (true)
			{
				EventWaitHandle.WaitAny(_waitHandles);
				if (_stopSignal.WaitOne(0, false))
					break;

				try
				{
					T task = Pop();
					if (task is T)
						task.Execute();
				}
				catch (Exception e)
				{
					Platform.Log(e);
				}

				Thread.Sleep(10);
			}
		}
	}
}
