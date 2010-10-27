#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Ris.Client.Workflow;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="LinkProceduresComponent"/>
    /// </summary>
    public partial class LinkProceduresComponentControl : ApplicationComponentUserControl
    {
        private readonly LinkProceduresComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public LinkProceduresComponentControl(LinkProceduresComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;
            _sourceWorklistItem.Table = _component.SourceTable;
            _worklistItemTableView.Table = _component.CandidateTable;

            _instructionsLabel.Text = _component.Instructions;
            _heading.Text = _component.Heading;
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }
    }
}
