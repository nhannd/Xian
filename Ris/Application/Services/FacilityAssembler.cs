#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class FacilityAssembler
	{
		public FacilitySummary CreateFacilitySummary(Facility facility)
		{
			return new FacilitySummary(
				facility.GetRef(),
				facility.Code,
				facility.Name,
				facility.Description,
				EnumUtils.GetEnumValueInfo(facility.InformationAuthority),
				facility.Deactivated);
		}

		public FacilityDetail CreateFacilityDetail(Facility facility)
		{
			return new FacilityDetail(
				facility.GetRef(),
				facility.Code,
				facility.Name,
				facility.Description,
				EnumUtils.GetEnumValueInfo(facility.InformationAuthority),
				facility.Deactivated);
		}

		public void UpdateFacility(FacilityDetail detail, Facility facility, IPersistenceContext context)
		{
			facility.Code = detail.Code;
			facility.Name = detail.Name;
			facility.Description = detail.Description;
			facility.InformationAuthority = EnumUtils.GetEnumValue<InformationAuthorityEnum>(detail.InformationAuthority, context);
			facility.Deactivated = detail.Deactivated;
		}
	}
}
