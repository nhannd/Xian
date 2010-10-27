#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
