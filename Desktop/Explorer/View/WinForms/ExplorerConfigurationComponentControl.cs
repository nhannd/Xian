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

namespace ClearCanvas.Desktop.Explorer.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ExplorerConfigurationComponent"/>.
    /// </summary>
    public partial class ExplorerConfigurationComponentControl : ApplicationComponentUserControl
    {
        private ExplorerConfigurationComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExplorerConfigurationComponentControl(ExplorerConfigurationComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

        	_launchAsShelf.DataBindings.Add("Checked", _component, "LaunchAsShelf", false,
        	                                DataSourceUpdateMode.OnPropertyChanged);

			_launchAsWorkspace.DataBindings.Add("Checked", _component, "LaunchAsWorkspace", false,
									DataSourceUpdateMode.OnPropertyChanged);

			_launchAtStartup.DataBindings.Add("Checked", _component, "LaunchAtStartup", false,
									DataSourceUpdateMode.OnPropertyChanged);
		}
    }
}
