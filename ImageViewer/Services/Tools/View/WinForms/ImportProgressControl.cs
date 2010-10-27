#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
		public ImportProgressControl()
		{
			InitializeComponent();
		}

		public event EventHandler ButtonClicked
		{
			add { _button.Click += value; }
			remove { _button.Click -= value; }
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

		public string ButtonText
		{
			get { return _button.Text; }
			set { _button.Text = value; }
		}

		public bool ButtonEnabled
		{
			get { return _button.Enabled; }
			set { _button.Enabled = value; }
		}
	}
}
