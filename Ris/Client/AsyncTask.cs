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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Provides a simple mechanism for executing code asynchronously.
	/// </summary>
	public class AsyncTask : IDisposable
	{
		public delegate void Action();

		private BackgroundTask _loaderTask;
		private Action _continuationCode;
		private Action<Exception> _errorHandler;

		/// <summary>
		/// Runs specified code asynchronously, executing the continuation code when the asynchronous code completes.
		/// </summary>
		/// <remarks>
		/// The <paramref name="asyncCode"/> block is executed on the thread pool.  When this block completes, the
		/// <paramref name="continuationCode"/> block is executed on the calling thread.  If an exception is thrown
		/// in the async block, the exeception is logged and the continuation code is not executed.
		/// This method returns immediately to the caller.  Subsequent calls to this method will cause any pending
		/// prior call to be effectively abandoned.
		/// </remarks>
		/// <param name="asyncCode"></param>
		/// <param name="continuationCode"></param>
		public void Run(Action asyncCode, Action continuationCode)
		{
			Run(asyncCode, continuationCode, DefaultErrorHandler);
		}

		/// <summary>
		/// Runs specified code asynchronously, executing the continuation code when the asynchronous code completes.
		/// </summary>
		/// <remarks>
		/// The <paramref name="asyncCode"/> block is executed on the thread pool.  When this block completes, the
		/// <paramref name="continuationCode"/> block is executed on the calling thread.  If an exception is thrown
		/// in the async block, the <paramref name="errorHandler"/> is executed instead of the continuation block.
		/// This method returns immediately to the caller.  Subsequent calls to this method will cause any pending
		/// prior call to be effectively abandoned.
		/// </remarks>
		/// <param name="asyncCode"></param>
		/// <param name="continuationCode"></param>
		/// <param name="errorHandler"></param>
		public void Run(Action asyncCode, Action continuationCode, Action<Exception> errorHandler)
		{
			// clear any previous task
			Cancel();

			_continuationCode = continuationCode;
			_errorHandler = errorHandler;

			_loaderTask = new BackgroundTask(
				delegate(IBackgroundTaskContext context)
				{
					try
					{
						asyncCode();
						context.Complete();
					}
					catch (Exception e)
					{
						context.Error(e);
					}
				}, false);

			_loaderTask.Terminated += TerminatedEventHandler;
			_loaderTask.Run();
		}

		/// <summary>
		/// Causes any pending asynchronous execution to be discarded (the continuation/error handler will not be called). 
		/// </summary>
		public void Cancel()
		{
			if (_loaderTask != null)
			{
				_continuationCode = null;
				_errorHandler = null;
				_loaderTask.Terminated -= TerminatedEventHandler;
				_loaderTask.Dispose();
				_loaderTask = null;
			}
		}

		/// <summary>
		/// Default error handler, used when no error handler is explicitly provided.
		/// </summary>
		/// <param name="e"></param>
		public static void DefaultErrorHandler(Exception e)
		{
			Platform.Log(LogLevel.Error, e);
		}

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <remarks>
		/// Cancels any pending asynchronous results.
		/// </remarks>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			Cancel();
		}

		#endregion

		private void TerminatedEventHandler(object sender, BackgroundTaskTerminatedEventArgs args)
		{
			if(args.Reason == BackgroundTaskTerminatedReason.Completed)
			{
				_continuationCode();
			}
			else
			{
				_errorHandler(args.Exception);
			}
		}


	}
}
