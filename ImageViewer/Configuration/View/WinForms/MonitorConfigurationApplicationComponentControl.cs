#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="MonitorConfigurationApplicationComponent"/>
	/// </summary>
	public partial class MonitorConfigurationApplicationComponentControl : ApplicationComponentUserControl
	{
		private MonitorConfigurationApplicationComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public MonitorConfigurationApplicationComponentControl(MonitorConfigurationApplicationComponent component)
			:base(component)
		{
			InitializeComponent();

			_component = component;

			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component;

			_singleWindowRadio.DataBindings.Add("Checked", bindingSource, "SingleWindow", true, DataSourceUpdateMode.OnPropertyChanged);
			_separateWindowRadio.DataBindings.Add("Checked", bindingSource, "SeparateWindow", true, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}