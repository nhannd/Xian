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
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ProgressDialogComponent"/>
    /// </summary>
    public partial class ProgressDialogComponentControl : ApplicationComponentUserControl
    {
        private ProgressDialogComponent _component;
        private int _defaultProgressBarWidth;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProgressDialogComponentControl(ProgressDialogComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

			base.CancelButton = _cancelButton;
			base.AcceptButton = _cancelButton;

            _cancelButton.Visible = _component.ShowCancel;
            _cancelButton.Text = _component.ButtonText;
            _message.Text = _component.ProgressMessage;
            _progressBar.Value = _component.ProgressBar;
            _progressBar.MarqueeAnimationSpeed = _component.MarqueeSpeed;
            _progressBar.Style = (System.Windows.Forms.ProgressBarStyle)_component.ProgressBarStyle;
            _progressBar.Maximum = _component.ProgressBarMaximum;

            _component.ProgressUpdateEvent += OnProgressUpdate;
            _component.ProgressTerminateEvent += OnProgressTerminate;

            _defaultProgressBarWidth = _progressBar.Width;
            UpdateProgressBarLength();
        }

        ~ProgressDialogComponentControl()
        {
            _component.ProgressUpdateEvent -= OnProgressUpdate;
            _component.ProgressTerminateEvent -= OnProgressTerminate;
        }

        private void OnProgressUpdate(object sender, EventArgs e)
        {
            _message.Text = _component.ProgressMessage;
            _progressBar.Value = _component.ProgressBar;
        }

        private void OnProgressTerminate(object sender, EventArgs e)
        {
            _cancelButton.Visible = _component.ShowCancel;
            _cancelButton.Text = _component.ButtonText;
            _message.Text = _component.ProgressMessage;
            _progressBar.Value = _component.ProgressBar;
            _progressBar.Style = (System.Windows.Forms.ProgressBarStyle)_component.ProgressBarStyle;
            _progressBar.MarqueeAnimationSpeed = _component.MarqueeSpeed;

            UpdateProgressBarLength();
        }

        private void UpdateProgressBarLength()
        {
            if (_cancelButton.Visible)
                _progressBar.Width = _defaultProgressBarWidth;
            else
                _progressBar.Width = _cancelButton.Right - _progressBar.Left;
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

        private void _message_Enter(object sender, EventArgs e)
        {
            // Give the current focus to the next control upon entering, so the caret never shows up
            Control nextControl = Parent.GetNextControl(_message, true);
            if (nextControl != null)
                nextControl.Focus();
        }
    }
}
