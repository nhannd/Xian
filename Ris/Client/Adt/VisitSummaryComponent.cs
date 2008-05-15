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

using System;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Client;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

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
        private readonly EntityRef _patientRef;

        private VisitSummaryTable _visitTable;
        private VisitSummary _selectedVisit;

        private CrudActionModel _visitActionHandler;

        public VisitSummaryComponent(EntityRef patientRef)
        {
            _patientRef = patientRef;
        }

        public override void Start()
        {
            _visitTable = new VisitSummaryTable();

            _visitActionHandler = new CrudActionModel(true, true, false);
            _visitActionHandler.Add.SetClickHandler(AddVisit);
			_visitActionHandler.Add.SetPermissibility(AuthorityTokens.Workflow.Visit.Create);
			_visitActionHandler.Add.Enabled = true;

            _visitActionHandler.Edit.SetClickHandler(UpdateSelectedVisit);
			_visitActionHandler.Add.SetPermissibility(AuthorityTokens.Workflow.Visit.Update);

            _visitTable.Items.Clear();

            Platform.GetService<IVisitAdminService>(
                delegate(IVisitAdminService service)
                {
                    ListVisitsForPatientResponse response = service.ListVisitsForPatient(new ListVisitsForPatientRequest(_patientRef));
                    _visitTable.Items.AddRange(response.Visits);
                });

            base.Start();
        }

        public ITable Visits
        {
            get { return _visitTable; }
        }

        public ISelection SelectedVisit
        {
            get { return new Selection(_selectedVisit); }
            set
            {
                _selectedVisit = (VisitSummary) value.Item;
                VisitSelectionChanged();
            }
        }

        private void VisitSelectionChanged()
        {
            if (_selectedVisit != null)
            {
                _visitActionHandler.Edit.Enabled = true;
                _visitActionHandler.Delete.Enabled = true;
            }
            else
            {
                _visitActionHandler.Edit.Enabled = false;
                _visitActionHandler.Delete.Enabled = false;
            }

            NotifyPropertyChanged("SelectedVisit");
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
                ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddVisit);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _visitTable.Items.Add(editor.AddedVisit);
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
            if (_selectedVisit == null) return;

            try
            {
                VisitEditorComponent editor = new VisitEditorComponent(_selectedVisit);
                ApplicationComponentExitCode exitCode = LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateVisit);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    // delete and re-insert to ensure that TableView updates correctly
                    VisitSummary toBeRemoved = _selectedVisit;
                    _visitTable.Items.Remove(toBeRemoved);
                    _visitTable.Items.Add(editor.AddedVisit);
                    this.Modified = true;
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void Close()
        {
            if (this.Modified)
                this.ExitCode = ApplicationComponentExitCode.Accepted;

            Host.Exit();
        }
    }
}
