#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
