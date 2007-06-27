using System;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;

namespace ClearCanvas.Ris.Client.Adt
{
    public class VisitEditorComponent : NavigatorComponentContainer
    {
        private EntityRef _patientRef;
        private EntityRef _visitRef;
        private VisitDetail _visit;

        private VisitDetailsEditorComponent _visitEditor;
        private VisitPractitionersSummaryComponent _visitPractionersSummary;
        private VisitLocationsSummaryComponent _visitLocationsSummary;

        private readonly bool _isNew;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitEditorComponent(EntityRef patientRef, EntityRef visitRef)
        {
            _isNew = false;
            _patientRef = patientRef;
            _visitRef = visitRef;
        }

        public VisitEditorComponent(EntityRef patientRef)
        {
            _isNew = true;
            _patientRef = patientRef;
        }

        public override void Start()
        {
            Platform.GetService<IVisitAdminService>(
                delegate(IVisitAdminService service)
                {
                    LoadVisitEditorFormDataResponse response = service.LoadVisitEditorFormData(new LoadVisitEditorFormDataRequest());

                    this.Pages.Add(new NavigatorPage("Visit",
                        _visitEditor = new VisitDetailsEditorComponent(
                            response.VisitNumberAssigningAuthorityChoices,
                            response.PatientClassChoices,
                            response.PatientTypeChoices,
                            response.AdmissionTypeChoices,
                            response.AmbulatoryStatusChoices,
                            response.VisitStatusChoices)));

                    this.Pages.Add(new NavigatorPage("Visit/Practitioners", 
                        _visitPractionersSummary = new VisitPractitionersSummaryComponent(
                            response.VisitPractitionerRoleChoices
                        )));

                    this.Pages.Add(new NavigatorPage("Visit/Location", 
                        _visitLocationsSummary = new VisitLocationsSummaryComponent(
                            response.VisitLocationRoleChoices
                        )));

                    if (_isNew)
                    {
                        _visit = new VisitDetail();
                        _visit.Patient = _patientRef;
                        _visit.VisitNumberAssigningAuthority = response.VisitNumberAssigningAuthorityChoices[0];
                        _visit.PatientClass = response.PatientClassChoices[0];
                        _visit.PatientType = response.PatientTypeChoices[0];
                        _visit.AdmissionType = response.AdmissionTypeChoices[0];
                        _visit.Status = response.VisitStatusChoices[0];
                        _visit.Facility = response.FacilityChoices[0];
                    }
                    else
                    {
                        LoadVisitForAdminEditResponse loadVisitResponse = service.LoadVisitForAdminEdit(new LoadVisitForAdminEditRequest(_visitRef));
                        _patientRef = loadVisitResponse.Patient;
                        _visitRef = loadVisitResponse.VisitRef;
                        _visit = loadVisitResponse.VisitDetail;
                    }

                });

            _visitEditor.Visit = _visit;
            _visitPractionersSummary.Visit = _visit;
            _visitLocationsSummary.Visit = _visit;

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public override void Accept()
        {
            try
            {
                Platform.GetService<IVisitAdminService>(
                    delegate(IVisitAdminService service)
                    {
                        if (_isNew)
                        {
                            AdminAddVisitResponse response = service.AdminAddVisit(new AdminAddVisitRequest(_visit));
                            _patientRef = response.AddedVisit.Patient;
                            _visitRef = response.AddedVisit.entityRef;
                        }
                        else
                        {
                            SaveAdminEditsForVisitResponse response = service.SaveAdminEditsForVisit(new SaveAdminEditsForVisitRequest(_visitRef, _visit));
                            _patientRef = response.AddedVisit.Patient;
                            _visitRef = response.AddedVisit.entityRef;
                        }
                    });
                this.Host.Exit();
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionCannotAddUpdateVisit, this.Host.DesktopWindow,
                    delegate
                    {
                        this.ExitCode = ApplicationComponentExitCode.Error;
                        this.Host.Exit();
                    });
            }
        }

        public override void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            this.Host.Exit();
        }
    }
}
