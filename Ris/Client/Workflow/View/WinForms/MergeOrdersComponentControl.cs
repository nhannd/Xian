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
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="MergeOrdersComponent"/>.
	/// </summary>
	public partial class MergeOrdersComponentControl : ApplicationComponentUserControl
	{
		private readonly MergeOrdersComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MergeOrdersComponentControl(MergeOrdersComponent component)
			:base(component)
		{
			_component = component;
			InitializeComponent();

			_ordersTableView.Table = _component.OrdersTable;
			_ordersTableView.DataBindings.Add("Selection", _component, "OrdersTableSelection", true, DataSourceUpdateMode.OnPropertyChanged);

			_acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			var previewControl = (Control)_component.MergedOrderPreviewComponentHost.ComponentView.GuiElement;
			_mergedOrderPreviewPanel.Controls.Add(previewControl);
			previewControl.Dock = DockStyle.Fill;
		}

		private void _acceptButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}
