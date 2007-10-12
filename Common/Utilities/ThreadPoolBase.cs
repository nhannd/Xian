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