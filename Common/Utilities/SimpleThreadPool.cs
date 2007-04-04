using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ClearCanvas.Common.Utilities
{
	public delegate void SimpleThreadPoolDelegate();

	public class SimpleThreadPool : SimpleThreadPoolBase<SimpleThreadPoolDelegate>
	{
		public SimpleThreadPool(int concurrency, ThreadPriority priority)
		 : base(concurrency, priority)
		{
		}

		public SimpleThreadPool(int concurrency)
			: base(concurrency)
		{
		}

		protected SimpleThreadPool()
			: base()
		{
		}

		protected override void ProcessItem(SimpleThreadPoolDelegate del)
		{
			//execute the delegate.
			del();
		}
	}

	public abstract class SimpleThreadPoolBase<T>
		where T : class
	{
		public enum StartStopState { Started, Stopped };

		private object _queueSyncLock = new object();
		private Queue<T> _queue;

		private object _startStopEventSyncLock = new object();
		private event EventHandler<ItemEventArgs<StartStopState>> _startStopEvent;

		private object _startStopSyncLock = new object();
		private bool _active;
		private List<Thread> _threads;

		private volatile bool _stopThreads;
		private volatile bool _completeBeforeStop;
		private volatile bool _allowInactiveAdd;
		private volatile uint _sleepTime;
		
		private ThreadPriority _threadPriority = ThreadPriority.BelowNormal;
		private int _concurrency;
		
		public SimpleThreadPoolBase(int concurrency, ThreadPriority priority)
		 : this(concurrency)
		{
			_threadPriority = priority;
		}

		public SimpleThreadPoolBase(int concurrency)
			: this()
		{
			_concurrency = concurrency;
		}

		protected SimpleThreadPoolBase()
		{
			_sleepTime = 0;
			_active = false;

			_stopThreads = false;
			_completeBeforeStop = false;
			_allowInactiveAdd = false;

			_threads = new List<Thread>();
			_queue = new Queue<T>();
		}

		public event EventHandler<ItemEventArgs<StartStopState>> StartStopEvent
		{
			add 
			{
				lock (_startStopEventSyncLock)
				{
					_startStopEvent += value;
				}
			}
			remove 
			{
				lock (_startStopEventSyncLock)
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
					return _active;
				}
			}
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

		public bool CompleteBeforeStop
		{
			get { return _completeBeforeStop; }
			set { _completeBeforeStop = value; }
		}

		public void Start()
		{
			lock (_startStopSyncLock)
			{
				if (_active)
					return;

				for (int i = 0; i < _concurrency; ++i)
				{
					ThreadStart threadStart = new ThreadStart(this.RunThread);
					Thread thread = new Thread(threadStart);
					thread.IsBackground = true;
					thread.Priority = ThreadPriority.BelowNormal;

					thread.Start();
					_threads.Add(thread);
				}

				_active = true;

				lock (_startStopEventSyncLock)
				{
					EventsHelper.Fire(_startStopEvent, this, new ItemEventArgs<StartStopState>(StartStopState.Started));
				}
			}
		}

		public void Stop()
		{
			lock (_startStopSyncLock)
			{
				if (!_active)
					return;

				_stopThreads = true;

				lock (_queueSyncLock)
				{
					Monitor.PulseAll(_queueSyncLock);
				}

				foreach (Thread thread in _threads)
					thread.Join();

				_threads.Clear();

				_stopThreads = false;
				_active = false;

				lock (_startStopEventSyncLock)
				{
					EventsHelper.Fire(_startStopEvent, this, new ItemEventArgs<StartStopState>(StartStopState.Stopped));
				}
			}
		}

		public void Enqueue(T item)
		{
			if (!_allowInactiveAdd && !this.Active)
				throw new InvalidOperationException(SR.ExceptionThreadPoolNotStarted);

			lock (_queueSyncLock)
			{
				_queue.Enqueue(item);
				Monitor.Pulse(_queueSyncLock);
			}
		}

		private void RunThread()
		{
			while (true)
			{
				try
				{
					T item = null;

					lock (_queueSyncLock)
					{
						if (!_stopThreads)
						{
							while (_queue.Count == 0 && !_stopThreads)
								Monitor.Wait(_queueSyncLock);
						}
						else
						{
							if (!_completeBeforeStop)
							{
								break;
							}
							else
							{
								if (_queue.Count == 0)
									break;
							}
						}

						if (_queue.Count > 0)
							item = _queue.Dequeue();
					}

					if (item != null)
						ProcessItem(item);
				}
				catch (Exception e)
				{
					Platform.Log(e);
				}

				Thread.Sleep((int)_sleepTime);
			}
		}

		protected abstract void ProcessItem(T item);
	}
}
