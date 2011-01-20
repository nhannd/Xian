#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class LocationAssembler
    {
        public LocationSummary CreateLocationSummary(Location location)
        {
            FacilitySummary facility = new FacilityAssembler().CreateFacilitySummary(location.Facility);
            return new LocationSummary(
                location.GetRef(),
				location.Id,
				location.Name,
                facility,
                location.Building,
                location.Floor,
                location.PointOfCare,
                location.Room,
                location.Bed,
				location.Deactivated);
        }

        public LocationDetail CreateLocationDetail(Location location)
        {
            FacilitySummary facility = new FacilityAssembler().CreateFacilitySummary(location.Facility);
            return new LocationDetail(
				location.GetRef(),
				location.Id,
				location.Name,
				location.Description,
                facility,
                location.Building,
                location.Floor,
                location.PointOfCare,
                location.Room,
                location.Bed,
				location.Deactivated);
        }

        public void UpdateLocation(LocationDetail detail, Location location, IPersistenceContext context)
        {
        	location.Name = detail.Name;
			location.Id = detail.Id;
			location.Description = detail.Description;

            location.Facility = context.Load<Facility>(detail.Facility.FacilityRef, EntityLoadFlags.Proxy);
            location.Building = detail.Building;
            location.Floor = detail.Floor;
            location.PointOfCare = detail.PointOfCare;
            location.Room = detail.Room;
            location.Bed = detail.Bed;
        	location.Deactivated = detail.Deactivated;
        }

    }
}
