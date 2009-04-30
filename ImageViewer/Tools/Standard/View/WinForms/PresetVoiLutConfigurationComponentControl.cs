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
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PresetVoiLutConfigurationComponent"/>
    /// </summary>
	public partial class PresetVoiLutConfigurationComponentControl : ClearCanvas.Desktop.View.WinForms.ApplicationComponentUserControl
    {
        private readonly PresetVoiLutConfigurationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PresetVoiLutConfigurationComponentControl(PresetVoiLutConfigurationComponent component)
            :base(component)
        {
			_component = component;
			
			InitializeComponent();

			BindingSource source = new BindingSource();
        	source.DataSource = _component;

			OnAddPresetVisibleChanged(null, EventArgs.Empty);
			
			_presetVoiLuts.Table = _component.VoiLutPresets;
			_presetVoiLuts.ToolbarModel = _component.ToolbarModel;
        	_presetVoiLuts.MenuModel = _component.ContextMenuModel;

			_presetVoiLuts.DataBindings.Add("Selection", source, "Selection", true, DataSourceUpdateMode.OnPropertyChanged);

			_comboModality.DataSource = _component.Modalities;
        	_comboModality.DataBindings.Add("Value", source, "SelectedModality", true, DataSourceUpdateMode.OnPropertyChanged);

        	_comboAddPreset.DataSource = _component.AvailableAddFactories;
        	_comboAddPreset.DisplayMember = "Description";
			_comboAddPreset.DataBindings.Add("Visible", source, "HasMultipleFactories", true, DataSourceUpdateMode.Never);
			_comboAddPreset.DataBindings.Add("Value", source, "SelectedAddFactory", true, DataSourceUpdateMode.OnPropertyChanged);

			_addPresetButton.DataBindings.Add("Visible", source, "HasMultipleFactories", true, DataSourceUpdateMode.Never);
			_addPresetButton.DataBindings.Add("Enabled", source, "AddEnabled", true, DataSourceUpdateMode.Never);

			_comboAddPreset.VisibleChanged += new System.EventHandler(OnAddPresetVisibleChanged);
			
			_addPresetButton.Click += delegate { _component.OnAdd(); };
			_presetVoiLuts.ItemDoubleClicked += delegate { _component.OnEditSelected(); };
        }

		void OnAddPresetVisibleChanged(object sender, System.EventArgs e)
		{
			if (!_comboAddPreset.Visible)
			{
				_tableLayoutPanel.SetRowSpan(_presetVoiLuts, 2);
			}
			else
			{
				_tableLayoutPanel.SetRowSpan(_presetVoiLuts, 1);
			}
		}
    }
}
