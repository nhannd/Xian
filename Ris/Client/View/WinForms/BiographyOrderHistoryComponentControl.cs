#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="BiographyOrderHistoryComponentControl"/>
    /// </summary>
    public partial class BiographyOrderHistoryComponentControl : ApplicationComponentUserControl
    {
        private readonly BiographyOrderHistoryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public BiographyOrderHistoryComponentControl(BiographyOrderHistoryComponent component)
        {
            InitializeComponent();
            _component = component;

            _orderList.Table = _component.Orders;
            _orderList.DataBindings.Add("Selection", _component, "SelectedOrder", true, DataSourceUpdateMode.OnPropertyChanged);

            // Order fields
            _placerNumber.DataBindings.Add("Value", _component, "PlacerNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            _accessionNumber.DataBindings.Add("Value", _component, "AccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            _reasonForStudy.DataBindings.Add("Value", _component, "ReasonForStudy", true, DataSourceUpdateMode.OnPropertyChanged);
            _cancelReason.DataBindings.Add("Value", _component, "CancelReason", true, DataSourceUpdateMode.OnPropertyChanged);
            _priority.DataBindings.Add("Value", _component, "Priority", true, DataSourceUpdateMode.OnPropertyChanged);
            _schedulingRequestDateTime.DataBindings.Add("Value", _component, "SchedulingRequestDateTime", true, DataSourceUpdateMode.OnPropertyChanged);
            _orderingPhysician.DataBindings.Add("Value", _component, "OrderingPhysician", true, DataSourceUpdateMode.OnPropertyChanged);
            _orderingFacility.DataBindings.Add("Value", _component, "OrderingFacility", true, DataSourceUpdateMode.OnPropertyChanged);

            // Visit fields
            _visitNumber.DataBindings.Add("Value", _component, "VisitNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            _site.DataBindings.Add("Value", _component, "Site", true, DataSourceUpdateMode.OnPropertyChanged);
            _preAdmitNumber.DataBindings.Add("Value", _component, "PreAdmitNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            _patientClass.DataBindings.Add("Value", _component, "PatientClass", true, DataSourceUpdateMode.OnPropertyChanged);
            _patientType.DataBindings.Add("Value", _component, "PatientType", true, DataSourceUpdateMode.OnPropertyChanged);
            _admissionType.DataBindings.Add("Value", _component, "AdmissionType", true, DataSourceUpdateMode.OnPropertyChanged);
            _visitStatus.DataBindings.Add("Value", _component, "VisitStatus", true, DataSourceUpdateMode.OnPropertyChanged);
            _admitDateTime.DataBindings.Add("Value", _component, "AdmitDateTime", true, DataSourceUpdateMode.OnPropertyChanged);
            _dischargeDateTime.DataBindings.Add("Value", _component, "DischargeDateTime", true, DataSourceUpdateMode.OnPropertyChanged);
            _ambulatoryStatus.DataBindings.Add("Value", _component, "AmbulatoryStatus", true, DataSourceUpdateMode.OnPropertyChanged);
            _vip.DataBindings.Add("Checked", _component, "VIP", true, DataSourceUpdateMode.OnPropertyChanged);

            // ProcedureStep fields
            _modality.DataBindings.Add("Value", _component, "Modality", true, DataSourceUpdateMode.OnPropertyChanged);
            _mpsState.DataBindings.Add("Value", _component, "MPSState", true, DataSourceUpdateMode.OnPropertyChanged);
            _mpsPerformerStaff.DataBindings.Add("Value", _component, "PerformerStaff", true, DataSourceUpdateMode.OnPropertyChanged);
            _mpsStartTime.DataBindings.Add("Value", _component, "StartTime", true, DataSourceUpdateMode.OnPropertyChanged);
            _mpsEndTime.DataBindings.Add("Value", _component, "EndTime", true, DataSourceUpdateMode.OnPropertyChanged);
            _mpsScheduledPerformerStaff.DataBindings.Add("Value", _component, "ScheduledPerformerStaff", true, DataSourceUpdateMode.OnPropertyChanged);
            _mpsScheduledStartTime.DataBindings.Add("Value", _component, "ScheduledStartTime", true, DataSourceUpdateMode.OnPropertyChanged);
            _mpsScheduledEndTime.DataBindings.Add("Value", _component, "ScheduledEndTime", true, DataSourceUpdateMode.OnPropertyChanged);

            // Requested Procedure breakdown
            _diagnosticServiceBreakdown.DataBindings.Add("Selection", _component, "SelectedDiagnosticServiceBreakdownItem", true, DataSourceUpdateMode.OnPropertyChanged);
            _component.DiagnosticServiceChanged += DiagnosticServiceChangedEventHandler;
        }

        private void DiagnosticServiceChangedEventHandler(object sender, EventArgs e)
        {
            _diagnosticServiceBreakdown.Tree = _component.DiagnosticServiceBreakdown;
            _diagnosticServiceBreakdown.ExpandAll();
        }
    }
}
