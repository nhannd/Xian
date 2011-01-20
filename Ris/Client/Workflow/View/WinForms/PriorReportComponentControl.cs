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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="PriorReportComponent"/>
	/// </summary>
	public partial class PriorReportComponentControl : ApplicationComponentUserControl
	{
		private PriorReportComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public PriorReportComponentControl(PriorReportComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			Control reportViewer = (Control)_component.ReportViewComponentHost.ComponentView.GuiElement;
			reportViewer.Dock = DockStyle.Fill;
			splitContainer1.Panel2.Controls.Add(reportViewer);

			_reportList.Table = _component.Reports;
			_reportList.DataBindings.Add("Selection", _component, "SelectedReport", true, DataSourceUpdateMode.OnPropertyChanged);

			_radioRelevantPriors.DataBindings.Add("Checked", _component, "RelevantPriorsOnly", true, DataSourceUpdateMode.OnPropertyChanged);
			_radioAllPriors.DataBindings.Add("Checked", _component, "AllPriors", true, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}
