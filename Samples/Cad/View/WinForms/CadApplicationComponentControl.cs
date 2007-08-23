using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Samples.Cad.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="CadApplicationComponent"/>
    /// </summary>
    public partial class CadApplicationComponentControl : ApplicationComponentUserControl
    {
        private CadApplicationComponent _component;
		private BindingSource _bindingSource;

        /// <summary>
        /// Constructor
        /// </summary>
        public CadApplicationComponentControl(CadApplicationComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

			_bindingSource = new BindingSource();
			_bindingSource.DataSource = _component;

			_opacityControl.TrackBarIncrements = 100;
        	_thresholdControl.TrackBarIncrements = 500;

			_opacityControl.DataBindings.Clear();
			_opacityControl.DataBindings.Add("Enabled", _bindingSource, "OpacityEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_opacityControl.DataBindings.Add("Minimum", _bindingSource, "OpacityMinimum", true, DataSourceUpdateMode.OnPropertyChanged);
			_opacityControl.DataBindings.Add("Maximum", _bindingSource, "OpacityMaximum", true, DataSourceUpdateMode.OnPropertyChanged);
			_opacityControl.DataBindings.Add("Value", _bindingSource, "Opacity", true, DataSourceUpdateMode.OnPropertyChanged);

			_thresholdControl.DataBindings.Clear();
			_thresholdControl.DataBindings.Add("Enabled", _bindingSource, "ThresholdEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_thresholdControl.DataBindings.Add("Minimum", _bindingSource, "ThresholdMinimum", true, DataSourceUpdateMode.OnPropertyChanged);
			_thresholdControl.DataBindings.Add("Maximum", _bindingSource, "ThresholdMaximum", true, DataSourceUpdateMode.OnPropertyChanged);
			_thresholdControl.DataBindings.Add("Value", _bindingSource, "Threshold", true, DataSourceUpdateMode.OnPropertyChanged);

        	_analyzeButton.Click += delegate(object sender, EventArgs e)
        	                        	{
        	                        		_component.Analyze();
        	                        	};
		}
    }
}
