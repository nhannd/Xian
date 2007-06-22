using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    public class LocationAssembler
    {
        public LocationSummary CreateLocationSummary(Location location)
        {
            return new LocationSummary(
                location.GetRef(),
                location.Facility.Name,
                location.Facility.Code,
                location.Building,
                location.Floor,
                location.PointOfCare,
                location.Room,
                location.Bed,
                location.Active,
                location.InactiveDate);
        }

        public LocationDetail CreateLocationDetail(Location location)
        {
            return new LocationDetail(
                location.Facility.GetRef(),
                location.Facility.Name,
                location.Facility.Code,
                location.Building,
                location.Floor,
                location.PointOfCare,
                location.Room,
                location.Bed);
        }

        public void UpdateLocation(LocationDetail detail, Location location, IPersistenceContext context)
        {
            location.Facility = context.Load<Facility>(detail.Facility.FacilityRef, EntityLoadFlags.Proxy);
            location.Building = detail.Building;
            location.Floor = detail.Floor;
            location.PointOfCare = detail.PointOfCare;
            location.Room = detail.Room;
            location.Bed = detail.Bed;
        }

    }
}
