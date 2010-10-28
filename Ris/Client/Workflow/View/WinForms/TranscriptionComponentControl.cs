#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="TranscriptionComponent"/>.
    /// </summary>
    public partial class TranscriptionComponentControl : ApplicationComponentUserControl
    {
        private TranscriptionComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TranscriptionComponentControl(TranscriptionComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

            _overviewLayoutPanel.RowStyles[0].Height = _component.BannerHeight; 

			Control banner = (Control)_component.BannerHost.ComponentView.GuiElement;
			banner.Dock = DockStyle.Fill;
			_bannerPanel.Controls.Add(banner);

			Control transcriptionEditor = (Control)_component.TranscriptionEditorHost.ComponentView.GuiElement;
			transcriptionEditor.Dock = DockStyle.Fill;
			_transcriptiontEditorPanel.Controls.Add(transcriptionEditor);

			Control rightHandContent = (Control)_component.RightHandComponentContainerHost.ComponentView.GuiElement;
			rightHandContent.Dock = DockStyle.Fill;
			_rightHandPanel.Controls.Add(rightHandContent);

			_statusText.DataBindings.Add("Text", _component, "StatusText", true, DataSourceUpdateMode.OnPropertyChanged);
			_statusText.DataBindings.Add("Visible", _component, "StatusTextVisible", true, DataSourceUpdateMode.OnPropertyChanged);

			_reportNextItem.DataBindings.Add("Checked", _component, "TranscribeNextItem", true, DataSourceUpdateMode.OnPropertyChanged);
			_reportNextItem.DataBindings.Add("Enabled", _component, "TranscribeNextItemEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_completeButton.DataBindings.Add("Enabled", _component, "CompleteEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
			_rejectButton.DataBindings.Add("Enabled", _component, "RejectEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_supervisor.LookupHandler = _component.SupervisorLookupHandler;
			_supervisor.DataBindings.Add("Value", _component, "Supervisor", true, DataSourceUpdateMode.OnPropertyChanged);
			_supervisor.Visible = _component.SupervisorVisible;

			_submitForReviewButton.DataBindings.Add("Enabled", _component, "SubmitForReviewEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_submitForReviewButton.Visible = _component.SubmitForReviewVisible;

			_btnSkip.DataBindings.Add("Enabled", _component, "SkipEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_saveButton.DataBindings.Add("Enabled", _component, "SaveReportEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_component.PropertyChanged += _component_PropertyChanged;
		}

		private void _component_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "StatusText")
			{
				_statusText.Refresh();
			}
		}


		private void _completeButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.Complete();
			}
		}

		private void _rejectButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.Reject();
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

		private void _submitForReviewButton_Click(object sender, System.EventArgs e)
		{
			using (new CursorManager(this, Cursors.WaitCursor))
			{
				_component.SubmitForReview();
			}
		}
	}
}
