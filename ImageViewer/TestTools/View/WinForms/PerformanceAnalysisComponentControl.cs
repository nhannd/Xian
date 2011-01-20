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

namespace ClearCanvas.ImageViewer.TestTools.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PerformanceAnalysisComponent"/>.
    /// </summary>
    public partial class PerformanceAnalysisComponentControl : ApplicationComponentUserControl
    {
        private PerformanceAnalysisComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PerformanceAnalysisComponentControl(PerformanceAnalysisComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

        	_table.Table = _component.Table;
        	_table.MenuModel = _component.MenuModel;
        	_table.ReadOnly = true;
        }
    }
}
