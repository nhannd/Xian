using System.Windows.Forms;
using System;
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

        	_addPresetButton.Click += delegate { _component.OnAdd(); };
			_presetVoiLuts.ItemDoubleClicked += delegate { _component.OnEditSelected(); };
        }
    }
}
