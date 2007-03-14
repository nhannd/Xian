using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Services
{
    public class HealthcardAssembler
    {
        public HealthcardDetail CreateHealthcardDetail(HealthcardNumber healthcard)
        {
            HealthcardDetail detail = new HealthcardDetail();
            detail.Id = healthcard.Id;
            detail.AssigningAuthority = healthcard.AssigningAuthority;
            detail.ExpiryDate = healthcard.ExpiryDate;
            detail.VersionCode = healthcard.VersionCode;
            return detail;
        }

        public void UpdatePersonName(HealthcardDetail detail, HealthcardNumber healthcard)
        {
            healthcard.Id = detail.Id;
            healthcard.AssigningAuthority = detail.AssigningAuthority;
            healthcard.ExpiryDate = detail.ExpiryDate;
            healthcard.VersionCode = detail.VersionCode;
        }    
    }
}
