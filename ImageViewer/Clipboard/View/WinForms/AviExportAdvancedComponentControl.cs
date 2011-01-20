#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
