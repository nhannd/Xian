#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DowntimeReportEntryComponent"/>
    /// </summary>
    public partial class DowntimeReportEntryComponentControl : ApplicationComponentUserControl
    {
        private DowntimeReportEntryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DowntimeReportEntryComponentControl(DowntimeReportEntryComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

        	_radioPasteReport.Checked = _component.HasReport;
        	_radioToBeReported.Checked = !_component.HasReport;

			_reportText.Enabled = _component.HasReport;
			_interpreterLookup.Enabled = _component.HasReport;
			_transcriptionistLookup.Enabled = _component.HasReport;

        	_reportText.DataBindings.Add("Value", _component, "ReportText", true, DataSourceUpdateMode.OnPropertyChanged);

        	_interpreterLookup.LookupHandler = _component.InterpreterLookupHandler;
        	_transcriptionistLookup.LookupHandler = _component.TranscriptionistLookupHandler;

			_interpreterLookup.DataBindings.Add("Value", _component, "Interpreter", true, DataSourceUpdateMode.OnPropertyChanged);
			_transcriptionistLookup.DataBindings.Add("Value", _component, "Transcriptionist", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _radioToBeReported_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void _radioPasteReport_CheckedChanged(object sender, EventArgs e)
		{
			_component.HasReport = _radioPasteReport.Checked;
			_reportText.Enabled = _component.HasReport;
			_interpreterLookup.Enabled = _component.HasReport;
			_transcriptionistLookup.Enabled = _component.HasReport;
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
