using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using ClearCanvas.Common;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Allows the background thread to communicate with the <see cref="BackgroundTask"/> object.
    /// Note that the background thread should not directly access the <see cref="BackgroundTask"/> object.
    /// </summary>
    public interface IBackgroundTaskContext
    {
        /// <summary>
        /// Provides access to the user state object.  Note that the user state object is available
        /// to both threads, and therefore it should either be immutable or thread-safe.
        /// </summary>
        object UserState { get; }

        /// <summary>
        /// Allows the background thread to ask whether cancellation has been requested.  If possible, the
        /// <see cref="BackgroundTaskMethod"/> should periodically poll this flag, and if true, terminate as quickly as possible
        /// without completing its work.  It should call <see cref="Cancel"/> to confirm cancellation.
        /// </summary>
        bool CancelRequested { get; }

        /// <summary>
        /// Allows the <see cref="BackgroundTaskMethod"/> to report progress to the foreground thread.  The progress object
        /// may be an instance of <see cref="BackgroundTaskProgress"/> or a derived class.
        /// </summary>
        /// <param name="progress"></param>
        void ReportProgress(BackgroundTaskProgress progress);

        /// <summary>
        /// Allows the <see cref="BackgroundTaskMethod"/> to inform that it has completed successfully, and return the result object
        /// to the foreground thread.  After calling this method, the <see cref="BackgroundTaskMethod"/> should simply exit.
        /// </summary>
        /// <param name="result"></param>
        void Complete(object result);

        /// <summary>
        /// Allows the <see cref="BackgroundTaskMethod"/> to inform that it has successfully cancelled,
        /// in response to querying the <see cref="CancelRequested"/> flag, and return the result object
        /// to the foreground thread.  After calling this method, the <see cref="BackgroundTaskMethod"/> should simply exit.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Allows the <see cref="BackgroundTaskMethod"/> to inform that it cannot complete due to an exception,
        /// and return the exception to the foreground thread.  After calling this method,
        /// the <see cref="BackgroundTaskMethod"/> should simply exit.  Note that it is technically ok for the background
        /// method to allow an exception to go unhandled, an the unhandled exception will still be reported to the foreground
        /// thread as an error.  However, the VS debugger will break in this case, which may not be desirable.
        /// </summary>
        /// <param name="e"></param>
        void Error(Exception e);
    }

    /// <summary>
    /// Defines the signature for a method that is to be executed as a background task.
    /// </summary>
    /// <param name="context"></param>
    public delegate void BackgroundTaskMethod(IBackgroundTaskContext context);

    /// <summary>
    /// Encapsulates information about the progress of the task.  This class may be subclassed in order
    /// to add additional information, or override the existing methods.
    /// </summary>
    public class BackgroundTaskProgress
    {
        private int _percent;
        private string _message;

        /// <summary>
        /// Constructor
        /// </summary>
        public BackgroundTaskProgress()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="message"></param>
        public BackgroundTaskProgress(int percent, string message)
        {
            Platform.CheckArgumentRange(percent, 0, 100, "percent");
            _percent = percent;
            _message = message;
        }

        /// <summary>
        /// Gets the percent completion, as an integer between 0..100
        /// </summary>
        public virtual int Percent { get { return _percent; } }

        /// <summary>
        /// Gets a status message describing the current state of the task
        /// </summary>
        public virtual string Message { get { return _message; } }
    }

    /// <summary>
    /// Conveys progress information about a <see cref="BackgroundTask"/>
    /// </summary>
    public class BackgroundTaskProgressEventArgs : EventArgs
    {
        private object _userState;
        private BackgroundTaskProgress _progress;

        public BackgroundTaskProgressEventArgs(object userState, BackgroundTaskProgress progress)
        {
            _userState = userState;
            _progress = progress;
        }

        /// <summary>
        /// Gets the progress object passed from the background thread.
        /// </summary>
        public BackgroundTaskProgress Progress { get { return _progress; } }

        /// <summary>
        /// Gets the user state object associated with the task.
        /// </summary>
        public object UserState { get { return _userState; } }
    }

    /// <summary>
    /// Defines the possible reasons a <see cref="BackgroundTask"/> has terminated.
    /// </summary>
    public enum BackgroundTaskTerminatedReason
    {
        /// <summary>
        /// The task completed
        /// </summary>
        Completed,

        /// <summary>
        /// The task was cancelled
        /// </summary>
        Cancelled,

        /// <summary>
        /// The task encountered an exception and could not complete
        /// </summary>
        Exception
    }

    /// <summary>
    /// Conveys information about the termination of a <see cref="BackgroundTask"/>.
    /// </summary>
    public class BackgroundTaskTerminatedEventArgs : EventArgs
    {
        private object _userState;
        private BackgroundTaskTerminatedReason _reason;
        private object _result;
        private Exception _exception;

        public BackgroundTaskTerminatedEventArgs(object userState, BackgroundTaskTerminatedReason reason, object result, Exception ex)
        {
            _userState = userState;
            _reason = reason;
            _result = result;
            _exception = ex;
        }

        /// <summary>
        /// Gets the reason for termination
        /// </summary>
        public BackgroundTaskTerminatedReason Reason { get { return _reason; } }

        /// <summary>
        /// Gets the result of the background task
        /// </summary>
        public object Result { get { return _result; } }

        /// <summary>
        /// Gets the exception that occured, if <see cref="Reason"/> is <see cref="BackgroundTaskTerminatedReason.Exception"/>
        /// </summary>
        public Exception Exception { get { return _exception; } }
    }

    /// <summary>
    /// Encapsulates a background task, allowing the task to run asynchronously on a background thread
    /// and report progress and completion events to the foreground thread.
    /// </summary>
    public class BackgroundTask : IDisposable
    {
        #region Helper implementation of IBackgroundTaskContext

        class BackgroundTaskContext : IBackgroundTaskContext
        {
            private BackgroundTask _owner;
            private DoWorkEventArgs _doWorkArgs;

            public BackgroundTaskContext(BackgroundTask owner, DoWorkEventArgs doWorkArgs)
            {
                _owner = owner;
                _doWorkArgs = doWorkArgs;
            }

            #region IBackgroundTaskContext Members

            public object UserState
            {
                get { return _doWorkArgs.Argument; }
            }

            public bool CancelRequested
            {
                get { return _owner.CancelRequestPending; }
            }

            public void Complete(object result)
            {
                _doWorkArgs.Result = result;
            }

            public void Cancel()
            {
                _doWorkArgs.Cancel = true;
            }

            public void Error(Exception e)
            {
                _owner._error = e;
            }

            public void ReportProgress(BackgroundTaskProgress progress)
            {
                _owner._backgroundWorker.ReportProgress(0, progress);
            }

            #endregion
        }

        #endregion

        private BackgroundWorker _backgroundWorker;
        private BackgroundTaskMethod _method;
        private object _userState;
        private Exception _error;
        private event EventHandler<BackgroundTaskProgressEventArgs> _progressUpdated;
        private event EventHandler<BackgroundTaskTerminatedEventArgs> _terminated;

        /// <summary>
        /// Creates and executes a new <see cref="BackgroundTask"/> based on the specified arguments.
        /// </summary>
        /// <param name="method">The method to run in the background</param>
        /// <param name="supportsCancel">Indicates whether the task supports cancellation or not</param>
        /// <param name="terminateHandler">Method that will be called when the task terminates</param>
        /// <param name="progressHandler">Optional method to handle progress updates, may be null</param>
        /// <param name="userState">Optional state to be passed to the background task, may be null</param>
        /// <returns>A running <see cref="BackgroundTask"/> object.</returns>
        public static BackgroundTask CreateAndRun(
            BackgroundTaskMethod method,
            bool supportsCancel,
            EventHandler<BackgroundTaskTerminatedEventArgs> terminateHandler,
            EventHandler<BackgroundTaskProgressEventArgs> progressHandler,
            object userState)
        {
            Platform.CheckForNullReference(method, "method");
            Platform.CheckForNullReference(terminateHandler, "terminateHandler");

            BackgroundTask task = new BackgroundTask(method, supportsCancel, userState);
            task.Terminated += terminateHandler;
            if (progressHandler != null)
            {
                task.ProgressUpdated += progressHandler;
            }
            task.Run();
            return task;
        }

        /// <summary>
        /// Constructs a new background task based on the specified method and optional state object.  The task
        /// is not executed until the <see cref="Run"/> method is called.
        /// </summary>
        /// <param name="method">The method to run in the background</param>
        /// <param name="supportsCancel">Indicates whether the task supports cancellation or not</param>
        /// <param name="userState">Optional state to be passed to the background method</param>
        public BackgroundTask(BackgroundTaskMethod method, bool supportsCancel, object userState)
        {
            Platform.CheckForNullReference(method, "method");
            _method = method;
            _userState = userState;

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.WorkerSupportsCancellation = supportsCancel;
            _backgroundWorker.DoWork += new DoWorkEventHandler(_backgroundWorker_DoWork);
            _backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(_backgroundWorker_ProgressChanged);
            _backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_backgroundWorker_RunWorkerCompleted);
        }

        /// <summary>
        /// Constructs a new background task based on the specified method.  The task
        /// is not executed until the <see cref="Run"/> method is called.
        /// </summary>
        /// <param name="method">The method to run in the background</param>
        /// <param name="supportsCancel">Indicates whether the task supports cancellation or not</param>
        public BackgroundTask(BackgroundTaskMethod method, bool supportsCancel)
            : this(method, supportsCancel, null)
        {
        }

        /// <summary>
        /// Runs the background task
        /// </summary>
        public void Run()
        {
            if (_backgroundWorker.IsBusy)
                throw new InvalidOperationException(SR.ExceptionBackgroundTaskAlreadyRunning);
            
            _backgroundWorker.RunWorkerAsync(_userState);
        }

        /// <summary>
        /// True if the task is currently running
        /// </summary>
        public bool IsRunning
        {
            get { return _backgroundWorker.IsBusy; }
        }

        /// <summary>
        /// True if the task supports cancellation
        /// </summary>
        public bool SupportsCancel
        {
            get { return _backgroundWorker.WorkerSupportsCancellation; }
        }

        /// <summary>
        /// Requests that the background task be cancelled.  The does not actually stop running until
        /// the <see cref="Terminated"/> event is fired.
        /// </summary>
        public void RequestCancel()
        {
            _backgroundWorker.CancelAsync();
        }

        /// <summary>
        /// True if the <see cref="RequestCancel"/> method has been called, and the request is pending.
        /// </summary>
        public bool CancelRequestPending
        {
            get { return _backgroundWorker.CancellationPending; }
        }

        /// <summary>
        /// Notifies that the progress of the task has been updated
        /// </summary>
        public event EventHandler<BackgroundTaskProgressEventArgs> ProgressUpdated
        {
            add { _progressUpdated += value; }
            remove { _progressUpdated -= value; }
        }

        /// <summary>
        /// Notifies that the task has terminated.  Check the <see cref="BackgroundTaskTerminatedEventArgs"/>
        /// to determine the reason for termination, and obtain results.
        /// </summary>
        public event EventHandler<BackgroundTaskTerminatedEventArgs> Terminated
        {
            add { _terminated += value; }
            remove { _terminated -= value; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _backgroundWorker.Dispose();
        }

        #endregion
        
        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // execute the operation
            _method(new BackgroundTaskContext(this, e));
        }

        private void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            EventsHelper.Fire(_progressUpdated, this, new BackgroundTaskProgressEventArgs(_userState, 
                (BackgroundTaskProgress)e.UserState));
        }
        
        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // if there was an unhandled exception in the worker thread, then e.Error will be non-null
            // if there was a handled exception in the worker thread, and the worker passed it back, _error will be non-null
            // therefore, coalesce the results, giving precedence to the unhandled exception
            _error = e.Error ?? _error;

            // determine the reason
            BackgroundTaskTerminatedReason reason = (_error != null) ? BackgroundTaskTerminatedReason.Exception
                : (e.Cancelled ? BackgroundTaskTerminatedReason.Cancelled : BackgroundTaskTerminatedReason.Completed);

            // the e.Result object is an exception for e.Cancelled status, we don't want that
            Object obj = (e.Cancelled ? null : e.Result);

            EventsHelper.Fire(_terminated, this,
                new BackgroundTaskTerminatedEventArgs(_userState, reason, obj, _error));
        }

    }
}
