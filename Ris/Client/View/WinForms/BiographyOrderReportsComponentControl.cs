#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="BiographyOrderReportsComponent"/>.
	/// </summary>
	public partial class BiographyOrderReportsComponentControl : ApplicationComponentUserControl
	{
		private BiographyOrderReportsComponent _component;
		private ActionModelNode _toolbarActionModel;

		/// <summary>
		/// Constructor.
		/// </summary>
		public BiographyOrderReportsComponentControl(BiographyOrderReportsComponent component)
			: base(component)
		{
			_component = component;
			InitializeComponent();

			Control reportPreview = (Control) _component.ReportPreviewComponentHost.ComponentView.GuiElement;
			reportPreview.Dock = DockStyle.Fill;
			_reportPreviewPanel.Controls.Add(reportPreview);

			_reports.DataSource = _component.Reports;
			_reports.DataBindings.Add("Value", _component, "SelectedReport", true, DataSourceUpdateMode.OnPropertyChanged);
			_reports.Format += delegate(object sender, ListControlConvertEventArgs e) { e.Value = _component.FormatReportListItem(e.ListItem); };

			_toolbarActionModel = _component.ActionModel;
			ToolStripBuilder.BuildToolbar(_toolstrip.Items, _toolbarActionModel.ChildNodes);

			_component.AllPropertiesChanged += AllPropertiesChangedEventHandler;
		}

		private void AllPropertiesChangedEventHandler(object sender, EventArgs e)
		{
			_reports.DataSource = _component.Reports;
		}
	}
}
