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

			_toolbarSize.DataSource = Enum.GetValues(typeof (IconSize));

			_wrapToolbars.DataBindings.Add("Checked", _component, "Wrap", false, DataSourceUpdateMode.OnPropertyChanged);
			_toolbarSize.DataBindings.Add("Value", _component, "IconSize", true, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}