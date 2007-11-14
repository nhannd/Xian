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
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.Admin;
using System.Security.Permissions;

namespace ClearCanvas.Ris.Application.Services.Admin.VisitAdmin
{
    [ServiceImplementsContract(typeof(IVisitAdminService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    class VisitAdminService : ApplicationServiceBase, IVisitAdminService
    {
        #region IVisitAdminService Members

        [ReadOperation]
        public LoadVisitEditorFormDataResponse LoadVisitEditorFormData(LoadVisitEditorFormDataRequest request)
        {
            // ignore request

            LoadVisitEditorFormDataResponse response = new LoadVisitEditorFormDataResponse();

            response.AdmissionTypeChoices = EnumUtils.GetEnumValueList<AdmissionTypeEnum>(PersistenceContext);
            response.AmbulatoryStatusChoices = EnumUtils.GetEnumValueList<AmbulatoryStatusEnum>(PersistenceContext);
            response.PatientClassChoices = EnumUtils.GetEnumValueList<PatientClassEnum>(PersistenceContext);
            response.PatientTypeChoices = EnumUtils.GetEnumValueList<PatientTypeEnum>(PersistenceContext);
            response.VisitLocationRoleChoices = EnumUtils.GetEnumValueList<VisitLocationRoleEnum>(PersistenceContext);

            //TODO:  replace dummy values
            List<string> dummyVisitNumberChoices = new List<string>();
            dummyVisitNumberChoices.Add("UHN");
            dummyVisitNumberChoices.Add("MSH");
            dummyVisitNumberChoices.Add("WCH");
            response.VisitNumberAssigningAuthorityChoices = dummyVisitNumberChoices;
            //response.VisitNumberAssigningAuthorityChoices = CollectionUtils.Map<XXXXXX, EnumValueInfo>(
            //    PersistenceContext.GetBroker<XXXXXXXX>().Load().Items,
            //    delegate(XXXXXXXXXX e)
            //    {
            //        return new EnumValueInfo(e.Code.ToString(), e.Value);
            //    });

            response.VisitPractitionerRoleChoices = EnumUtils.GetEnumValueList<VisitPractitionerRoleEnum>(PersistenceContext);
            response.VisitStatusChoices = EnumUtils.GetEnumValueList<VisitStatusEnum>(PersistenceContext);

            FacilityAssembler facilityAssembler = new FacilityAssembler();
            response.FacilityChoices = CollectionUtils.Map<Facility, FacilitySummary, List<FacilitySummary>>(
                PersistenceContext.GetBroker<IFacilityBroker>().FindAll(),
                delegate(Facility f)
                {
                    return facilityAssembler.CreateFacilitySummary(f);
                });

            return response;
        }

        [ReadOperation]
        public ListVisitsForPatientResponse ListVisitsForPatient(ListVisitsForPatientRequest request)
        {
            Patient patient = PersistenceContext.Load<Patient>(request.PatientRef);

            VisitSearchCriteria criteria = new VisitSearchCriteria();
            criteria.Patient.EqualTo(patient);

            IList<Visit> visits = PersistenceContext.GetBroker<IVisitBroker>().Find(criteria);

            VisitAssembler assembler = new VisitAssembler();
            ListVisitsForPatientResponse response = new ListVisitsForPatientResponse();
            response.Visits = new List<VisitSummary>();
            foreach (Visit v in visits)
            {
                response.Visits.Add(assembler.CreateVisitSummary(v, PersistenceContext));
            }

            return response;
        }

        [ReadOperation]
        public LoadVisitForAdminEditResponse LoadVisitForAdminEdit(LoadVisitForAdminEditRequest request)
        {
            IVisitBroker broker = PersistenceContext.GetBroker<IVisitBroker>();

            Visit visit = broker.Load(request.VisitRef);
            VisitAssembler assembler = new VisitAssembler();
            return new LoadVisitForAdminEditResponse(visit.GetRef(), assembler.CreateVisitDetail(visit, PersistenceContext));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.VisitAdmin)]
        public SaveAdminEditsForVisitResponse SaveAdminEditsForVisit(SaveAdminEditsForVisitRequest request)
        {
            Visit visit = PersistenceContext.Load<Visit>(request.VisitRef);

            VisitAssembler assembler = new VisitAssembler();
            assembler.UpdateVisit(visit, request.VisitDetail, PersistenceContext);

            return new SaveAdminEditsForVisitResponse(assembler.CreateVisitSummary(visit, PersistenceContext));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.VisitAdmin)]
        public AdminAddVisitResponse AdminAddVisit(AdminAddVisitRequest request)
        {
            Visit visit = new Visit();

            VisitAssembler assembler = new VisitAssembler();
            assembler.UpdateVisit(visit, request.VisitDetail, PersistenceContext);

            PersistenceContext.Lock(visit, DirtyState.New);
            PersistenceContext.SynchState();

            return new AdminAddVisitResponse(assembler.CreateVisitSummary(visit, PersistenceContext));
        }

        #endregion
    }
}
