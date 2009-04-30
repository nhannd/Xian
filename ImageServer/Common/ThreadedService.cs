#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Threading;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Common
{
	/// <summary>
	/// Base class for a service that runs in a dedicated thread.
	/// </summary>
	public abstract class ThreadedService
	{
		#region Private Members
		private ManualResetEvent _threadStop;
		private Thread _theThread = null;
		private bool _stopFlag = false;
		private readonly string _name;
		private int _threadRetryDelay = 60000;
		#endregion

		#region Public Properties
		/// <summary>
		/// Flag set to true if stop has been requested.
		/// </summary>
		public bool StopFlag
		{
			get { return _stopFlag; }
		}

		/// <summary>
		/// Reset event to signal when stopping the service thread.
		/// </summary>
		public ManualResetEvent ThreadStop
		{
			get { return _threadStop; }
		}

		/// <summary>
		/// The name of the thread.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Retry delay (in ms) before retrying after a failure
		/// </summary>
		public int ThreadRetryDelay
		{
			get { return _threadRetryDelay; }
			set { _threadRetryDelay = value; }
		}
		#endregion

		#region Protected Abstract Methods
		protected abstract void Initialize();
		protected abstract void Run();
		protected abstract void Stop();
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of the service.</param>
		public ThreadedService(string name)
		{
			_name = name;
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// Check if a stop is requested.
		/// </summary>
		/// <param name="msDelay"></param>
		/// <returns></returns>
		protected bool CheckStop(int msDelay)
		{
			ThreadStop.WaitOne(msDelay, false);
			ThreadStop.Reset();

			return StopFlag;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Start the service.
		/// </summary>
		public void StartService()
		{
			if (_theThread == null)
			{
				_threadStop = new ManualResetEvent(false);
				_theThread = new Thread(delegate()
											{
												bool bInit = false;
												while (!bInit)
												{
													try
													{
														Initialize();
														bInit = true;
													}
													catch (Exception e)
													{
														Platform.Log(LogLevel.Warn, e, "Unexpected exception intializing {0} service", Name);
													}
													if (!bInit)
													{
														// Wait before retrying init
														ThreadStop.WaitOne(ThreadRetryDelay, false);
														ThreadStop.Reset();

														if (_stopFlag)
															return;
													}
												}

												try
												{
													Run();
												}
												catch (Exception e)
												{
													Platform.Log(LogLevel.Error, e, "Unexpected exception running service {0}", Name);
												}

											});
				_theThread.Name = String.Format("{0}:{1}", Name, _theThread.ManagedThreadId);
				_theThread.Start();
			}
		}

		/// <summary>
		/// Stop the service.
		/// </summary>
		public void StopService()
		{
			_stopFlag = true;
			Stop();

			if (_theThread.IsAlive)
			{
				ThreadStop.Set();
				_theThread.Join();
				_theThread = null;
			}
		}
		#endregion
	}
}
