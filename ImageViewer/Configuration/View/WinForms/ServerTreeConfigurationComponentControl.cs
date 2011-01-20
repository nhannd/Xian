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
using ClearCanvas.ImageViewer.Configuration.ServerTree;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
	public partial class ServerTreeConfigurationComponentControl : ApplicationComponentUserControl
	{
		private readonly ServerTreeConfigurationComponent _component;

		public ServerTreeConfigurationComponentControl(ServerTreeConfigurationComponent component)
			:base(component)
		{
			_component = component;
			InitializeComponent();

			_description.DataBindings.Add("Text", _component, "Description", true, DataSourceUpdateMode.OnPropertyChanged);
			_splitContainer.Panel2.Controls.Add((UserControl)_component.ServerTreeHost.ComponentView.GuiElement);
			_splitContainer.Panel2.Controls[0].Dock = DockStyle.Fill;
		}
	}
}