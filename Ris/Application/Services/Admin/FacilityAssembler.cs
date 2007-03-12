using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services.Admin
{
    class FacilityAssembler
    {
        public FacilitySummary CreateFacilitySummary(Facility facility)
        {
            FacilitySummary summary = new FacilitySummary();
            summary.FacilityRef = facility.GetRef();
            summary.Code = facility.Code;
            summary.Name = facility.Name;
            return summary;
        }

        public FacilityDetail CreateFacilityDetail(Facility facility)
        {
            FacilityDetail detail = new FacilitySummary();
            detail.Code = facility.Code;
            detail.Name = facility.Name;
            return detail;
        }

        public FacilityDetail UpdateFacility(Facility facility, FacilityDetail detail)
        {
            facility.Code = detail.Code;
            facility.Name = detail.Name;
        }
    }
}
