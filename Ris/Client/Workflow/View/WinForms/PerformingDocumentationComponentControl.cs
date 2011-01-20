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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
	/// Provides a Windows Forms user-interface for <see cref="PerformingDocumentationComponent"/>
    /// </summary>
    public partial class PerformingDocumentationComponentControl : ApplicationComponentUserControl
    {
        private readonly PerformingDocumentationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PerformingDocumentationComponentControl(PerformingDocumentationComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _overviewLayoutPanel.RowStyles[0].Height = _component.BannerHeight; 

            Control banner = (Control) _component.BannerHost.ComponentView.GuiElement;
            banner.Dock = DockStyle.Fill;
            _bannerPanel.Controls.Add(banner);

            Control documentationTabs = (Control)_component.DocumentationHost.ComponentView.GuiElement;
            documentationTabs.Dock = DockStyle.Fill;
            _orderDocumentationPanel.Controls.Add(documentationTabs);

            _btnComplete.DataBindings.Add("Enabled", _component, "CompleteEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_btnComplete.DataBindings.Add("Visible", _component, "CompleteVisible", true, DataSourceUpdateMode.OnPropertyChanged);
            _btnSave.DataBindings.Add("Text", _component, "SaveText", true, DataSourceUpdateMode.OnPropertyChanged);
            _btnSave.DataBindings.Add("Enabled", _component, "SaveEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _assignedRadiologistLookup.LookupHandler = _component.RadiologistLookupHandler;
            _assignedRadiologistLookup.DataBindings.Add("Value", _component, "AssignedRadiologist", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _btnSave_Click(object sender, EventArgs e)
        {
            using(new CursorManager(Cursors.WaitCursor))
            {
                _component.SaveDocumentation();
            }
        }

        private void _btnComplete_Click(object sender, EventArgs e)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                _component.CompleteDocumentation();
            }
        }
    }
}
