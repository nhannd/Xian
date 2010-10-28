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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="VisitLocationsSummaryComponent"/>
    /// </summary>
    public partial class VisitLocationsSummaryComponentControl : CustomUserControl
    {
        private VisitLocationsSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitLocationsSummaryComponentControl(VisitLocationsSummaryComponent component)
        {
            InitializeComponent();

            _component = component;

            _locations.Table = _component.Locations;
            _locations.ToolbarModel = _component.VisitLocationActionModel;
            _locations.MenuModel = _component.VisitLocationActionModel;
        }

        private void _locations_SelectionChanged(object sender, EventArgs e)
        {
            _component.SetSelectedVisitLocation(_locations.Selection);
        }
    }
}
