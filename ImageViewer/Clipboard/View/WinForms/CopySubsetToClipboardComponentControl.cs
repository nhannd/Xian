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
using ClearCanvas.ImageViewer.Clipboard.CopyToClipboard;

namespace ClearCanvas.ImageViewer.Clipboard.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="CopySubsetToClipboardComponent"/>.
    /// </summary>
    public partial class CopySubsetToClipboardComponentControl : ApplicationComponentUserControl
    {
        private CopySubsetToClipboardComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CopySubsetToClipboardComponentControl(CopySubsetToClipboardComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

        	base.AcceptButton = _sendToClipboardButton;

			_sourceDisplaySet.DataBindings.Add("Text", _component, "SourceDisplaySetDescription", true, DataSourceUpdateMode.OnPropertyChanged);

			_radioUseInstanceNumber.DataBindings.Add("Checked", _component, "UseInstanceNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_radioUseInstanceNumber.DataBindings.Add("Enabled", _component, "UseInstanceNumberEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_radioUsePositionNumber.DataBindings.Add("Checked", _component, "UsePositionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_radioUsePositionNumber.DataBindings.Add("Enabled", _component, "UsePositionNumberEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			
			_radioCopyRange.DataBindings.Add("Checked", _component, "CopyRange", true, DataSourceUpdateMode.OnPropertyChanged);
			_radioCopyRange.DataBindings.Add("Enabled", _component, "CopyRangeEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_radioCopyCustom.DataBindings.Add("Checked", _component, "CopyCustom", true, DataSourceUpdateMode.OnPropertyChanged);
			_radioCopyCustom.DataBindings.Add("Enabled", _component, "CopyCustomEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_radioCopyRangeAll.DataBindings.Add("Checked", _component, "CopyRangeAll", true, DataSourceUpdateMode.OnPropertyChanged);
			_radioCopyRangeAll.DataBindings.Add("Enabled", _component, "CopyRangeAllEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_radioCopyRangeAtInterval.DataBindings.Add("Checked", _component, "CopyRangeAtInterval", true, DataSourceUpdateMode.OnPropertyChanged);
			_radioCopyRangeAtInterval.DataBindings.Add("Enabled", _component, "CopyRangeAtIntervalEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_copyRangeStart.DataBindings.Add("Minimum", _component, "RangeMinimum", true, DataSourceUpdateMode.OnPropertyChanged);
			_copyRangeStart.DataBindings.Add("Maximum", _component, "CopyRangeEnd", true, DataSourceUpdateMode.OnPropertyChanged);
			_copyRangeStart.DataBindings.Add("Value", _component, "CopyRangeStart", true, DataSourceUpdateMode.OnPropertyChanged);
			_copyRangeStart.DataBindings.Add("Enabled", _component, "CopyRangeStartEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_copyRangeEnd.DataBindings.Add("Minimum", _component, "CopyRangeStart", true, DataSourceUpdateMode.OnPropertyChanged);
			_copyRangeEnd.DataBindings.Add("Maximum", _component, "RangeMaximum", true, DataSourceUpdateMode.OnPropertyChanged);
			_copyRangeEnd.DataBindings.Add("Value", _component, "CopyRangeEnd", true, DataSourceUpdateMode.OnPropertyChanged);
			_copyRangeEnd.DataBindings.Add("Enabled", _component, "CopyRangeEndEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_copyRangeInterval.DataBindings.Add("Minimum", _component, "RangeMinInterval", true, DataSourceUpdateMode.OnPropertyChanged);
			_copyRangeInterval.DataBindings.Add("Maximum", _component, "RangeMaxInterval", true, DataSourceUpdateMode.OnPropertyChanged);
			_copyRangeInterval.DataBindings.Add("Value", _component, "CopyRangeInterval", true, DataSourceUpdateMode.OnPropertyChanged);
			_copyRangeInterval.DataBindings.Add("Enabled", _component, "CopyRangeIntervalEnabled", true, DataSourceUpdateMode.OnPropertyChanged); 

			_customRange.DataBindings.Add("Text", _component, "CustomRange", true, DataSourceUpdateMode.OnPropertyChanged);
			_customRange.DataBindings.Add("Enabled", _component, "CustomRangeEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_sendToClipboardButton.DataBindings.Add("Enabled", _component, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void OnSendToClipboard(object sender, EventArgs e)
		{
			_component.CopyToClipboard();
		}

		private void OnRangeCopyAllImagesCheckedChanged(object sender, EventArgs e)
		{
			_copyRangeInterval.Enabled = _component.CopyRangeAtInterval;
		}

		private void OnRangeCopyAtIntervalCheckedChanged(object sender, EventArgs e)
		{
			_copyRangeInterval.Enabled = _component.CopyRangeAtInterval;
		}
	}
}
