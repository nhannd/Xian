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
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DiskspaceManagerConfigurationComponent"/>
    /// </summary>
    public partial class DiskspaceManagerConfigurationComponentControl : ApplicationComponentUserControl
    {
        private DiskspaceManagerConfigurationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DiskspaceManagerConfigurationComponentControl(DiskspaceManagerConfigurationComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;
			
			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component;

        	_component.AllPropertiesChanged += delegate { bindingSource.ResetBindings(false); };

			_txtDriveName.DataBindings.Add("Text", bindingSource, "DriveDisplay", true, DataSourceUpdateMode.OnPropertyChanged);

			Binding lowWatermarkBinding = new Binding("Value", bindingSource, "LowWatermark", true, DataSourceUpdateMode.OnPropertyChanged);
			Binding highWatermarkBinding = new Binding("Value", bindingSource, "HighWatermark", true, DataSourceUpdateMode.OnPropertyChanged);
			lowWatermarkBinding.Parse += new ConvertEventHandler(ParseWatermarkBinding);
			highWatermarkBinding.Parse += new ConvertEventHandler(ParseWatermarkBinding);
			lowWatermarkBinding.Format += new ConvertEventHandler(FormatWatermarkBinding);
			highWatermarkBinding.Format += new ConvertEventHandler(FormatWatermarkBinding);

			_txtLowWatermarkBytesDisplay.DataBindings.Add("Text", bindingSource, "LowWaterMarkBytesDisplay", true, DataSourceUpdateMode.OnPropertyChanged);
			_upDownLowWatermark.DataBindings.Add("Value", bindingSource, "LowWaterMark", true, DataSourceUpdateMode.OnPropertyChanged);
			_tbLowWatermark.DataBindings.Add(lowWatermarkBinding);

			_txtHighWatermarkBytesDisplay.DataBindings.Add("Text", bindingSource, "HighWaterMarkBytesDisplay", true, DataSourceUpdateMode.OnPropertyChanged);
			_upDownHighWatermark.DataBindings.Add("Value", bindingSource, "HighWaterMark", true, DataSourceUpdateMode.OnPropertyChanged);
			_tbHighWatermark.DataBindings.Add(highWatermarkBinding);

			_pbUsedSpace.DataBindings.Add("Value", bindingSource, "SpaceUsedPercent", true, DataSourceUpdateMode.OnPropertyChanged);
			_txtUsedSpace.DataBindings.Add("Text", bindingSource, "SpaceUsedPercentDisplay", true, DataSourceUpdateMode.OnPropertyChanged);
			_txtUsedSpaceBytesDisplay.DataBindings.Add("Text", bindingSource, "SpaceUsedBytesDisplay", true, DataSourceUpdateMode.OnPropertyChanged);
			
			_txtFrequency.DataBindings.Add("Text", bindingSource, "CheckFrequency", true, DataSourceUpdateMode.OnPropertyChanged);
			_tbFrequency.DataBindings.Add("Value", bindingSource, "CheckFrequency", true, DataSourceUpdateMode.OnPropertyChanged);
			_tbFrequency.DataBindings.Add("Minimum", bindingSource, "MinimumCheckFrequency", true, DataSourceUpdateMode.OnPropertyChanged);
			_tbFrequency.DataBindings.Add("Maximum", bindingSource, "MaximumCheckFrequency", true, DataSourceUpdateMode.OnPropertyChanged);

            _tbLowWatermark.DataBindings.Add("Enabled", bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _tbHighWatermark.DataBindings.Add("Enabled", bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _tbFrequency.DataBindings.Add("Enabled", bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _pbUsedSpace.DataBindings.Add("Enabled", bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_upDownLowWatermark.DataBindings.Add("Enabled", bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_upDownHighWatermark.DataBindings.Add("Enabled", bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);

        	_enforceStudyLimit.DataBindings.Add("Checked", bindingSource, "EnforceStudyLimit", true, DataSourceUpdateMode.OnPropertyChanged);
        	_enforceStudyLimit.DataBindings.Add("Enabled", bindingSource, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_studyCount.DataBindings.Add("Text", bindingSource, "StudyCountText", true, DataSourceUpdateMode.OnPropertyChanged);

			_studyLimit.DataBindings.Add("Value", bindingSource, "StudyLimit", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyLimit.DataBindings.Add("Enabled", bindingSource, "EnforceStudyLimit", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyLimit.DataBindings.Add("Maximum", bindingSource, "MaxStudyLimit", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyLimit.DataBindings.Add("Minimum", bindingSource, "MinStudyLimit", true, DataSourceUpdateMode.OnPropertyChanged);

			_bnRefresh.Click += OnRefreshClick;
        }

		private void FormatWatermarkBinding(object sender, ConvertEventArgs e)
		{
			float value = (float)e.Value;
			e.Value = (int)(value * 1000F);
		}

		private void ParseWatermarkBinding(object sender, ConvertEventArgs e)
		{
			int value = (int)e.Value;
			e.Value = value / 1000F;
		}

        private void OnRefreshClick(object sender, EventArgs e)
        {
            _component.Refresh();
        }
    }
}
