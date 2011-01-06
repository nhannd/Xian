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

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="HomepageConfigurationComponent"/>.
    /// </summary>
    public partial class HomepageConfigurationComponentControl : ApplicationComponentUserControl
    {
        private HomepageConfigurationComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public HomepageConfigurationComponentControl(HomepageConfigurationComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

			_showHomepageOnStartup.DataBindings.Add("Checked", _component, "ShowHomepageOnStartUp", true, DataSourceUpdateMode.OnPropertyChanged);
			_preventHompageClosing.DataBindings.Add("Checked", _component, "PreventHomepageFromClosing", true, DataSourceUpdateMode.OnPropertyChanged);
			_preventHompageClosing.DataBindings.Add("Enabled", _component, "PreventHomepageFromClosingEnabled");
		}
    }
}
