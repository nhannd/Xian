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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="VisitPractitionerSummaryComponent"/>
    /// </summary>
    public partial class VisitPractitionersSummaryComponentControl : CustomUserControl
    {
        private VisitPractitionersSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitPractitionersSummaryComponentControl(VisitPractitionersSummaryComponent component)
        {
            InitializeComponent();

            _component = component;

            _visitPractitioners.Table = _component.VisitPractitioners;
            _visitPractitioners.MenuModel = _component.VisitPractionersActionModel;
            _visitPractitioners.ToolbarModel = _component.VisitPractionersActionModel;
        }

        private void _visitPractitioners_SelectionChanged(object sender, EventArgs e)
        {
            _component.SetSelectedVisitPractitioner(_visitPractitioners.Selection);
        }
    }
}
