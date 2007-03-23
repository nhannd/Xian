using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
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

        private bool _isNew;

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
            try
            {
                Platform.GetService<IVisitAdminService>(
                    delegate(IVisitAdminService service)
                    {
                        if (_isNew)
                        {
                            _visit = new VisitDetail();
                            _visit.Patient = _patientRef;
                        }
                        else
                        {
                            LoadVisitForAdminEditResponse response = service.LoadVisitForAdminEdit(new LoadVisitForAdminEditRequest(_visitRef));
                            _patientRef = response.Patient;
                            _visitRef = response.VisitRef;
                            _visit = response.VisitDetail;
                        }

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
                                resposne.VisitPractitionerRoleChoices
                            )));

                        this.Pages.Add(new NavigatorPage("Visit/Location", 
                            _visitLocationsSummary = new VisitLocationsSummaryComponent(
                                resposne.VisitLocationRoleChoices
                            )));

                    });

                _visitEditor.Visit = _visit;
                _visitPractionersSummary.Visit = _visit;
                _visitLocationsSummary.Visit = _visit;
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public override void Accept()
        {
            try
            {
                SaveChanges();
                this.ExitCode = ApplicationComponentExitCode.Normal;
            }
            catch (ConcurrencyException e)
            {
                ExceptionHandler.Report(e, SR.ExceptionConcurrencyVisitNotSaved, this.Host.DesktopWindow);
                this.ExitCode = ApplicationComponentExitCode.Error;
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionFailedToSave, this.Host.DesktopWindow);
                this.ExitCode = ApplicationComponentExitCode.Error;
            }
            this.Host.Exit();
        }

        public override void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        private void SaveChanges()
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
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }
    }
}
