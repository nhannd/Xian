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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;

namespace ClearCanvas.Ris.Client.Adt
{
    public class VisitEditorComponent : NavigatorComponentContainer
    {
        private EntityRef _patientRef;
        private EntityRef _visitRef;
        private VisitDetail _visit;
        private VisitSummary _addedVisit;

        private VisitDetailsEditorComponent _visitEditor;

        private readonly bool _isNew;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitEditorComponent(EntityRef patientRef)
        {
            _isNew = true;
            _patientRef = patientRef;
        }

        public VisitEditorComponent(VisitSummary editVisit)
        {
            _isNew = false;
            _patientRef = editVisit.PatientRef;
            _visitRef = editVisit.VisitRef;
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
                            response.VisitStatusChoices,
							response.FacilityChoices,
							response.CurrentLocationChoices)));

					// JR (may 2008): these pages are not currently needed, and they are not complete, so
					// better just to remove them for now

					//this.Pages.Add(new NavigatorPage("Visit/Practitioners", 
					//    _visitPractionersSummary = new VisitPractitionersSummaryComponent(
					//        response.VisitPractitionerRoleChoices
					//    )));

					//this.Pages.Add(new NavigatorPage("Visit/Location", 
					//    _visitLocationsSummary = new VisitLocationsSummaryComponent(
					//        response.VisitLocationRoleChoices
					//    )));

                    if (_isNew)
                    {
                        _visit = new VisitDetail();
                        _visit.PatientRef = _patientRef;
                        _visit.VisitNumber.AssigningAuthority = response.VisitNumberAssigningAuthorityChoices.Count > 0 ?
                            response.VisitNumberAssigningAuthorityChoices[0] : null;
                        _visit.PatientClass = response.PatientClassChoices[0];
                        _visit.PatientType = response.PatientTypeChoices[0];
                        _visit.AdmissionType = response.AdmissionTypeChoices[0];
                        _visit.Status = response.VisitStatusChoices[0];
                        _visit.Facility = response.FacilityChoices[0];
                    }
                    else
                    {
                        LoadVisitForEditResponse loadVisitResponse = service.LoadVisitForEdit(new LoadVisitForEditRequest(_visitRef));
                        _patientRef = loadVisitResponse.Patient;
                        _visitRef = loadVisitResponse.VisitRef;
                        _visit = loadVisitResponse.VisitDetail;
                    }

                });

            _visitEditor.Visit = _visit;
			//_visitPractionersSummary.Visit = _visit;
			//_visitLocationsSummary.Visit = _visit;

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }
        
        public VisitSummary VisitSummary
        {
            get { return _addedVisit; }
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
                            AddVisitResponse response = service.AddVisit(new AddVisitRequest(_visit));
                            _addedVisit = response.Visit;
                            _patientRef = response.Visit.PatientRef;
                            _visitRef = response.Visit.VisitRef;
                        }
                        else
                        {
                            UpdateVisitResponse response = service.UpdateVisit(new UpdateVisitRequest(_visitRef, _visit));
                            _addedVisit = response.Visit;
                            _patientRef = response.Visit.PatientRef;
                            _visitRef = response.Visit.VisitRef;
                        }
                    });
                this.Exit(ApplicationComponentExitCode.Accepted);
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
            this.ExitCode = ApplicationComponentExitCode.None;
            this.Host.Exit();
        }
    }
}
