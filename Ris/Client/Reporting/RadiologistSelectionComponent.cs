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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Reporting
{
    /// <summary>
    /// Extension point for views onto <see cref="RadiologistSelectionComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class RadiologistSelectionComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(RadiologistSelectionComponentViewExtensionPoint))]
    public class RadiologistSelectionComponent : ApplicationComponent
    {
        private bool _makeDefault;
        private StaffSummary _selectedRadiologist;
        private StaffSummaryTable _radiologistTable;

        public override void Start()
        {
            _radiologistTable = new StaffSummaryTable();

            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    GetRadiologistListResponse response = service.GetRadiologistList(new GetRadiologistListRequest());
                    _radiologistTable.Items.AddRange(response.Radiologists);

                    // Choose the default supervisor
                    if (String.IsNullOrEmpty(SupervisorSettings.Default.SupervisorID) == false)
                    {
                        _selectedRadiologist = CollectionUtils.SelectFirst<StaffSummary>(
                            response.Radiologists,
                            delegate(StaffSummary staff)
                                {
                                    return staff.StaffId == SupervisorSettings.Default.SupervisorID;
                                });
                        NotifyPropertyChanged("RadiologistSelection");
                    }
                });

            base.Start();
        }

        public ITable Radiologists
        {
            get { return _radiologistTable; }
        }

        public ISelection RadiologistSelection
        {
            get { return _selectedRadiologist == null ? Selection.Empty : new Selection(_selectedRadiologist); }
            set { _selectedRadiologist = (StaffSummary)value.Item; }
        }

        public bool AcceptEnabled
        {
            get { return _selectedRadiologist != null; }
        }

        public bool MakeDefaultChecked
        {
            get { return _makeDefault; }
            set { _makeDefault = value; }
        }

        public StaffSummary SelectedRadiologist
        {
            get { return _selectedRadiologist; }
        }

        public void Accept()
        {
            if (_makeDefault && _selectedRadiologist != null)
                SupervisorSettings.Default.SupervisorID = _selectedRadiologist.StaffId;

            this.ExitCode = ApplicationComponentExitCode.Normal;
            this.Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            this.Host.Exit();
        }
    }
}
