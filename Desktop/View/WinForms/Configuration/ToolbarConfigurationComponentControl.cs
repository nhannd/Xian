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
using ClearCanvas.Desktop.Configuration.Standard;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	public partial class ToolbarConfigurationComponentControl : UserControl
	{
		private readonly ToolbarConfigurationComponent _component;

		public ToolbarConfigurationComponentControl(ToolbarConfigurationComponent component)
		{
			InitializeComponent();

			_component = component;

			_wrapToolbars.DataBindings.Add("Checked", _component, "Wrap", false, DataSourceUpdateMode.OnPropertyChanged);
			_toolbarSize.DataBindings.Add("Text", _component, "IconSize", true, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}