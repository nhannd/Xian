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
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;
using System.IO;

namespace ClearCanvas.Ris.Client
{
    public static class RandomUtils
    {
        private static Random _randomizer;
        private static Random Randomizer
        {
            get
            {
                if (_randomizer == null)
                    _randomizer = new Random(Platform.Time.Millisecond);

                return _randomizer;
            }
        }

        #region Basic Utilities

        public static int RandomInteger
        {
            get { return Randomizer.Next(); }
        }

        public static char RandomAlphabet
        {
            get { return Convert.ToChar(Convert.ToInt32(Randomizer.Next(0, 25) + 65)); }
        }

        public static string GenerateRandomIntegerString(int length)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                builder.Append(Randomizer.Next(0, 9).ToString());
            }

            return builder.ToString();
        }

        public static string GenerateRandomString(int length)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                builder.Append(RandomAlphabet);
            }

            return builder.ToString();
        }

        public static TItem ChooseRandom<TItem>(IList<TItem> target)
        {
            if (target.Count == 0)
                return default(TItem);

            if (target.Count == 1)
                return target[0];

            int randomIndex = Randomizer.Next(target.Count - 1);
            return target[randomIndex];
        }

        private static string GetRandomNameFromFile(string file)
        {
            List<string> nameList = new List<string>();

            using (TextReader reader = new StringReader(file))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    nameList.Add(line);
                }
            }

            return RandomUtils.ChooseRandom(nameList);
        }

        #endregion

        public static string FormatDateTime(DateTime dateTime, string format)
        {
            if (String.IsNullOrEmpty(format))
                format = "YYYYMMDDmmssxxx";

            string result = format;
            result = result.Replace("YYYY", dateTime.Year.ToString());
            result = result.Replace("MM", dateTime.Month.ToString("00"));
            result = result.Replace("DD", dateTime.Day.ToString("00"));
            result = result.Replace("hh", dateTime.Hour.ToString("00"));
            result = result.Replace("mm", dateTime.Minute.ToString("00"));
            result = result.Replace("ss", dateTime.Second.ToString("00"));
            result = result.Replace("xxx", dateTime.Millisecond.ToString("000"));
            return result.Trim();
        }

        public static PatientProfileSummary RandomPatientProfile()
        {
            PatientProfileDetail profile = null;

            Platform.GetService<IPatientAdminService>(
                delegate(IPatientAdminService service)
                {
                    LoadPatientProfileEditorFormDataResponse fromResponse = service.LoadPatientProfileEditorFormData(new LoadPatientProfileEditorFormDataRequest());

                    DateTime now = Platform.Time;

                    profile = new PatientProfileDetail();

                    profile.Mrn = new CompositeIdentifierDetail(
                        RandomUtils.FormatDateTime(now, null),
                        RandomUtils.ChooseRandom(fromResponse.MrnAssigningAuthorityChoices));

                    profile.Healthcard = new HealthcardDetail(
                        RandomUtils.GenerateRandomIntegerString(10),
                        RandomUtils.ChooseRandom(fromResponse.HealthcardAssigningAuthorityChoices),
                        "", null);

                    profile.DateOfBirth = now;
                    profile.Sex = RandomUtils.ChooseRandom(fromResponse.SexChoices);
                    profile.PrimaryLanguage = RandomUtils.ChooseRandom(fromResponse.PrimaryLanguageChoices);
                    profile.Religion = RandomUtils.ChooseRandom(fromResponse.ReligionChoices);
                    profile.DeathIndicator = false;
                    profile.TimeOfDeath = null;

                    string givenName;
                    string familyName = GetRandomNameFromFile(RandomUtilsSettings.Default.FamilyNameDictionary);
                    if (profile.Sex.Code == "F")
                        givenName = GetRandomNameFromFile(RandomUtilsSettings.Default.FemaleNameDictionary);
                    else
                        givenName = GetRandomNameFromFile(RandomUtilsSettings.Default.MaleNameDictionary);

                    givenName += " Anonymous";
                    profile.Name = new PersonNameDetail();
                    profile.Name.FamilyName = familyName;
                    profile.Name.GivenName = givenName;
                });

            AdminAddPatientProfileResponse addResponse = null;

            Platform.GetService<IPatientAdminService>(
                delegate(IPatientAdminService service)
                {
                    addResponse = service.AdminAddPatientProfile(new AdminAddPatientProfileRequest(profile));
                });

            PatientProfileSummary summary = new PatientProfileSummary();
            summary.PatientRef = addResponse.PatientRef;
            summary.PatientProfileRef = addResponse.PatientProfileRef;
            summary.Mrn = profile.Mrn;
            summary.Name = profile.Name;
            summary.Healthcard = profile.Healthcard;
            summary.DateOfBirth = profile.DateOfBirth;
            summary.Sex = profile.Sex;

            return summary;
        }

        public static VisitSummary RandomVisit(EntityRef patientRef, EntityRef profileRef, EnumValueInfo assigningAuthority)
        {
            VisitSummary visit = null;

            // choose from existing visits
            Platform.GetService<IOrderEntryService>(
                delegate(IOrderEntryService service)
                    {
                        ListActiveVisitsForPatientRequest request = new ListActiveVisitsForPatientRequest(patientRef);

                        ListActiveVisitsForPatientResponse visitResponse = service.ListActiveVisitsForPatient(request);
                        visit = ChooseRandom(visitResponse.Visits);
                    });

            if (visit != null)
                return visit;
                    
            // Generate an active visit with randomize properties
            Platform.GetService<IVisitAdminService>(
                delegate(IVisitAdminService service)
                {
                    LoadVisitEditorFormDataResponse visitFormResponse = service.LoadVisitEditorFormData(new LoadVisitEditorFormDataRequest());

                    DateTime now = Platform.Time;

                    VisitDetail visitDetail = new VisitDetail();
                    visitDetail.PatientRef = patientRef;
                    visitDetail.VisitNumber = new CompositeIdentifierDetail(FormatDateTime(now, null), assigningAuthority);
                    visitDetail.PatientClass = ChooseRandom(visitFormResponse.PatientClassChoices);
                    visitDetail.PatientType = ChooseRandom(visitFormResponse.PatientTypeChoices);
                    visitDetail.AdmissionType = ChooseRandom(visitFormResponse.AdmissionTypeChoices);
                    visitDetail.Status = CollectionUtils.SelectFirst<EnumValueInfo>(visitFormResponse.VisitStatusChoices,
                        delegate(EnumValueInfo enumValue)
                        {
                            return enumValue.Code == "AA";
                        });
                    visitDetail.AdmitDateTime = now;
                    visitDetail.Facility = ChooseRandom(visitFormResponse.FacilityChoices);

                    AdminAddVisitResponse addVisitResponse = service.AdminAddVisit(new AdminAddVisitRequest(visitDetail));
                    visit = addVisitResponse.AddedVisit;
                });

            return visit;
        }

        public static EntityRef RandomOrder(VisitSummary visit, string diagnosticServiceName)
        {
            List<DiagnosticServiceSummary> diagnosticServiceChoices = null;
            Platform.GetService<IDiagnosticServiceAdminService>(
                delegate(IDiagnosticServiceAdminService service)
                {
                    // get the diagnostic service by name, or if name is null, just load the first 100
                    // so that we can choose a random one
                    ListDiagnosticServicesRequest request = new ListDiagnosticServicesRequest(diagnosticServiceName, null);
                    request.Page.FirstRow = 0;
                    request.Page.MaxRows = 100;
                    diagnosticServiceChoices = service.ListDiagnosticServices(request).DiagnosticServices;
                });

            List<ExternalPractitionerSummary> practitionerChoices = null;
            Platform.GetService<IExternalPractitionerAdminService>(
                delegate(IExternalPractitionerAdminService service)
                {
                    ListExternalPractitionersRequest request = new ListExternalPractitionersRequest();
                    request.Page.FirstRow = 0;
                    request.Page.MaxRows = 100;
                    practitionerChoices = service.ListExternalPractitioners(request).Practitioners;
                });

            EntityRef orderRef = null;
            Platform.GetService<IOrderEntryService>(
                delegate(IOrderEntryService service)
                {
                    GetOrderEntryFormDataResponse formChoicesResponse = service.GetOrderEntryFormData(new GetOrderEntryFormDataRequest(visit.PatientRef));

                    DiagnosticServiceSummary diagnosticService;
                    if (String.IsNullOrEmpty(diagnosticServiceName))
                    {
                        diagnosticService = ChooseRandom(diagnosticServiceChoices);                        
                    }
                    else
                    {
                        diagnosticService = CollectionUtils.SelectFirst<DiagnosticServiceSummary>(
                            diagnosticServiceChoices,
                            delegate(DiagnosticServiceSummary ds)
                            {
                                return ds.Name == diagnosticServiceName;
                            });

                        if (diagnosticService == null)
                            throw new Exception(String.Format("Cannot find diagnostic service with name {0}", diagnosticServiceName));
                    }

                    FacilitySummary randomFacility = ChooseRandom(formChoicesResponse.FacilityChoices);
                    ExternalPractitionerSummary randomPhysician = ChooseRandom(practitionerChoices);
                    EnumValueInfo randomPriority = ChooseRandom(formChoicesResponse.OrderPriorityChoices);

                    OrderRequisition requisition = new OrderRequisition();
                    requisition.Patient = visit.PatientRef;
                    requisition.Visit = visit;
                    requisition.DiagnosticService = diagnosticService;
                    requisition.OrderingPractitioner = randomPhysician;
                    requisition.OrderingFacility = randomFacility;
                    requisition.Priority = randomPriority;
                    requisition.ReasonForStudy = "Randomly generated test order";
                    requisition.SchedulingRequestTime = Platform.Time;

                    LoadDiagnosticServiceBreakdownResponse dsResponse = service.LoadDiagnosticServiceBreakdown(
                        new LoadDiagnosticServiceBreakdownRequest(diagnosticService.DiagnosticServiceRef));

                    requisition.RequestedProcedures = new List<ProcedureRequisition>();
                    requisition.RequestedProcedures.AddRange(
                       CollectionUtils.Map<RequestedProcedureTypeDetail, ProcedureRequisition>(
                           dsResponse.DiagnosticServiceDetail.RequestedProcedureTypes,
                           delegate(RequestedProcedureTypeDetail rpt)
                           {
                               ProcedureRequisition req = new ProcedureRequisition(rpt.GetSummary(), randomFacility);
                               req.ScheduledTime = Platform.Time;
                               return req;
                           }));
                   
                    requisition.CopiesToPractitioners = new List<ExternalPractitionerSummary>();
                    requisition.Attachments = new List<OrderAttachmentSummary>();
                    requisition.Notes = new List<OrderNoteDetail>();

                    PlaceOrderResponse response = service.PlaceOrder(new PlaceOrderRequest(requisition));

                    orderRef = response.OrderRef;
                });

            return orderRef;
        }
    }
}
