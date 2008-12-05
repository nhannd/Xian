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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="ReportingComponent"/>
	/// </summary>
	public partial class ReportingComponentControl : ApplicationComponentUserControl
	{
		private readonly ReportingComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public ReportingComponentControl(ReportingComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			Control banner = (Control)_component.BannerHost.ComponentView.GuiElement;
			banner.Dock = DockStyle.Fill;
			_bannerPanel.Controls.Add(banner);

			Control reportEditor = (Control)_component.ReportEditorHost.ComponentView.GuiElement;
			reportEditor.Dock = DockStyle.Fill;
			_reportEditorPanel.Controls.Add(reportEditor);

			Control rightHandContent = (Control)_component.RightHandComponentContainerHost.ComponentView.GuiElement;
			rightHandContent.Dock = DockStyle.Fill;
			_rightHandPanel.Controls.Add(rightHandContent);

			_statusText.DataBindings.Add("Text", _component, "StatusText", true, DataSourceUpdateMode.OnPropertyChanged);
			_statusText.DataBindings.Add("Visible", _component, "StatusTextVisible", true, DataSourceUpdateMode.OnPropertyChanged);

			_imagesUnavailable.DataBindings.Add("Visible", _component, "ImagesUnavailableVisible", true, DataSourceUpdateMode.OnPropertyChanged);

			_reportNextItem.DataBindings.Add("Checked", _component, "ReportNextItem", true, DataSourceUpdateMode.OnPropertyChanged);
			_reportNextItem.DataBindings.Add("Enabled", _component, "ReportNextItemEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_verifyButton.DataBindings.Add("Enabled", _component, "VerifyEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			_sendToVerifyButton.DataBindings.Add("Enabled", _component, "SendToVerifyEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			_sendToTranscriptionButton.DataBindings.Add("Enabled", _component, "SendToTranscriptionEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_supervisor.LookupHandler = _component.SupervisorLookupHandler;
			_supervisor.DataBindings.Add("Value", _component, "Supervisor", true, DataSourceUpdateMode.OnPropertyChanged);

			_supervisor.Visible = _component.SupervisorVisible;
			_verifyButton.Visible = _component.VerifyReportVisible;
			_sendToVerifyButton.Visible = _component.SendToVerifyVisible;
			_sendToTranscriptionButton.Visible = _component.SendToTranscriptionVisible;

			_btnSkip.DataBindings.Add("Enabled", _component, "SkipEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_saveButton.DataBindings.Add("Enabled", _component, "SaveReportEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_component.PropertyChanged += _component_PropertyChanged;

			_reportedProcedures.DataBindings.Add("Text", _component, "ProceduresText", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _component_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "StatusText")
			{
				_statusText.Refresh();
			}
		}


		private void _verifyButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.Verify();
			}
		}

		private void _sendToVerifyButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.SendToBeVerified();
			}
		}

		private void _sendToTranscriptionButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.SendToTranscription();
			}
		}

		private void _saveButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.SaveReport();
			}
		}

		private void _cancelButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.CancelEditing();
			}
		}

		private void _btnSkip_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(this, Cursors.WaitCursor))
			{
				_component.Skip();
			}
		}
	}
}
