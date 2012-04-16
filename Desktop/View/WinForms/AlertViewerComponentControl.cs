#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="TestComponent"/>.
    /// </summary>
    public partial class AlertViewerComponentControl : ApplicationComponentUserControl
    {
        private readonly AlertViewerComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
		public AlertViewerComponentControl(AlertViewerComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

        	_alertTableView.Table = _component.Alerts;
        	_alertTableView.ToolbarModel = _component.AlertActions;
        	_alertTableView.MenuModel = _component.AlertActions;
        }
	}
}
