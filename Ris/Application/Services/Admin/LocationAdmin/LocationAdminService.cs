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
using ClearCanvas.Ris.Application.Common.Admin.LocationAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin.LocationAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class LocationAdminService : ApplicationServiceBase, ILocationAdminService
    {
        /// <summary>
        /// Return all location options
        /// </summary>
        /// <returns></returns>
        [ReadOperation]
        public ListAllLocationsResponse ListAllLocations(ListAllLocationsRequest request)
        {
            LocationAssembler assembler = new LocationAssembler();
            return new ListAllLocationsResponse(
                CollectionUtils.Map<LocationAdmin, LocationSummary, List<LocationSummary>>(
                    PersistenceContext.GetBroker<ILocationBroker>().FindAll(),
                    delegate(LocationAdmin l)
                    {
                        return assembler.CreateLocationSummary(l);
                    }));
        }

        [ReadOperation]
        public GetLocationEditFormDataResponse GetLocationEditFormData(GetLocationEditFormDataRequest request)
        {
            FacilityAssembler assembler = new FacilityAssembler();
            return new GetLocationEditFormDataResponse(
                CollectionUtils.Map<FacilityAdmin, FacilitySummary, List<FacilitySummary>>(
                    PersistenceContext.GetBroker<IFacilityBroker>().FindAll(),
                    delegate(FacilityAdmin f)
                    {
                        return assembler.CreateFacilitySummary(f);
                    }));
        }

        [ReadOperation]
        public LoadLocationForEditResponse LoadLocationForEdit(LoadLocationForEditRequest request)
        {
            // note that the version of the LocationRef is intentionally ignored here (default behaviour of ReadOperation)
            LocationAdmin l = (LocationAdmin)PersistenceContext.Load(request.LocationRef);
            LocationAssembler assembler = new LocationAssembler();

            return new LoadLocationForEditResponse(l.GetRef(), assembler.CreateLocationDetail(l));
        }

        /// <summary>
        /// Add the specified location
        /// </summary>
        /// <param name="location"></param>
        [UpdateOperation]
        public AddLocationResponse AddLocation(AddLocationRequest request)
        {
            LocationAdmin location = new LocationAdmin();
            LocationAssembler assembler = new LocationAssembler();
            assembler.UpdateLocation(locationm, request.LocationDetail, PersistenceContext);

            PersistenceContext.Lock(location, DirtyState.New);
            PersistenceContext.SynchState();

            return new AddLocationResponse(assembler.CreateLocationSummary(location));
        }


        /// <summary>
        /// Update the specified location
        /// </summary>
        /// <param name="location"></param>
        [UpdateOperation]
        public UpdateLocationResponse UpdateLocation(UpdateLocationRequest request)
        {
            LocationAdmin location = (LocationAdmin)PersistenceContext.Load(request.LocationRef, EntityLoadFlags.CheckVersion);

            LocationAssembler assembler = new LocationAssembler();
            assembler.UpdateLocation(locationm, request.LocationDetail, PersistenceContext);

            return new UpdateLocationResponse(assembler.CreateLocationSummary(location));
        }
    }
}
