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

            response.AdmissionTypeChoices = CollectionUtils.Map<AdmissionTypeEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IAdmissionTypeEnumBroker>().Load().Items,
                delegate(AdmissionTypeEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

            response.AmbulatoryStatusChoices = CollectionUtils.Map<AmbulatoryStatusEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IAmbulatoryStatusEnumBroker>().Load().Items,
                delegate(AmbulatoryStatusEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

            response.PatientClassChoices = CollectionUtils.Map<PatientClassEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IPatientClassEnumBroker>().Load().Items,
                delegate(PatientClassEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

            response.PatientTypeChoices = CollectionUtils.Map<PatientTypeEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IPatientTypeEnumBroker>().Load().Items,
                delegate(PatientTypeEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

            response.VisitLocationRoleChoices = CollectionUtils.Map<VisitLocationRoleEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IVisitLocationRoleEnumBroker>().Load().Items,
                delegate(VisitLocationRoleEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

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

            response.VisitPractitionerRoleChoices = CollectionUtils.Map<VisitPractitionerRoleEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IVisitPractitionerRoleEnumBroker>().Load().Items,
                delegate(VisitPractitionerRoleEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

            response.VisitStatusChoices = CollectionUtils.Map<VisitStatusEnum, EnumValueInfo, List<EnumValueInfo>>(
                PersistenceContext.GetBroker<IVisitStatusEnumBroker>().Load().Items,
                delegate(VisitStatusEnum e)
                {
                    return new EnumValueInfo(e.Code.ToString(), e.Value);
                });

            return response;
        }

        [ReadOperation]
        public ListVisitsForPatientResponse ListVisitsForPatient(ListVisitsForPatientRequest request)
        {
            PatientProfile profile = (PatientProfile)PersistenceContext.Load(request.PatientProfile);
            Patient patient = profile.Patient;

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
            broker.LoadAmbulatoryStatusesForVisit(visit);
            broker.LoadFacilityForVisit(visit);
            broker.LoadLocationsForVisit(visit);
            broker.LoadPractitionersForVisit(visit);
            //broker.LoadPatientForVisit(visit);

            VisitAssembler assembler = new VisitAssembler();
            return new LoadVisitForAdminEditResponse(visit.GetRef(), assembler.CreateVisitDetail(visit, PersistenceContext));
        }

        [UpdateOperation]
        public SaveAdminEditsForVisitResponse SaveAdminEditsForVisit(SaveAdminEditsForVisitRequest request)
        {
            Visit visit = (Visit)PersistenceContext.Load(request.VisitRef);

            VisitAssembler assembler = new VisitAssembler();
            assembler.UpdateVisit(visit, request.VisitDetail, PersistenceContext);

            return new SaveAdminEditsForVisitResponse(assembler.CreateVisitSummary(visit, PersistenceContext));
        }

        [UpdateOperation]
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
