#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using MessageBox=ClearCanvas.Desktop.View.WinForms.MessageBox;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="LocalDataStoreReindexApplicationComponent"/>
    /// </summary>
	public partial class LocalDataStoreReindexApplicationComponentControl : ApplicationComponentUserControl
    {
    	private readonly IReindexLocalDataStore _reindexer;

        /// <summary>
        /// Constructor
        /// </summary>
        public LocalDataStoreReindexApplicationComponentControl(IReindexLocalDataStore reindexer)
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
