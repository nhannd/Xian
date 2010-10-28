#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services
{
	public class DepartmentAssembler
	{
		public DepartmentSummary CreateSummary(Department item, IPersistenceContext context)
		{
			return new DepartmentSummary(item.GetRef(),
										 item.Id,
										 item.Name,
										 item.Facility.Code,
										 item.Facility.Name,
										 item.Deactivated);
		}

		public DepartmentDetail CreateDetail(Department item, IPersistenceContext context)
		{
			var facilityAssembler = new FacilityAssembler();
			return new DepartmentDetail(item.GetRef(),
										 item.Id,
										 item.Name,
										 item.Description,
										 facilityAssembler.CreateFacilitySummary(item.Facility),
										 item.Deactivated);
		}

		public void UpdateDepartment(Department item, DepartmentDetail detail, IPersistenceContext context)
		{
			item.Id = detail.Id;
			item.Name = detail.Name;
			item.Description = detail.Description;
			item.Facility = context.Load<Facility>(detail.Facility.FacilityRef, EntityLoadFlags.Proxy);
			item.Deactivated = detail.Deactivated;
		}
	}
}