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
            if (healthcard == null)
                return new HealthcardDetail();

            return new HealthcardDetail(
                healthcard.Id,
                healthcard.AssigningAuthority,
                healthcard.VersionCode,
                healthcard.ExpiryDate);
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
