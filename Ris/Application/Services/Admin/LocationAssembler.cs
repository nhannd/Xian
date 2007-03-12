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
            LocationSummary summary = new LocationSummary();
            summary.LocationRef = location.GetRef();
            summary.Active = location.Active;
            summary.Bed = location.Bed;
            summary.Building = location.Building;
            summary.FacilityCode = location.Facility.Code;
            summary.FacilityName = location.Facility.Name;
            summary.Floor = location.Floor;
            summary.InactiveDate = location.InactiveDate;
            summary.PointOfCare = location.PointOfCare;
            summary.Room = location.Room;
            return summary;
        }

        public LocationDetail CreateLocationDetail(Location location)
        {
            LocationDetail detail = new LocationDetail();
            detail.Bed = location.Bed;
            detail.Building = location.Building;
            detail.Facility = location.Facility.GetRef();
            detail.Floor = location.Floor;
            detail.PointOfCare = location.PointOfCare;
            detail.Room = location.Room;

            return detail;
        }

        public void UpdateLocation(Location location, LocationDetail detail, IPersistenceContext context)
        {
            location.Bed = detail.Bed;
            location.Building = detail.Building;
            location.Facility = (Facility)context.Load(detail.Facility, EntityLoadFlags.Proxy);
            location.Floor = detail.Floor;
            location.PointOfCare = detail.PointOfCare;
            location.Room = detail.Room;
        }

    }
}
