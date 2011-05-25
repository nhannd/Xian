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
			return new LocationSummary(
				location.GetRef(),
				location.Id,
				location.Name,
				new FacilityAssembler().CreateFacilitySummary(location.Facility),
				location.Building,
				location.Floor,
				location.PointOfCare,
				location.Deactivated);
		}

		public LocationDetail CreateLocationDetail(Location location)
		{
			return new LocationDetail(
				location.GetRef(),
				location.Id,
				location.Name,
				location.Description,
				new FacilityAssembler().CreateFacilitySummary(location.Facility),
				location.Building,
				location.Floor,
				location.PointOfCare,
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
			location.Deactivated = detail.Deactivated;
		}

	}
}
