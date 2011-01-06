#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="CloseHelperComponent"/>
    /// </summary>
    public partial class CloseHelperComponentControl : ApplicationComponentUserControl
    {
        private CloseHelperComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public CloseHelperComponentControl(CloseHelperComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;
            _workspaceTableView.Table = _component.Workspaces;
            _workspaceTableView.DataBindings.Add("Selection", _component, "SelectedWorkspace", true, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
