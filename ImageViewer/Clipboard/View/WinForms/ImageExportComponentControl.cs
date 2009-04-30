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
    /// Provides a Windows Forms user-interface for <see cref="ImageExportComponent"/>.
    /// </summary>
    public partial class ImageExportComponentControl : ApplicationComponentUserControl
    {
        private ImageExportComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ImageExportComponentControl(ImageExportComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

        	base.CancelButton = _buttonCancel;
        	base.AcceptButton = _buttonOk;

			_imageExporters.DataSource = _component.ExporterInfoList;
        	_imageExporters.DisplayMember = "Description";
			_imageExporters.DataBindings.Add("Value", _component, "SelectedExporterInfo", true, DataSourceUpdateMode.OnPropertyChanged);
			
			_checkOptionWysiwyg.DataBindings.Add("Checked", _component, "OptionWysiwyg", true, DataSourceUpdateMode.OnPropertyChanged);
			_checkOptionCompleteImage.DataBindings.Add("Checked", _component, "OptionCompleteImage", true, DataSourceUpdateMode.OnPropertyChanged);

			_scale.DataBindings.Add("Maximum", _component, "MaximumScale", true, DataSourceUpdateMode.OnPropertyChanged);
			_scale.DataBindings.Add("Minimum", _component, "MinimumScale", true, DataSourceUpdateMode.OnPropertyChanged);
			_scale.DataBindings.Add("Value", _component, "Scale", true, DataSourceUpdateMode.OnPropertyChanged);

			_buttonConfigure.DataBindings.Add("Visible", _component, "ConfigureVisible", true, DataSourceUpdateMode.OnPropertyChanged);
			_buttonConfigure.DataBindings.Add("Enabled", _component, "ConfigureEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void OnConfigureExporter(object sender, EventArgs e)
		{
			_component.Configure();
		}

		private void OnOk(object sender, EventArgs e)
		{
			if (_component.NumberOfImagesToExport > 1)
			{
				using (FolderBrowserDialog dialog = new FolderBrowserDialog())
				{
					if (DialogResult.OK == dialog.ShowDialog())
						_component.ExportFilePath = dialog.SelectedPath;
					else
						return;
				}
			}
			else
			{
				using (SaveFileDialog dialog = new SaveFileDialog())
				{
					dialog.Filter = _component.SelectedExporterInfo.FileExtensionFilter;
					dialog.DefaultExt = _component.SelectedExporterInfo.DefaultFileExtension;
					dialog.RestoreDirectory = true;
					dialog.AddExtension = true;

					if (DialogResult.OK == dialog.ShowDialog())
						_component.ExportFilePath = dialog.FileName;
					else
						return;
				}
			}

			_component.Accept();
		}

		private void OnCancel(object sender, EventArgs e)
		{
			_component.Cancel();
		}
    }
}
