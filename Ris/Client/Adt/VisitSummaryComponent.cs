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
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="VisitSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class VisitSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// VisitSummaryComponent class
    /// </summary>
    [AssociateView(typeof(VisitSummaryComponentViewExtensionPoint))]
    public class VisitSummaryComponent : ApplicationComponent
    {
        private EntityRef _patientRef;
        private EntityRef _patientProfileRef;

        private VisitSummaryTable _visitTable;
        private VisitSummary _currentVisitSelection;

        private CrudActionModel _visitActionHandler;

        public VisitSummaryComponent(EntityRef patientProfileRef)
        {
            _patientProfileRef = patientProfileRef;
        }

        public override void Start()
        {
            Platform.GetService<IPatientAdminService>(
                delegate(IPatientAdminService service)
                {
                    LoadPatientProfileForAdminEditResponse response = service.LoadPatientProfileForAdminEdit(new LoadPatientProfileForAdminEditRequest(_patientProfileRef));
                    _patientRef = response.PatientRef;
                    _patientProfileRef = response.PatientProfileRef;

                    this.Host.SetTitle(string.Format(SR.TitleVisitSummaryComponent,
                        PersonNameFormat.Format(response.PatientDetail.Name), 
                        MrnFormat.Format(response.PatientDetail.Mrn)));                    
                });

            _visitTable = new VisitSummaryTable();

            _visitActionHandler = new CrudActionModel();
            _visitActionHandler.Add.SetClickHandler(AddVisit);
            _visitActionHandler.Edit.SetClickHandler(UpdateSelectedVisit);
            _visitActionHandler.Delete.SetClickHandler(DeleteSelectedVisit);
            _visitActionHandler.Add.Enabled = true;

            LoadVisitsTable();

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public ITable Visits
        {
            get { return _visitTable; }
        }

        public VisitSummary CurrentVisitSelection
        {
            get { return _currentVisitSelection; }
            set
            {
                _currentVisitSelection = value;
                VisitSelectionChanged();
            }
        }

        public void SetSelectedVisit(ISelection selection)
        {
            this.CurrentVisitSelection = (VisitSummary)selection.Item;
        }

        private void VisitSelectionChanged()
        {
            if (_currentVisitSelection != null)
            {
                _visitActionHandler.Edit.Enabled = true;
                _visitActionHandler.Delete.Enabled = true;
            }
            else
            {
                _visitActionHandler.Edit.Enabled = false;
                _visitActionHandler.Delete.Enabled = false;
            }
        }


        public ActionModelNode VisitListActionModel
        {
            get { return _visitActionHandler; }
        }

        public void AddVisit()
        {
            try
            {
                VisitEditorComponent editor = new VisitEditorComponent(_patientRef);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddVisit);
                if (exitCode == ApplicationComponentExitCode.Normal)
                {
                    LoadVisitsTable();
                    this.Modified = true;
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void UpdateSelectedVisit()
        {             
            // can occur if user double clicks while holding control
            if (_currentVisitSelection == null) return;

            try
            {
                VisitEditorComponent editor = new VisitEditorComponent(_patientRef, _currentVisitSelection.VisitRef);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateVisit);
                if (exitCode == ApplicationComponentExitCode.Normal)
                {
                    LoadVisitsTable();
                    this.Modified = true;
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void DeleteSelectedVisit()
        {
            if (this.Host.ShowMessageBox(SR.MessageDeleteSelectedVisit, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                //delete the visit
                LoadVisitsTable();
                this.Modified = true;
            }
        }

        public void LoadVisitsTable()
        {
            _visitTable.Items.Clear();

            Platform.GetService<IVisitAdminService>(
                delegate(IVisitAdminService service)
                {
                    ListVisitsForPatientResponse response = service.ListVisitsForPatient(new ListVisitsForPatientRequest(_patientProfileRef));
                    _visitTable.Items.AddRange(response.Visits);
                });
        }

        public void Close()
        {
            this.ExitCode = ApplicationComponentExitCode.Normal;
            Host.Exit();
        }
    }
}
