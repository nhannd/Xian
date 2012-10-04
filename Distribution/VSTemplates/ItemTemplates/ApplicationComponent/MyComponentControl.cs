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

namespace $app_comp_view_namespace$
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="$app_comp$"/>.
    /// </summary>
    public partial class $app_comp_control$ : ApplicationComponentUserControl
    {
        private $app_comp$ _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public $app_comp_control$($app_comp$ component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

            BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component;

            // TODO add .NET databindings to bindingSource
        }
    }
}
