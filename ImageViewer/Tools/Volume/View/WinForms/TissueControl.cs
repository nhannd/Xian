using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.ImageViewer.Tools.Volume;

namespace ClearCanvas.ImageViewer.Tools.Volume.View.WinForms
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

			_opacityControl.DataBindings.Clear();
			_opacityControl.DataBindings.Add("Enabled", _bindingSource, "Visible", true, DataSourceUpdateMode.OnPropertyChanged);
			_opacityControl.DataBindings.Add("Minimum", _bindingSource, "MinimumOpacity", true, DataSourceUpdateMode.OnPropertyChanged);
			_opacityControl.DataBindings.Add("Maximum", _bindingSource, "MaximumOpacity", true, DataSourceUpdateMode.OnPropertyChanged);
			_opacityControl.DataBindings.Add("Value", _bindingSource, "Opacity", true, DataSourceUpdateMode.OnPropertyChanged);

			_windowControl.DataBindings.Clear();
			_windowControl.DataBindings.Add("Enabled", _bindingSource, "Visible", true, DataSourceUpdateMode.OnPropertyChanged);
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
