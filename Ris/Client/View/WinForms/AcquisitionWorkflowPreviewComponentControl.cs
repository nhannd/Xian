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
    /// Provides a Windows Forms user-interface for <see cref="AcquisitionWorkflowPreviewComponent"/>
    /// </summary>
    public partial class AcquisitionWorkflowPreviewComponentControl : ApplicationComponentUserControl
    {
        private AcquisitionWorkflowPreviewComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public AcquisitionWorkflowPreviewComponentControl(AcquisitionWorkflowPreviewComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            // TODO add .NET databindings to _component
        }
    }
}
