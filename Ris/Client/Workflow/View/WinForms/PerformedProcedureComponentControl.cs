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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PerformedProcedureComponent"/>
    /// </summary>
    public partial class PerformedProcedureComponentControl : ApplicationComponentUserControl
    {
        private PerformedProcedureComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PerformedProcedureComponentControl(PerformedProcedureComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;
            _component = component;

            _procedurePlanSummary.Table = _component.ProcedurePlanSummaryTable;
            _procedurePlanSummary.MenuModel = _component.ProcedurePlanTreeActionModel;
            _procedurePlanSummary.ToolbarModel = _component.ProcedurePlanTreeActionModel;

            _mppsTableView.Table = _component.MppsTable;
            _mppsTableView.DataBindings.Add("Selection", _component, "SelectedMpps", true, DataSourceUpdateMode.OnPropertyChanged);
            _mppsTableView.MenuModel = _component.MppsTableActionModel;
            _mppsTableView.ToolbarModel = _component.MppsTableActionModel;

            Control detailsPage = (Control)_component.DetailsComponentHost.ComponentView.GuiElement;
            detailsPage.Dock = DockStyle.Fill;
            _mppsDetailsPanel.Controls.Add(detailsPage);
        }
    }
}
