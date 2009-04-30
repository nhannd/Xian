#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Security.Permissions;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.Admin.VisitAdmin
{
    [ServiceImplementsContract(typeof(IVisitAdminService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class VisitAdminService : ApplicationServiceBase, IVisitAdminService
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
            response.VisitNumberAssigningAuthorityChoices = EnumUtils.GetEnumValueList<InformationAuthorityEnum>(PersistenceContext);

            response.VisitPractitionerRoleChoices = EnumUtils.GetEnumValueList<VisitPractitionerRoleEnum>(PersistenceContext);
            response.VisitStatusChoices = EnumUtils.GetEnumValueList<VisitStatusEnum>(PersistenceContext);

            FacilityAssembler facilityAssembler = new FacilityAssembler();
            response.FacilityChoices = CollectionUtils.Map<Facility, FacilitySummary, List<FacilitySummary>>(
				PersistenceContext.GetBroker<IFacilityBroker>().FindAll(false),
                delegate(Facility f)
                {
                    return facilityAssembler.CreateFacilitySummary(f);
                });

			LocationAssembler locationAssembler = new LocationAssembler();
			response.CurrentLocationChoices = CollectionUtils.Map<Location, LocationSummary>(
				PersistenceContext.GetBroker<ILocationBroker>().FindAll(false),
				delegate(Location f)
				{
					return locationAssembler.CreateLocationSummary(f);
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
        public LoadVisitForEditResponse LoadVisitForEdit(LoadVisitForEditRequest request)
        {
            IVisitBroker broker = PersistenceContext.GetBroker<IVisitBroker>();

            Visit visit = broker.Load(request.VisitRef);
            VisitAssembler assembler = new VisitAssembler();
            return new LoadVisitForEditResponse(visit.GetRef(), assembler.CreateVisitDetail(visit, PersistenceContext));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Visit.Update)]
        public UpdateVisitResponse UpdateVisit(UpdateVisitRequest request)
        {
            Visit visit = PersistenceContext.Load<Visit>(request.VisitRef);

            VisitAssembler assembler = new VisitAssembler();
            assembler.UpdateVisit(visit, request.VisitDetail, PersistenceContext);

            return new UpdateVisitResponse(assembler.CreateVisitSummary(visit, PersistenceContext));
        }

        [UpdateOperation]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Visit.Create)]
		public AddVisitResponse AddVisit(AddVisitRequest request)
        {
            Visit visit = new Visit();

            VisitAssembler assembler = new VisitAssembler();
            assembler.UpdateVisit(visit, request.VisitDetail, PersistenceContext);

            PersistenceContext.Lock(visit, DirtyState.New);
            PersistenceContext.SynchState();

            return new AddVisitResponse(assembler.CreateVisitSummary(visit, PersistenceContext));
        }

        #endregion
    }
}
