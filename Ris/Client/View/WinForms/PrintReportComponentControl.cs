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
	/// Provides a Windows Forms user-interface for <see cref="PrintReportComponent"/>.
	/// </summary>
	public partial class PrintReportComponentControl : ApplicationComponentUserControl
	{
		private readonly PrintReportComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public PrintReportComponentControl(PrintReportComponent component)
			: base(component)
		{
			_component = component;
			InitializeComponent();

			_practitionerLookup.LookupHandler = _component.RecipientsLookupHandler;
			_practitionerLookup.DataBindings.Add("Value", _component, "SelectedRecipient", true, DataSourceUpdateMode.OnPropertyChanged);
			_contactPoint.DataBindings.Add("DataSource", _component, "RecipientContactPointChoices", true, DataSourceUpdateMode.Never);
			_contactPoint.DataBindings.Add("Value", _component, "SelectedContactPoint", true, DataSourceUpdateMode.OnPropertyChanged);
			_contactPoint.Format += delegate(object source, ListControlConvertEventArgs e) { e.Value = _component.FormatContactPoint(e.ListItem); };
			_closeOnPrint.DataBindings.Add("Checked", _component, "CloseOnPrint", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _printButton_Click(object sender, EventArgs e)
		{
			_component.Print();
		}

		private void _closeButton_Click(object sender, EventArgs e)
		{
			_component.Close();
		}
	}
}
