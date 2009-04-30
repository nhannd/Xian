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
using ClearCanvas.ImageViewer.Clipboard.ImageExport;

namespace ClearCanvas.ImageViewer.Clipboard.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="AviExportComponent"/>.
    /// </summary>
    public partial class AviExportComponentControl : ApplicationComponentUserControl
    {
        private AviExportComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AviExportComponentControl(AviExportComponent component)
            :base(component)
        {
            InitializeComponent();
			_component = component;

        	base.AcceptButton = _buttonOk;
        	base.CancelButton = _buttonCancel;

			_trackBarFrameRate.DataBindings.Add("Minimum", _component, "MinFrameRate", true, DataSourceUpdateMode.OnPropertyChanged);
			_trackBarFrameRate.DataBindings.Add("Maximum", _component, "MaxFrameRate", true, DataSourceUpdateMode.OnPropertyChanged);
			_trackBarFrameRate.DataBindings.Add("Value", _component, "FrameRate", true, DataSourceUpdateMode.OnPropertyChanged);
			Binding binding = new Binding("Text", _component, "FrameRate", true, DataSourceUpdateMode.OnPropertyChanged);
			binding.Format += new ConvertEventHandler(OnFormatFrameRate);
			_frameRate.DataBindings.Add(binding);
			
			binding = new Binding("Text", _component, "DurationSeconds", true, DataSourceUpdateMode.OnPropertyChanged);
			binding.Format += new ConvertEventHandler(OnFormatDuration);
        	_duration.DataBindings.Add(binding);

			_checkOptionWysiwyg.DataBindings.Add("Checked", _component, "OptionWysiwyg", true, DataSourceUpdateMode.OnPropertyChanged);
			_checkOptionCompleteImage.DataBindings.Add("Checked", _component, "OptionCompleteImage", true, DataSourceUpdateMode.OnPropertyChanged);

			_scale.DataBindings.Add("Maximum", _component, "MaximumScale", true, DataSourceUpdateMode.OnPropertyChanged);
			_scale.DataBindings.Add("Minimum", _component, "MinimumScale", true, DataSourceUpdateMode.OnPropertyChanged);
			_scale.DataBindings.Add("Value", _component, "Scale", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void OnFormatFrameRate(object sender, ConvertEventArgs e)
		{
			if (e.DesiredType != typeof(string))
				return;

			e.Value = String.Format("({0})", (int)e.Value);
		}

		private void OnFormatDuration(object sender, ConvertEventArgs e)
		{
			if (e.DesiredType != typeof(string))
				return;

			e.Value = String.Format(((float) e.Value).ToString("F2"));
		}

		private void OnAdvanced(object sender, EventArgs e)
		{
			_component.ShowAdvanced();
		}

		private void OnCancel(object sender, EventArgs e)
		{
			_component.Cancel();
		}

		private void OnOk(object sender, EventArgs e)
		{
			using (SaveFileDialog dialog = new SaveFileDialog())
			{
				dialog.Filter = "Avi Files|*.avi;";
				dialog.DefaultExt = "avi";
				dialog.AddExtension = true;
				dialog.RestoreDirectory = true;

				if (DialogResult.OK == dialog.ShowDialog())
				{
					_component.FilePath = dialog.FileName;
					_component.Accept();
				}
			}
		}
	}
}
