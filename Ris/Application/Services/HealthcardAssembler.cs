#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
                EnumUtils.GetEnumValueInfo(healthcard.AssigningAuthority),
                healthcard.VersionCode,
                healthcard.ExpiryDate);
        }

        public void UpdateHealthcard(HealthcardNumber hc, HealthcardDetail detail, IPersistenceContext context)
        {
            hc.Id = detail.Id;
            hc.AssigningAuthority = EnumUtils.GetEnumValue<InsuranceAuthorityEnum>(detail.AssigningAuthority, context);
            hc.VersionCode = detail.VersionCode;
            hc.ExpiryDate = detail.ExpiryDate;
        }
    }
}
