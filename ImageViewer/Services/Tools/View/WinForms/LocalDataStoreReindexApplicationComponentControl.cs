#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Common.LocalDataStore;
using MessageBox=ClearCanvas.Desktop.View.WinForms.MessageBox;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="LocalDataStoreReindexApplicationComponent"/>
    /// </summary>
	public partial class LocalDataStoreReindexApplicationComponentControl : ApplicationComponentUserControl
    {
    	private readonly ILocalDataStoreReindexer _reindexer;

        /// <summary>
        /// Constructor
        /// </summary>
        public LocalDataStoreReindexApplicationComponentControl(ILocalDataStoreReindexer reindexer)
        {
            InitializeComponent();

			_reindexer = reindexer;

			_reindexProgressControl.DataBindings.Add("StatusMessage", _reindexer, "StatusMessage", true, DataSourceUpdateMode.OnPropertyChanged);
			_reindexProgressControl.DataBindings.Add("TotalToProcess", _reindexer, "TotalToProcess", true, DataSourceUpdateMode.OnPropertyChanged);
			_reindexProgressControl.DataBindings.Add("TotalProcessed", _reindexer, "TotalProcessed", true, DataSourceUpdateMode.OnPropertyChanged);
			_reindexProgressControl.DataBindings.Add("AvailableCount", _reindexer, "AvailableCount", true, DataSourceUpdateMode.OnPropertyChanged);
			_reindexProgressControl.DataBindings.Add("FailedSteps", _reindexer, "FailedSteps", true, DataSourceUpdateMode.OnPropertyChanged);

        	UpdateButtonText();
        	_reindexer.PropertyChanged += OnPropertyChanged;
			_reindexProgressControl.ButtonClicked += OnReindexButtonClicked;
		}

		public bool StartEnabled { get; set; }

		private void OnReindexButtonClicked(object sender, EventArgs e)
		{
			if (_reindexer.CanCancel)
			{
				MessageBox box = new MessageBox();
				if (box.Show(SR.MessageConfirmCancelReindex, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
					_reindexer.Cancel();
			}
			else if (_reindexer.CanStart)
			{
				_reindexer.Start();
			}
		}

		private void UpdateButtonText()
		{
			if (_reindexer.CanCancel)
			{
				_reindexProgressControl.ButtonText = SR.LabelCancel;
				_reindexProgressControl.ButtonEnabled = true;
			}
			else if (_reindexer.CanStart)
			{
				_reindexProgressControl.ButtonEnabled = StartEnabled;
				_reindexProgressControl.ButtonText = SR.LabelStart;
			}
			else
			{
				_reindexProgressControl.ButtonEnabled = false;
			}
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "CanCancel" || e.PropertyName == "CanStart")
			{
				UpdateButtonText();
			}
		}
    }
}
