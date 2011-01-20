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

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="ExternalPractitionerContactPointSummaryComponent"/>
	/// </summary>
	public partial class ExternalPractitionerContactPointSummaryComponentControl : ApplicationComponentUserControl
	{
		private readonly ExternalPractitionerContactPointSummaryComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public ExternalPractitionerContactPointSummaryComponentControl(ExternalPractitionerContactPointSummaryComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			_contactPointTableView.Table = _component.SummaryTable;
			_contactPointTableView.MenuModel = _component.SummaryTableActionModel;
			_contactPointTableView.ToolbarModel = _component.SummaryTableActionModel;
			_contactPointTableView.DataBindings.Add("Selection", _component, "SummarySelection", true, DataSourceUpdateMode.OnPropertyChanged);

			_okButton.DataBindings.Add("Visible", _component, "ShowAcceptCancelButtons");
			_okButton.DataBindings.Add("Enabled", _component, "AcceptEnabled");
			_cancelButton.DataBindings.Add("Visible", _component, "ShowAcceptCancelButtons");
		}

		private void _contactPointTableView_ItemDoubleClicked(object sender, EventArgs e)
		{
			_component.DoubleClickSelectedItem();
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}
