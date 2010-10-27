#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Ris.Client;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="PublishReportComponent"/>.
	/// </summary>
	public partial class PublishReportComponentControl : ApplicationComponentUserControl
	{
		private readonly PublishReportComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public PublishReportComponentControl(PublishReportComponent component)
			: base(component)
		{
			_component = component;
			InitializeComponent();

			Control browser = (Control) _component.PublishReportPreviewComponentHost.ComponentView.GuiElement;
			_browserPanel.Controls.Add(browser);
			browser.Dock = DockStyle.Fill;

			_recipientsTableView.Table = _component.Recipients;
			_recipientsTableView.MenuModel = _component.RecipientsActionModel;
			_recipientsTableView.ToolbarModel = _component.RecipientsActionModel;
			_recipientsTableView.DataBindings.Add("Selection", _component, "SelectedRecipient", true, DataSourceUpdateMode.OnPropertyChanged);
			_addConsultantButton.DataBindings.Add("Enabled", _component.RecipientsActionModel.Add, "Enabled");

			_consultantLookup.LookupHandler = _component.RecipientsLookupHandler;
			_consultantLookup.DataBindings.Add("Value", _component, "RecipientToAdd", true, DataSourceUpdateMode.OnPropertyChanged);
			_consultantContactPoint.DataBindings.Add("DataSource", _component, "RecipientContactPointChoices", true, DataSourceUpdateMode.Never);
			_consultantContactPoint.DataBindings.Add("Value", _component, "RecipientContactPointToAdd", true, DataSourceUpdateMode.OnPropertyChanged);
			_consultantContactPoint.Format += delegate(object source, ListControlConvertEventArgs e) { e.Value = _component.FormatContactPoint(e.ListItem); };

			_publishButton.Visible = _component.PublishVisible;
			_printButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_publishButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_component.PropertyChanged += _component_propertyChanged;
		}

		private void _component_propertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "AcknowledgeEnabled")
			{
				_printButton.Enabled = _component.AcceptEnabled;
				_publishButton.Enabled = _component.AcceptEnabled;
			}
		}

		private void _publishButton_Click(object sender, EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.SendReportToQueue();
			}
		}

		private void _printButton_Click(object sender, EventArgs e)
		{
			using(new CursorManager(Cursors.WaitCursor))
			{
				_component.PrintLocal();
			}
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}

		private void _addConsultantButton_Click(object sender, EventArgs e)
		{
			_component.AddRecipient();
		}
	}
}
