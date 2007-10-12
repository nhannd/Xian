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
