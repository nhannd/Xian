using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
	public partial class ImportProgressControl : ApplicationComponentUserControl
	{
		private event EventHandler _cancelButtonClicked;

		public ImportProgressControl()
		{
			InitializeComponent();
		}

		public event EventHandler CancelButtonClicked
		{
			add { _cancelButtonClicked += value; }
			remove { _cancelButtonClicked -= value; }
		}

		public string StatusMessage
		{
			get
			{
				return _statusMessage.Text;
			}
			set
			{
				_statusMessage.Text = value;
			}
		}

		public int TotalToProcess
		{
			get 
			{
				return _processedProgress.Maximum;
			}
			set
			{
				_processedProgress.Maximum = value;
				_processedCount.Text = _processedProgress.Maximum.ToString();
			}
		}

		public int TotalProcessed
		{
			get 
			{
				return _processedProgress.Value;
			}
			set
			{
				_processedProgress.Value = value;
			}
		}

		public int AvailableCount
		{
			get
			{
				if (String.IsNullOrEmpty(_availableCount.Text))
					return 0;

				return Convert.ToInt32(_availableCount.Text);
			}
			set
			{
				_availableCount.Text = value.ToString();
			}
		}

		public int FailedSteps
		{
			get
			{
				if (String.IsNullOrEmpty(_failedCount.Text))
					return 0;
				
				return Convert.ToInt32(_failedCount.Text);
			}
			set
			{
				_failedCount.Text = value.ToString();
			}
		}

		public bool CancelEnabled
		{
			get { return _cancelButton.Enabled; }
			set { _cancelButton.Enabled = value; }
		}

		private void OnCancelButtonClicked(object sender, EventArgs e)
		{
			EventsHelper.Fire(_cancelButtonClicked, this, EventArgs.Empty);
		}
	}
}
