using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ProgressDialogComponent"/>
    /// </summary>
    public partial class ProgressDialogComponentControl : ApplicationComponentUserControl
    {
        private ProgressDialogComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProgressDialogComponentControl(ProgressDialogComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _message.DataBindings.Add("Text", _component, "ProgressMessage", true, DataSourceUpdateMode.OnPropertyChanged);
            _progressBar.DataBindings.Add("Value", _component, "ProgressBar", true, DataSourceUpdateMode.OnPropertyChanged);
            _progressBar.DataBindings.Add("Maximum", _component, "ProgressBarMaximum", true, DataSourceUpdateMode.OnPropertyChanged);
            _cancelButton.DataBindings.Add("Enabled", _component, "EnableCancel", true, DataSourceUpdateMode.OnPropertyChanged);
            _cancelButton.DataBindings.Add("Text", _component, "ButtonText", true, DataSourceUpdateMode.OnPropertyChanged);
            
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
