using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common;
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
            try
            {
                Platform.GetService<IPatientAdminService>(
                    delegate(IPatientAdminService service)
                    {
                        LoadPatientProfileForAdminEditResponse response = service.LoadPatientProfileForAdminEdit(new LoadPatientProfileForAdminEditRequest(_patientProfileRef));
                        _patientRef = response.PatientRef;
                        _patientProfileRef = response.PatientProfileRef;

                        //TODO: PersonNameDetail formatting
                        this.Host.SetTitle(string.Format(SR.TitleVisitSummaryComponent,
                            PersonNameFormat.Format(response.PatientDetail.Name), 
                            String.Format("{0} {1}", response.PatientDetail.MrnAssigningAuthority, response.PatientDetail.Mrn)));                    
                    });

                _visitTable = new VisitSummaryTable();

                _visitActionHandler = new CrudActionModel();
                _visitActionHandler.Add.SetClickHandler(AddVisit);
                _visitActionHandler.Edit.SetClickHandler(UpdateSelectedVisit);
                _visitActionHandler.Delete.SetClickHandler(DeleteSelectedVisit);
                _visitActionHandler.Add.Enabled = true;

            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

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
            VisitEditorComponent editor = new VisitEditorComponent(_patientRef);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddVisit);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                LoadVisitsTable();
                this.Modified = true;
            }
        }

        public void UpdateSelectedVisit()
        {             
            // can occur if user double clicks while holding control
            if (_currentVisitSelection == null) return;

            VisitEditorComponent editor = new VisitEditorComponent(_patientRef, _currentVisitSelection.entityRef);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateVisit);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                LoadVisitsTable();
                this.Modified = true;
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

            try
            {
                Platform.GetService<IVisitAdminService>(
                    delegate(IVisitAdminService service)
                    {
                        ListVisitsForPatientResponse response = service.ListVisitsForPatient(new ListVisitsForPatientRequest(_patientProfileRef));
                        _visitTable.Items.AddRange(response.Visits);
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void Close()
        {
            this.ExitCode = ApplicationComponentExitCode.Normal;
            Host.Exit();
        }
    }
}
