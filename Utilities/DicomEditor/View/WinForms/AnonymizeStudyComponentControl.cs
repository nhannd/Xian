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
using ClearCanvas.Utilities.DicomEditor.Tools;

namespace ClearCanvas.Utilities.DicomEditor.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="AnonymizeStudyComponent"/>.
    /// </summary>
    public partial class AnonymizeStudyComponentControl : ApplicationComponentUserControl
    {
        private AnonymizeStudyComponent _component;
    	private bool _updating = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AnonymizeStudyComponentControl(AnonymizeStudyComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

        	this.AcceptButton = _okButton;
        	this.CancelButton = _cancelButton;

			_patientId.DataBindings.Add("Value", _component, "PatientId", true, DataSourceUpdateMode.OnPropertyChanged);
			_patientsName.DataBindings.Add("Value", _component, "PatientsName", true, DataSourceUpdateMode.OnPropertyChanged);
			_accessionNumber.DataBindings.Add("Value", _component, "AccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDescription.DataBindings.Add("Value", _component, "StudyDescription", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDate.DataBindings.Add("Value", _component, "StudyDate", true, DataSourceUpdateMode.OnPropertyChanged);
			_dateOfBirth.DataBindings.Add("Value", _component, "PatientsBirthDate", true, DataSourceUpdateMode.OnPropertyChanged);
			_preserveSeriesData.DataBindings.Add("Checked", _component, "PreserveSeriesData", true, DataSourceUpdateMode.OnPropertyChanged);

        	_keepReportsAndAttachments.Checked = _component.KeepReportsAndAttachments;
        	_keepReportsAndAttachments.CheckedChanged += _keepReportsAndAttachments_CheckedChanged;
		}

		private void OnOkButtonClicked(object sender, System.EventArgs e)
		{
			_component.Accept();
		}

		private void OnCancelButtonClicked(object sender, System.EventArgs e)
		{
			_component.Cancel();
		}

    	private void _keepReportsAndAttachments_CheckedChanged(object sender, EventArgs e)
    	{
    		if (_updating)
    			return;

    		_updating = true;
    		try
    		{
    			_component.KeepReportsAndAttachments = _keepReportsAndAttachments.Checked;
    			_keepReportsAndAttachments.Checked = _component.KeepReportsAndAttachments;
    			_warningProvider.SetError(_keepReportsAndAttachments, _keepReportsAndAttachments.Checked ? SR.WarningKeepReportsAndAttachmentsIsPotentialPatientPrivacyIssue : string.Empty);
    		}
    		finally
    		{
    			_updating = false;
    		}
    	}
    }
}
