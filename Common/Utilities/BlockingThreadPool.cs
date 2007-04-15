using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ClearCanvas.Common.Utilities
{
	public delegate void SimpleBlockingThreadPoolDelegate();

	public class SimpleBlockingThreadPool : BlockingThreadPool<SimpleBlockingThreadPoolDelegate>
	{
		public SimpleBlockingThreadPool(int concurrency, bool allowInactiveAdd)
			: base(concurrency, allowInactiveAdd)
		{
		}

		public SimpleBlockingThreadPool(int concurrency)
			: base(concurrency)
		{
		}

		protected SimpleBlockingThreadPool()
			: base()
		{
		}

		protected override void ProcessItem(SimpleBlockingThreadPoolDelegate del)
		{
			//execute the delegate.
			del();
		}
	}

	public abstract class BlockingThreadPool<T> : ThreadPoolBase
		where T : class
	{
		private BlockingQueue<T> _blockingQueue;
		private bool _allowInactiveAdd;
		private volatile int _sleepTime;
	
		public BlockingThreadPool(int concurrency, bool allowInactiveAdd)
			: base(concurrency)
		{
			_blockingQueue = new BlockingQueue<T>();
			_allowInactiveAdd = allowInactiveAdd;
			_sleepTime = 0; //will at least give up the remainder of the time slice.
		}

		protected BlockingThreadPool(int concurrency)
			: this(concurrency, false)
		{ 
		}

		protected BlockingThreadPool()
			: this(MinConcurrency, false)
		{
		}

		public bool AllowInactiveAdd
		{
			get { return _allowInactiveAdd; }
			set
			{
				if (this.Active)
					throw new InvalidOperationException(String.Format(SR.ExceptionThreadPoolMustBeStopped, "AllowInactiveAdd"));

				_allowInactiveAdd = value;
			}
		}

		public int SleepTime
		{
			get { return _sleepTime; }
			set { _sleepTime = value; }
		}

		public int QueueCount
		{
			get { return _blockingQueue.Count; }
		}

		protected override bool OnStart()
		{
			if (!base.OnStart())
				return false;

			_blockingQueue.ContinueBlocking = true;
			return true;
		}

		protected override bool OnStop(bool completeBeforeStop)
		{
			if (!base.OnStop(completeBeforeStop))
				return false;

			_blockingQueue.ContinueBlocking = false;
			return true;
		}

		public void Enqueue(T item)
		{
			if (!_allowInactiveAdd && !this.Active)
				throw new InvalidOperationException(SR.ExceptionThreadPoolNotStarted);

			_blockingQueue.Enqueue(item);
		}
		
		protected override void RunThread()
		{
			while (true)
			{
				T item;
				bool queueEmpty = !_blockingQueue.Dequeue(out item);

				if (!queueEmpty)
					ProcessItem(item);

				if (base.State == StartStopState.Stopping)
				{
					if (queueEmpty || !base.CompleteBeforeStop)
						break;
				}

				Thread.Sleep(_sleepTime);
			}
		}

		protected abstract void ProcessItem(T item);
	}
}
