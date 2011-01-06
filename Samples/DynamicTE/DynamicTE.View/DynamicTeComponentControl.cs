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

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.DynamicTe.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DynamicTeComponent"/>
    /// </summary>
    public partial class DynamicTeComponentControl : ApplicationComponentUserControl
    {
        private DynamicTeComponent _component;
		private BindingSource _bindingSource;

        /// <summary>
        /// Constructor
        /// </summary>
        public DynamicTeComponentControl(DynamicTeComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

			_bindingSource = new BindingSource();
			_bindingSource.DataSource = _component;

			_probabilityMapVisible.DataBindings.Clear();
        	_probabilityMapVisible.DataBindings.Add("Enabled", _bindingSource, "ProbabilityMapEnabled", true,
        	                                        DataSourceUpdateMode.OnPropertyChanged);
			_probabilityMapVisible.DataBindings.Add("Checked", _bindingSource, "ProbabilityMapVisible", true,
													DataSourceUpdateMode.OnPropertyChanged);


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

			_createDynamicTeButton.DataBindings.Clear();
        	_createDynamicTeButton.DataBindings.Add("Enabled", _bindingSource, "CreateDynamicTeSeriesEnabled", true,
        	                                        DataSourceUpdateMode.OnPropertyChanged);
			_createDynamicTeButton.Click += delegate(object sender, EventArgs e)
										{
											_component.CreateDynamicTeSeries();
										};

			_component.AllPropertiesChanged += delegate(object sender, EventArgs e)
			                             	{
			                             		_bindingSource.ResetBindings(false);
			                             	};
		}

    }
}
