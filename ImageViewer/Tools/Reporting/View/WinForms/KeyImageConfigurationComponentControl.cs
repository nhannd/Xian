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
using ClearCanvas.ImageViewer.Tools.Reporting.KeyImages;

namespace ClearCanvas.ImageViewer.Tools.Reporting.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="KeyImageConfigurationComponent"/>.
    /// </summary>
    public partial class KeyImageConfigurationComponentControl : ApplicationComponentUserControl
    {
        private KeyImageConfigurationComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public KeyImageConfigurationComponentControl(KeyImageConfigurationComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

			_publishToDefaultServers.DataBindings.Add("Checked", _component, "PublishToDefaultServers", false, DataSourceUpdateMode.OnPropertyChanged);
			_publishLocalToSourceAE.DataBindings.Add("Checked", _component, "PublishLocalToSourceAE", false, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
