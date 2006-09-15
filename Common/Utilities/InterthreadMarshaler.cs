using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using ClearCanvas.Common;

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
    public class InterthreadMarshaler : IDisposable
    {
        private BlockingQueue<InvokeDelegate> _queue;
        private BackgroundWorker _queueProcessor;

        /// <summary>
        /// Constructor.  The object must be constructed on the thread that events should be marshaled to.
        /// </summary>
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
        /// thread this method is called from.  Note that this method simply queues the delegate and
        /// returns.  There is no guarantee as to when the delegate will actually be invoked.
        /// </summary>
        /// <param name="del"></param>
        public void QueueInvoke(InvokeDelegate del)
        {
            _queue.Enqueue(del);
        }

        /// <summary>
        /// Handles progress events from the queue processing thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueueProgressEventHandler(object sender, ProgressChangedEventArgs e)
        {
            // this method will be called on the main thread, hence it will fire the event
            // on the main thread
            InvokeDelegate del = (InvokeDelegate)e.UserState;
            del();
        }

        /// <summary>
        /// Defines the worker process for the queue processing thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessQueueAsync(object sender, DoWorkEventArgs e)
        {
            // this method is running on a background thread
            // consume each object on the queue, and marshall it over to the main thread
            // as a "progress" event
            // note that this is effectively an infinite loop that terminates
            // only when this object is Disposed
            foreach (InvokeDelegate del in _queue)
            {
                _queueProcessor.ReportProgress(0, del);
            }
        }

        /// <summary>
        /// Supports the implementation of the <see cref="IDisposable"/> pattern.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_queueProcessor != null)
                {
                    _queueProcessor.Dispose();
                    _queueProcessor = null;
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                Platform.Log(e);
            }
        }

        #endregion
    }
}
