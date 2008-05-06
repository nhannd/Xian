#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="PreliminaryDiagnosisConversationComponent"/>.
	/// </summary>
	public partial class PreliminaryDiagnosisConversationComponentControl : ApplicationComponentUserControl
	{
		private readonly PreliminaryDiagnosisConversationComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public PreliminaryDiagnosisConversationComponentControl(PreliminaryDiagnosisConversationComponent component)
			: base(component)
		{
			InitializeComponent();

			_component = component;

			_notes.Table = _component.Notes;
			_notes.DataBindings.Add("Selection", _component, "SelectedNote", true, DataSourceUpdateMode.OnPropertyChanged);
			_selectedNoteBody.DataBindings.Add("Value", _component, "SelectedNoteBody", true, DataSourceUpdateMode.OnPropertyChanged);

			_replyBody.DataBindings.Add("Text", _component, "Body", true, DataSourceUpdateMode.OnPropertyChanged);
			_recipients.Table = _component.Recipients;
			_recipients.MenuModel = _component.RecipientActionModel;
			_recipients.ToolbarModel = _component.RecipientActionModel;
			_recipients.DataBindings.Add("Selection", _component, "SelectedRecipient", true, DataSourceUpdateMode.OnPropertyChanged);

			_staffRecipientLookup.LookupHandler = _component.StaffRecipientLookupHandler;
			_staffRecipientLookup.DataBindings.Add("Value", _component, "SelectedStaffRecipient", true, DataSourceUpdateMode.OnPropertyChanged);
			_staffRecipientAddButton.DataBindings.Add("Enabled", _component, "AddStaffRecipientEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_groupRecipientLookup.LookupHandler = _component.GroupRecipientLookupHandler;
			_groupRecipientLookup.DataBindings.Add("Value", _component, "SelectedGroupRecipient", true, DataSourceUpdateMode.OnPropertyChanged);
			_groupRecipientAddButton.DataBindings.Add("Enabled", _component, "AddGroupRecipientEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_acknowledgePostButton.DataBindings.Add("Text", _component, "ReplyOrAcknowledge", true, DataSourceUpdateMode.OnPropertyChanged);
			_acknowledgePostButton.DataBindings.Add("Enabled", _component, "AcknowledgeEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_component.PropertyChanged += _component_propertyChanged;
		}

		private void _component_propertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "Recipients")
			{
				_recipients.Table = _component.Recipients;
			}
			else if (e.PropertyName == "AcknowledgeEnabled")
			{
				_acknowledgePostButton.Enabled = _component.AcknowledgeEnabled;
			}
		}

		private void _staffRecipientAddButton_Click(object sender, System.EventArgs e)
		{
			using(new CursorManager(Cursors.WaitCursor))
			{
				_component.AddStaffRecipient();
			}
		}

		private void _groupRecipientAddButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.AddGroupRecipient();
			}
		}

		private void _acknowledgePostButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.AcknowledgeAndOrPost();
			}
		}

		private void _cancelButton_Click(object sender, System.EventArgs e)
		{
			_component.Cancel();
		}
	}
}
