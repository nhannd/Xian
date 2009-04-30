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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.ImageViewer.Tools.Volume;

namespace ClearCanvas.ImageViewer.Tools.Volume.VTK.View.WinForms
{
	public partial class TissueControl : UserControl
	{
		private TissueSettings _tissueSettings;
		private BindingSource _bindingSource;

		public TissueControl()
		{
			InitializeComponent();

			_bindingSource = new BindingSource();
			_presetComboBox.DataSource = TissueSettings.Presets;
			_presetComboBox.SelectedValueChanged += new EventHandler(OnPresetChanged);
			_surfaceRenderingRadio.Click += new EventHandler(OnSurfaceRenderingRadioClick);
			_volumeRenderingRadio.Click += new EventHandler(OnVolumeRenderingRadioClick);
		}

		void OnVolumeRenderingRadioClick(object sender, EventArgs e)
		{
			_volumeRenderingRadio.Checked = true;
		}

		void OnSurfaceRenderingRadioClick(object sender, EventArgs e)
		{
			_surfaceRenderingRadio.Checked = true;
		}

		public TissueSettings TissueSettings
		{
			get { return _tissueSettings; }
			set
			{
				if (_tissueSettings != value)
				{
					_tissueSettings = value;
					UpdateBindings();
				}
			}
		}

		private void UpdateBindings()
		{
			_bindingSource.Clear();
			_bindingSource.DataSource = _tissueSettings;

			_visibleCheckBox.DataBindings.Clear();
			_visibleCheckBox.DataBindings.Add("Checked", _bindingSource, "Visible", true, DataSourceUpdateMode.OnPropertyChanged);

			_surfaceRenderingRadio.DataBindings.Clear();
			_surfaceRenderingRadio.DataBindings.Add("Enabled", _bindingSource, "Visible", true, DataSourceUpdateMode.OnPropertyChanged);
			_surfaceRenderingRadio.DataBindings.Add("Checked", _bindingSource, "SurfaceRenderingSelected", true, DataSourceUpdateMode.OnPropertyChanged);

			_volumeRenderingRadio.DataBindings.Clear();
			_volumeRenderingRadio.DataBindings.Add("Enabled", _bindingSource, "Visible", true, DataSourceUpdateMode.OnPropertyChanged);
			_volumeRenderingRadio.DataBindings.Add("Checked", _bindingSource, "VolumeRenderingSelected", true, DataSourceUpdateMode.OnPropertyChanged);

			_opacityControl.DataBindings.Clear();
			_opacityControl.DataBindings.Add("Enabled", _bindingSource, "Visible", true, DataSourceUpdateMode.OnPropertyChanged);
			_opacityControl.DataBindings.Add("Minimum", _bindingSource, "MinimumOpacity", true, DataSourceUpdateMode.OnPropertyChanged);
			_opacityControl.DataBindings.Add("Maximum", _bindingSource, "MaximumOpacity", true, DataSourceUpdateMode.OnPropertyChanged);
			_opacityControl.DataBindings.Add("Value", _bindingSource, "Opacity", true, DataSourceUpdateMode.OnPropertyChanged);

			_windowControl.DataBindings.Clear();
			_windowControl.DataBindings.Add("Enabled", _bindingSource, "WindowEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_windowControl.DataBindings.Add("Minimum", _bindingSource, "MinimumWindow", true, DataSourceUpdateMode.OnPropertyChanged);
			_windowControl.DataBindings.Add("Maximum", _bindingSource, "MaximumWindow", true, DataSourceUpdateMode.OnPropertyChanged);
			_windowControl.DataBindings.Add("Value", _bindingSource, "Window", true, DataSourceUpdateMode.OnPropertyChanged);

			_levelControl.DataBindings.Clear();
			_levelControl.DataBindings.Add("Enabled", _bindingSource, "Visible", true, DataSourceUpdateMode.OnPropertyChanged);
			_levelControl.DataBindings.Add("Minimum", _bindingSource, "MinimumLevel", true, DataSourceUpdateMode.OnPropertyChanged);
			_levelControl.DataBindings.Add("Maximum", _bindingSource, "MaximumLevel", true, DataSourceUpdateMode.OnPropertyChanged);
			_levelControl.DataBindings.Add("Value", _bindingSource, "Level", true, DataSourceUpdateMode.OnPropertyChanged);

			_presetComboBox.DataBindings.Clear();
			_presetComboBox.DataBindings.Add("Enabled", _bindingSource, "Visible", true, DataSourceUpdateMode.OnPropertyChanged);
			_presetComboBox.DataBindings.Add("SelectedItem", _bindingSource, "SelectedPreset", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		void OnPresetChanged(object sender, EventArgs e)
		{
			_tissueSettings.SelectPreset(_presetComboBox.SelectedItem.ToString());
		}
	}
}
