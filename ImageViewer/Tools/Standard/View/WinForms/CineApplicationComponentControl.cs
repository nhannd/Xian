#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
    /// Provides a Windows Forms user-interface for <see cref="CineApplicationComponent"/>
    /// </summary>
    public partial class CineApplicationComponentControl : ApplicationComponentUserControl
    {
        private readonly CineApplicationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public CineApplicationComponentControl(CineApplicationComponent component)
            :base(component)
        {
			_component = component;
			
			InitializeComponent();

			BindingSource source = new BindingSource();
        	source.DataSource = _component;
        	_startForwardButton.DataBindings.Add("Enabled", source, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_startReverseButton.DataBindings.Add("Enabled", source, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
        	_stopButton.DataBindings.Add("Enabled", source, "Running", true, DataSourceUpdateMode.OnPropertyChanged);
			_cineSpeed.DataBindings.Add("Minimum", source, "MinimumScale", true, DataSourceUpdateMode.OnPropertyChanged);
			_cineSpeed.DataBindings.Add("Maximum", source, "MaximumScale", true, DataSourceUpdateMode.OnPropertyChanged);
			_cineSpeed.DataBindings.Add("Value", source, "CurrentScaleValue", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void StartReverseButtonClicked(object sender, EventArgs e)
		{
			_component.Reverse = true;
			_component.StartCine();
		}

		private void StopButtonClicked(object sender, EventArgs e)
		{
			_component.AutoCineEnabled = false;
			_component.StopCine();
		}

		private void StartForwardButtonClicked(object sender, EventArgs e)
		{
			_component.Reverse = false;
			_component.StartCine();
		}
    }
}
