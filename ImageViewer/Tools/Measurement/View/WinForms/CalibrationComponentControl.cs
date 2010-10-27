#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using System;

namespace ClearCanvas.ImageViewer.Tools.Measurement.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="CalibrationComponent"/>.
    /// </summary>
    public partial class CalibrationComponentControl : ApplicationComponentUserControl
    {
        private readonly CalibrationComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CalibrationComponentControl(CalibrationComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

        	this.AcceptButton = _ok;
        	this.CancelButton = _cancel;

        	_length.DecimalPlaces = _component.DecimalPlaces;
        	_length.Increment = (decimal) _component.Increment;
        	_length.Minimum = (decimal) _component.Minimum;

            BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component;

        	_length.DataBindings.Add("Value", bindingSource, "LengthInCm", true, DataSourceUpdateMode.OnPropertyChanged);
        	_ok.Click += delegate
        	             	{
        	             		_component.Accept();
        	             	};
        	_cancel.Click += delegate
        	                 	{
        	                 		_component.Cancel();
        	                 	};
        }
    }
}
