#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
    /// Provides a Windows Forms user-interface for <see cref="WorkflowHistoryComponent"/>.
    /// </summary>
    public partial class WorkflowHistoryComponentControl : ApplicationComponentUserControl
    {
        private WorkflowHistoryComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public WorkflowHistoryComponentControl(WorkflowHistoryComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

        	_procedureTableView.Table = _component.ProcedureTable;
        	_procedureTableView.DataBindings.Add("Selection", _component, "SelectedProcedure", true, DataSourceUpdateMode.OnPropertyChanged);

        	Control c = (Control) _component.ProcedureViewComponentHost.ComponentView.GuiElement;
        	c.Dock = DockStyle.Fill;
			_historyPanel.Controls.Add(c);
        }
    }
}
