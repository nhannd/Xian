using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="WindowLevelPresetApplicationComponent"/>
    /// </summary>
    public partial class WindowLevelPresetApplicationComponentControl : ApplicationComponentUserControl
    {
		private BindingSource _bindingSource;
		private WindowLevelPresetApplicationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public WindowLevelPresetApplicationComponentControl(WindowLevelPresetApplicationComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

			_window.Minimum = 1;
			_window.Maximum = 65535;
			_window.Increment = 10;
			_window.Accelerations.Add(new NumericUpDownAcceleration(2, 50));
			_window.Accelerations.Add(new NumericUpDownAcceleration(4, 500));
			_window.Accelerations.Add(new NumericUpDownAcceleration(6, 5000));

			_level.Minimum = -65535;
			_level.Maximum = 65535;
			_level.Increment = 10;
			_level.Accelerations.Add(new NumericUpDownAcceleration(2, 50));
			_level.Accelerations.Add(new NumericUpDownAcceleration(4, 500));
			_level.Accelerations.Add(new NumericUpDownAcceleration(6, 5000));

			_window.Value = _component.Window;
			_level.Value = _component.Level;
			_name.Value = _component.Name;

			_bindingSource = new BindingSource();
			_bindingSource.DataSource = _component;

			_comboKey.DataSource = _component.AvailableKeys;

			_comboKey.DataBindings.Add("Value", _bindingSource, "SelectedKey", true, DataSourceUpdateMode.OnPropertyChanged);
			_name.DataBindings.Add("Value", _bindingSource, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
			_window.DataBindings.Add("Value", _bindingSource, "Window", true, DataSourceUpdateMode.OnPropertyChanged);
			_level.DataBindings.Add("Value", _bindingSource, "Level", true, DataSourceUpdateMode.OnPropertyChanged);
			_ok.DataBindings.Add("Enabled", _bindingSource, "OKEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_ok.Click += new EventHandler(OnOK);
			_cancel.Click += new EventHandler(OnCancel);
        }

		void OnOK(object sender, EventArgs e)
		{
			_component.OK();
		}

		void OnCancel(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}
