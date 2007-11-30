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
using System.ComponentModel;
using ClearCanvas.Common;
using System.Threading;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Delegate used to queue events for marshaling.
    /// </summary>
    public delegate void InvokeDelegate();

    /// <summary>
    /// This class allows any block of code, in the form of a delegate, to be passed from an abitrary
    /// thread over to the application's main thread for execution. 
    /// </summary>
	/// <remarks>
	/// Note that on disposal, this object does not finish processing its queue, it quits immediately.
	/// </remarks>
    public class InterthreadMarshaler : IDisposable
    {
        private BlockingQueue<InvokeDelegate> _queue;
        private BackgroundWorker _queueProcessor;

		private object _threadExitingLock = new object(); 
        
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// The object must be constructed on the thread that events should be marshaled to.
		/// </remarks>
		public InterthreadMarshaler()
		{
			_queue = new BlockingQueue<InvokeDelegate>();
			_queueProcessor = new BackgroundWorker();
			_queueProcessor.WorkerReportsProgress = true;
			_queueProcessor.DoWork += new DoWorkEventHandler(ProcessQueueAsync);
			_queueProcessor.ProgressChanged += new ProgressChangedEventHandler(QueueProgressEventHandler);
			_queueProcessor.RunWorkerAsync();
		}

        /// <summary>
        /// Queues the specified delegate for invocation on this object's thread, regardless of which
        /// thread this method is called from. 
        /// </summary>
        /// <remarks>
		/// Note that this method simply queues the delegate and returns.  There 
		/// is no guarantee as to when the delegate will actually be invoked.
		/// </remarks>
        /// <param name="del">The delegate to be invoked on this object's thread.</param>
        public void QueueInvoke(InvokeDelegate del)
        {
            _queue.Enqueue(del);
        }

        /// <summary>
        /// Handles progress events from the queue processing thread.
        /// </summary>
        private void QueueProgressEventHandler(object sender, ProgressChangedEventArgs e)
        {
            // this method will be called on the main thread, hence it will fire the event
            // on the main thread
            InvokeDelegate del = (InvokeDelegate)e.UserState;
            del();
        }

        /// <summary>
        /// Defines the worker process for the queue processing thread.
        /// </summary>
        private void ProcessQueueAsync(object sender, DoWorkEventArgs e)
        {
            // this method is running on a background thread
            // consume each object on the queue, and marshall it over to the main thread
            // as a "progress" event
            // note that this is effectively an infinite loop that terminates
            // only when this object is Disposed
			while(true)
			{
				InvokeDelegate del;
				_queue.Dequeue(out del);

				//we've been signalled to quit.
				if (!_queue.ContinueBlocking)
					break;

				_queueProcessor.ReportProgress(0, del);
			}

			lock (_threadExitingLock)
			{
				Monitor.Pulse(_threadExitingLock);
			}
		}

        /// <summary>
        /// Supports the implementation of the <see cref="IDisposable"/> pattern.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
				if (_queue != null)
				{
					lock (_threadExitingLock)
					{
						_queue.ContinueBlocking = false;
						Monitor.Wait(_threadExitingLock);
					}
					_queue = null;
				}
				
				if (_queueProcessor != null)
                {
					_queueProcessor.DoWork -= new DoWorkEventHandler(ProcessQueueAsync);
					_queueProcessor.ProgressChanged -= new ProgressChangedEventHandler(QueueProgressEventHandler);

                    _queueProcessor.Dispose();
                    _queueProcessor = null;
                }
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
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error,e);
            }
        }

        #endregion
    }
}
