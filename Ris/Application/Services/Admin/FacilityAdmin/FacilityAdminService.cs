using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;
using System.Security.Permissions;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services.Admin.FacilityAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IFacilityAdminService))]
    public class FacilityAdminService : ApplicationServiceBase, IFacilityAdminService
    {
        [ReadOperation]
        public ListAllFacilitiesResponse ListAllFacilities(ListAllFacilitiesRequest request)
        {
            FacilityAssembler assembler = new FacilityAssembler();
            return new ListAllFacilitiesResponse(
                CollectionUtils.Map<Facility, FacilitySummary, List<FacilitySummary>>(
                    PersistenceContext.GetBroker<IFacilityBroker>().FindAll(),
                    delegate(Facility f)
                    {
                        return assembler.CreateFacilitySummary(f);
                    }));
        }

        [ReadOperation]
        public LoadFacilityForEditResponse LoadFacilityForEdit(LoadFacilityForEditRequest request)
        {
            // note that the version of the FacilityRef is intentionally ignored here (default behaviour of ReadOperation)
            Facility f = (Facility)PersistenceContext.Load(request.FacilityRef);
            FacilityAssembler assembler = new FacilityAssembler();

            return new LoadFacilityForEditResponse(f.GetRef(), assembler.CreateFacilityDetail(f));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.FacilityAdmin)]
        public AddFacilityResponse AddFacility(AddFacilityRequest request)
        {
            Facility facility = new Facility();
            FacilityAssembler assembler = new FacilityAssembler();
            assembler.UpdateFacility(request.FacilityDetail, facility);

            // TODO prior to accepting this add request, we should check that the same facility does not already exist

            PersistenceContext.Lock(facility, DirtyState.New);

            // ensure the new facility is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new AddFacilityResponse(assembler.CreateFacilitySummary(facility));
        }

        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.FacilityAdmin)]
        public UpdateFacilityResponse UpdateFacility(UpdateFacilityRequest request)
        {
            Facility facility = (Facility)PersistenceContext.Load(request.FacilityRef, EntityLoadFlags.CheckVersion);

            FacilityAssembler assembler = new FacilityAssembler();
            assembler.UpdateFacility(request.FacilityDetail, facility);

            // TODO prior to accepting this update request, we should check that the same facility does not already exist

            return new UpdateFacilityResponse(assembler.CreateFacilitySummary(facility));
        }

    }
}
