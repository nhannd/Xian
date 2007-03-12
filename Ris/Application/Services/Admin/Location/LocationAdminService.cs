using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.Location;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class LocationAdminService : ApplicationServiceBase, ILocationAdminService
    {
        /// <summary>
        /// Return all location options
        /// </summary>
        /// <returns></returns>
        [ReadOperation]
        public GetAllLocationsResponse GetAllLocations(GetAllLocationsRequest request)
        {
            LocationAssembler assembler = new LocationAssembler();
            return new GetAllLocationsResponse(
                CollectionUtils.Map<Location, LocationSummary, List<LocationSummary>>(
                    PersistenceContext.GetBroker<ILocationBroker>().FindAll(),
                    delegate(Location l)
                    {
                        return assembler.CreateLocationSummary(l);
                    }));
        }

        [ReadOperation]
        public GetLocationEditFormDataResponse GetLocationEditFormData(GetLocationEditFormDataRequest request)
        {
            FacilityAssembler assembler = new FacilityAssembler();
            return new GetLocationEditFormDataResponse(
                CollectionUtils.Map<Facility, FacilitySummary, List<FacilitySummary>>(
                    PersistenceContext.GetBroker<IFacilityBroker>().FindAll(),
                    delegate(Facility f)
                    {
                        return assembler.CreateFacilitySummary(f);
                    }));
        }

        [ReadOperation]
        public LoadLocationForEditResponse LoadLocationForEdit(LoadLocationForEditRequest request)
        {
            Location l = (Location)PersistenceContext.Load(request.LocationRef);
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
            Location location = new Location();
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
            Location location = (Location)PersistenceContext.Load(request.LocationRef, EntityLoadFlags.CheckVersion);

            LocationAssembler assembler = new LocationAssembler();
            assembler.UpdateLocation(locationm, request.LocationDetail, PersistenceContext);

            return new UpdateLocationResponse(assembler.CreateLocationSummary(location));
        }
    }
}
