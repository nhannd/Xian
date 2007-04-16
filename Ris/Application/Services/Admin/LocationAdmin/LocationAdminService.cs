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
using System.Security.Permissions;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services.Admin.LocationAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(ILocationAdminService))]
    public class LocationAdminService : ApplicationServiceBase, ILocationAdminService
    {
        #region ILocationAdminService Members

        /// <summary>
        /// Return all location options
        /// </summary>
        /// <returns></returns>
        [ReadOperation]
        public ListAllLocationsResponse ListAllLocations(ListAllLocationsRequest request)
        {
            LocationAssembler assembler = new LocationAssembler();
            return new ListAllLocationsResponse(
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
            // note that the version of the LocationRef is intentionally ignored here (default behaviour of ReadOperation)
            Location l = (Location)PersistenceContext.Load(request.LocationRef);
            LocationAssembler assembler = new LocationAssembler();

            return new LoadLocationForEditResponse(l.GetRef(), assembler.CreateLocationDetail(l));
        }

        /// <summary>
        /// Add the specified location
        /// </summary>
        /// <param name="location"></param>
        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.LocationAdmin)]
        public AddLocationResponse AddLocation(AddLocationRequest request)
        {
            Location location = new Location();

            LocationAssembler assembler = new LocationAssembler();
            assembler.UpdateLocation(request.LocationDetail, location, PersistenceContext);

            CheckForDuplicateLocation(location);

            PersistenceContext.Lock(location, DirtyState.New);

            // ensure the new location is assigned an OID before using it in the return value
            PersistenceContext.SynchState();

            return new AddLocationResponse(assembler.CreateLocationSummary(location));
        }


        /// <summary>
        /// Update the specified location
        /// </summary>
        /// <param name="location"></param>
        [UpdateOperation]
        [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.LocationAdmin)]
        public UpdateLocationResponse UpdateLocation(UpdateLocationRequest request)
        {
            Location location = (Location)PersistenceContext.Load(request.LocationRef, EntityLoadFlags.CheckVersion);

            LocationAssembler assembler = new LocationAssembler();
            assembler.UpdateLocation(request.LocationDetail, location, PersistenceContext);

            CheckForDuplicateLocation(location);

            return new UpdateLocationResponse(assembler.CreateLocationSummary(location));
        }

        #endregion

        /// <summary>
        /// Helper method to check that the location with the exact same content does not already exist
        /// </summary>
        /// <param name="subject"></param>
        private void CheckForDuplicateLocation(Location subject)
        {
            try
            {
                LocationSearchCriteria where = new LocationSearchCriteria();
                where.Facility.EqualTo(subject.Facility);
                where.Building.EqualTo(subject.Building);
                where.Floor.EqualTo(subject.Floor);
                where.PointOfCare.EqualTo(subject.PointOfCare);
                where.Room.EqualTo(subject.Room);
                where.Bed.EqualTo(subject.Bed);

                Location duplicate = PersistenceContext.GetBroker<ILocationBroker>().FindOne(where);
                if (duplicate != subject)
                    throw new RequestValidationException(string.Format(SR.ExceptionLocationAlreadyExist));
            }
            catch (EntityNotFoundException)
            {
                // no duplicates
            }
        }
    }
}
