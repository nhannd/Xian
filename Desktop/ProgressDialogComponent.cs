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

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Extension point for views onto <see cref="ProgressDialogComponent"/>.
    /// </summary>
    [ExtensionPoint]
	public sealed class ProgressDialogComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

	/// <summary>
	/// A default implementation of a progress dialog as an <see cref="ApplicationComponent"/>.
	/// </summary>
	/// <remarks>
	/// The progress dialog blocks the UI thread while the actual processing takes place
	/// on a worker thread (a <see cref="BackgroundTask"/>).  You must therefore be very careful
	/// to ensure that all shared resources are thread-safe.
	/// </remarks>
    [AssociateView(typeof(ProgressDialogComponentViewExtensionPoint))]
    public class ProgressDialogComponent : ApplicationComponent
    {
        private BackgroundTask _task;
        private bool _autoClose;
        private Exception _exception;

        private int _progressBar;
        private string _progressMessage;
        private bool _enableCancel;
        private ProgressBarStyle _progressBarStyle;
        private int _marqueeSpeed;

        private EventHandler<EventArgs> _progressUpdateEvent;
        private EventHandler<EventArgs> _progressTerminateEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="task">The task that will run in the background, reporting its progress to the foreground UI thread.</param>
        /// <param name="autoClose">Whether or not the progress dialog should close automatically upon task completion.</param>
        /// <param name="progressBarStyle">The style of the progress bar.</param>
        public ProgressDialogComponent(BackgroundTask task, bool autoClose, ProgressBarStyle progressBarStyle)
        {
            _task = task;
            _autoClose = autoClose;

            _progressBar = 0;
            _progressMessage = "";
            _progressBarStyle = progressBarStyle;
            _marqueeSpeed = 100;

            if (_task != null)
                _enableCancel = _task.SupportsCancel;
            else
                _enableCancel = false;
        }

		/// <summary>
		/// Raised when a progress update has occurred.
		/// </summary>
		public event EventHandler<EventArgs> ProgressUpdateEvent
		{
			add { _progressUpdateEvent += value; }
			remove { _progressUpdateEvent -= value; }
		}

		/// <summary>
		/// Raised when the task has finished, for any reason.
		/// </summary>
		public event EventHandler<EventArgs> ProgressTerminateEvent
		{
			add { _progressTerminateEvent += value; }
			remove { _progressTerminateEvent -= value; }
		}

		/// <summary>
		/// Gets the error that has occurred while running the task.
		/// </summary>
		public Exception TaskException
		{
			get { return _exception; }
		}

		/// <summary>
		/// Starts the <see cref="BackgroundTask"/> and shows the progress dialog.
		/// </summary>
        public override void Start()
        {
            if (_task != null)
            {
                _task.ProgressUpdated += ProgressHandler;
                _task.Terminated += TerminatedHandler;

                if (_task.IsRunning)
                    UpdateProgress(_task.LastBackgroundTaskProgress);
                else
                    _task.Run();                    
            }

            base.Start();
        }

		/// <summary>
		/// Does nothing unless the task has completed; closes the progress dialog.
		/// </summary>
        public override void Stop()
        {
            // wait for the background task to stop running
            if (_task != null && _task.IsRunning)
                return;

            _task.ProgressUpdated -= ProgressHandler;
            _task.Terminated -= TerminatedHandler;
            base.Stop();
        }

        /// <summary>
        /// Override implementation of <see cref="IApplicationComponent.CanExit"/>.
        /// </summary>
        /// <remarks>
		/// This is called when the user clicks on the 'X' button to close the dialog.
		/// </remarks>
		/// <returns>True only if the task has finished running.</returns>
        public override bool CanExit()
        {
            return _task == null || !_task.IsRunning;
        }

        /// <summary>
        /// Called by the framework in the case where the host has initiated the exit, rather than the component,
        /// to give the component a chance to prepare prior to being stopped.
        /// </summary>
        /// <returns>
        /// False if the task is still running, otherwise true.
        /// </returns>
        public override bool PrepareExit()
        {
            // if task is running, the component cannot exit
            if (_task != null && _task.IsRunning)
            {
                if (_task.SupportsCancel)
                {
                    if (this.Host.DesktopWindow.ShowMessageBox(SR.MessageConfirmCancelTask, MessageBoxActions.OkCancel) == DialogBoxAction.Ok)
                    {
                        _task.RequestCancel();
                        _autoClose = true;

                        // even though the user has cancelled the task, we don't return true
                        // because we don't want to allow this component to exit until the _task 
                        // has actually stopped
                    }
                }

                // Don't allow the dialog to close until the task stop running or is properly cancelled
                return false;
            }

            return true;
        }

        #region Presentation Model

		/// <summary>
		/// Gets whether or not a 'Cancel' button should be shown.
		/// </summary>
        public bool ShowCancel
        {
            get 
            {
                // Always show Cancel if the task is cancellable
                if (_enableCancel)
                    return true;

                // Otherwise only show Cancel if the task stops running
                return (_task != null && _task.IsRunning ? false : true); 
            }
        }

		/// <summary>
		/// Returns 100.
		/// </summary>
        public int ProgressBarMaximum
        {
            get { return 100; }
        }

		/// <summary>
		/// Geths the current position of the progress bar.
		/// </summary>
        public int ProgressBar
        {
            get { return _progressBar; }
        }

		/// <summary>
		/// Gets the style of the progress bar.
		/// </summary>
        public ProgressBarStyle ProgressBarStyle
        {
            get { return _progressBarStyle; }
        }

		/// <summary>
		/// Gets a number indicating the speed of the progress bar when in Marquee mode.
		/// </summary>
        public int MarqueeSpeed
        {
            get { return _marqueeSpeed; }
        }

		/// <summary>
		/// Gets the current progress message.
		/// </summary>
        public string ProgressMessage
        {
            get { return _progressMessage; }
        }

		/// <summary>
		/// "Close" if the task has completed, "Cancel" if it is still running.
		/// </summary>
        public string ButtonText
        {
            get { return (_task != null && _task.IsRunning ? SR.TitleCancel : SR.TitleClose); }
        }

        #endregion

        #region Task EventHandler

        private void UpdateProgress(BackgroundTaskProgressEventArgs e)
        {
            if (e == null)
                return;

            _progressMessage = e.Progress.Message;
            _progressBar = e.Progress.Percent;
            SignalProgressUpdate();            
        }

        private void ProgressHandler(object sender, BackgroundTaskProgressEventArgs e)
        {
            UpdateProgress(e);
        }

        private void TerminatedHandler(object sender, BackgroundTaskTerminatedEventArgs e)
        {
            if (_autoClose && e.Reason != BackgroundTaskTerminatedReason.Exception)
            {
                this.ExitCode = ApplicationComponentExitCode.None;
                Host.Exit();
            }
            else
            {
                _marqueeSpeed = 0;
                _enableCancel = true;
                
                switch (e.Reason)
                {
                    case BackgroundTaskTerminatedReason.Exception:
                        _exception = e.Exception;                        
                        this.ExitCode = ApplicationComponentExitCode.Error;
                        Host.Exit();

                        break;                        
                    case BackgroundTaskTerminatedReason.Completed:
                    case BackgroundTaskTerminatedReason.Cancelled:
                    default:
                        if (this.ProgressBarStyle == ProgressBarStyle.Marquee)
                        {
                            // Make the progress bar looks 'full' at completion
                            _progressBarStyle = ProgressBarStyle.Blocks;
                            _progressBar = this.ProgressBarMaximum;
                        }
                        break;
                }
                
                SignalProgressTerminate();
            }
        }

        #endregion

        #region Signal Model Changed

        private void SignalProgressUpdate()
        {
            EventsHelper.Fire(_progressUpdateEvent, this, new EventArgs());
        }

        private void SignalProgressTerminate()
        {
            EventsHelper.Fire(_progressTerminateEvent, this, new EventArgs());
        }

        #endregion

		/// <summary>
		/// Called from the view to indicate that the task should be cancelled.
		/// </summary>
        public void Cancel()
        {
            if (_task != null && _task.IsRunning)
            {
                if (_task.SupportsCancel)
                {
                    _task.RequestCancel();
                    _autoClose = true;
                }
            }
            else
            {
                this.ExitCode = ApplicationComponentExitCode.None;
                Host.Exit();
            }
        }
    }
}
