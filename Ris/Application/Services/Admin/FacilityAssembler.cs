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
            return new FacilitySummary(
                facility.GetRef(),
                facility.Code,
                facility.Name);
        }

        public FacilityDetail CreateFacilityDetail(Facility facility)
        {
            return new FacilityDetail(
                facility.Code,
                facility.Name);
        }

        public void UpdateFacility(FacilityDetail detail, Facility facility)
        {
            facility.Code = detail.Code;
            facility.Name = detail.Name;
        }
    }
}
