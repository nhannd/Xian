#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Provides a simple mechanism for executing code asynchronously.
	/// </summary>
	public class AsyncTask : IDisposable
	{
		public delegate void Action();

		private BackgroundTask _backgroundTask;
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

			_backgroundTask = new BackgroundTask(
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

			_backgroundTask.Terminated += TerminatedEventHandler;
			_backgroundTask.Run();
		}

		/// <summary>
		/// Causes any pending asynchronous execution to be discarded (the continuation/error handler will not be called). 
		/// </summary>
		public void Cancel()
		{
			if (_backgroundTask != null)
			{
				_continuationCode = null;
				_errorHandler = null;
				_backgroundTask.Terminated -= TerminatedEventHandler;
				_backgroundTask.Dispose();
				_backgroundTask = null;
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
