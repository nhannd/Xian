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
	public partial class ReportingMppsDocumentationComponentControl : ApplicationComponentUserControl
	{
		private readonly ReportingMppsDocumentationComponent _component;

		public ReportingMppsDocumentationComponentControl(ReportingMppsDocumentationComponent component)
			: base(component)
		{
			InitializeComponent();

			_component = component;
			_mppsTableView.Table = _component.MppsTable;
			_mppsTableView.DataBindings.Add("Selection", _component, "SelectedMpps", true, DataSourceUpdateMode.OnPropertyChanged);

			_comments.DataBindings.Add("Text", _component, "MppsComments");
		}
	}
}
