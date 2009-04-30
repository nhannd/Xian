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
    public partial class AviExportAdvancedComponentControl : ApplicationComponentUserControl
    {
        private AviExportAdvancedComponent _component;

        public AviExportAdvancedComponentControl(AviExportAdvancedComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

			base.AcceptButton = _buttonOk;
			base.CancelButton = _buttonCancel;
			
			_checkUseDefaultQuality.DataBindings.Add("Checked", _component, "UseDefaultQuality", true, DataSourceUpdateMode.OnPropertyChanged);
			_checkUseDefaultQuality.DataBindings.Add("Enabled", _component, "UseDefaultQualityEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

        	_comboCodec.DataSource = _component.CodecInfoList;
			_comboCodec.DisplayMember = "Description";
			_comboCodec.DataBindings.Add("Value", _component, "SelectedCodecInfo", true, DataSourceUpdateMode.OnPropertyChanged);

			_trackBarQuality.DataBindings.Add("Enabled", _component, "QualityEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_trackBarQuality.DataBindings.Add("Minimum", _component, "MinQuality", true, DataSourceUpdateMode.OnPropertyChanged);
			_trackBarQuality.DataBindings.Add("Maximum", _component, "MaxQuality", true, DataSourceUpdateMode.OnPropertyChanged);
			_trackBarQuality.DataBindings.Add("Value", _component, "Quality", true, DataSourceUpdateMode.OnPropertyChanged);

			_quality.DataBindings.Add("Enabled", _component, "QualityEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			Binding binding = new Binding("Text", _component, "Quality", true, DataSourceUpdateMode.OnPropertyChanged);
			binding.Format += new ConvertEventHandler(OnFormatQuality);
			_quality.DataBindings.Add(binding);
        }

		private void OnFormatQuality(object sender, ConvertEventArgs e)
		{
			if (e.DesiredType != typeof(string))
				return;

			e.Value = String.Format("({0})", (int)e.Value);
		}

		private void OnOk(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void OnCancel(object sender, EventArgs e)
		{
			_component.Cancel();
		}
    }
}
