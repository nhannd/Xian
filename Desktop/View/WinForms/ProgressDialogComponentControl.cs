#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
