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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Extension point for views onto <see cref="ProgressDialogComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ProgressDialogComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ProgressDialogComponent class
    /// </summary>
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
        public event EventHandler<EventArgs> ProgressUpdateEvent
        {
            add { _progressUpdateEvent += value; }
            remove { _progressUpdateEvent -= value; }
        }
        public event EventHandler<EventArgs> ProgressTerminateEvent
        {
            add { _progressTerminateEvent += value; }
            remove { _progressTerminateEvent -= value; }
        }

        public Exception TaskException
        {
            get { return _exception; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
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

        public override void Start()
        {
            if (_task != null)
            {
                _task.ProgressUpdated += ProgressHandler;
                _task.Terminated += TerminatedHandler;
                _task.Run();
            }

            base.Start();
        }

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
        /// This is called when user click on the 'X' button to close the dialog
        /// </summary>
        public override bool CanExit(UserInteraction interactive)
        {
            // 
            if (_task != null && _task.IsRunning)
            {
                if (_task.SupportsCancel && interactive == UserInteraction.Allowed)
                {
                    if (this.Host.DesktopWindow.ShowMessageBox(SR.MessageConfirmCancelTask, MessageBoxActions.OkCancel) == DialogBoxAction.Ok)
                    {
                        _task.RequestCancel();
                        _autoClose = true;
                    }
                }

                // Don't allow the dialog to close until the task stop running or is properly cancelled
                return false;
            }

            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            return true;
        }

        #region Presentation Model

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

        public int ProgressBarMaximum
        {
            get { return 100; }
        }

        public int ProgressBar
        {
            get { return _progressBar; }
        }

        public ProgressBarStyle ProgressBarStyle
        {
            get { return _progressBarStyle; }
        }

        public int MarqueeSpeed
        {
            get { return _marqueeSpeed; }
        }

        public string ProgressMessage
        {
            get { return _progressMessage; }
        }

        public string ButtonText
        {
            get { return (_task != null && _task.IsRunning ? "Cancel" : "Close"); }
        }

        #endregion

        #region Task EventHandler

        private void ProgressHandler(object sender, BackgroundTaskProgressEventArgs e)
        {
            _progressMessage = e.Progress.Message;
            _progressBar = e.Progress.Percent;
            SignalProgressUpdate();
        }

        private void TerminatedHandler(object sender, BackgroundTaskTerminatedEventArgs e)
        {
            if (_autoClose && e.Reason != BackgroundTaskTerminatedReason.Exception)
            {
                this.ExitCode = ApplicationComponentExitCode.Cancelled;
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
                this.ExitCode = ApplicationComponentExitCode.Cancelled;
                Host.Exit();
            }
        }
    }
}
