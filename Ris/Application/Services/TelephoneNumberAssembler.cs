#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class TelephoneNumberAssembler
    {
        public TelephoneDetail CreateTelephoneDetail(TelephoneNumber telephoneNumber, IPersistenceContext context)
        {
            if (telephoneNumber == null)
                return null;

            TelephoneDetail telephoneDetail = new TelephoneDetail();

            telephoneDetail.CountryCode = telephoneNumber.CountryCode;
            telephoneDetail.AreaCode = telephoneNumber.AreaCode;
            telephoneDetail.Number = telephoneNumber.Number;
            telephoneDetail.Extension = telephoneNumber.Extension;

            SimplifiedPhoneTypeAssembler simplePhoneTypeAssembler = new SimplifiedPhoneTypeAssembler();
            telephoneDetail.Type = simplePhoneTypeAssembler.GetSimplifiedPhoneType(telephoneNumber);

            telephoneDetail.ValidRangeFrom = telephoneNumber.ValidRange.From;
            telephoneDetail.ValidRangeUntil = telephoneNumber.ValidRange.Until;

            return telephoneDetail;
        }

        public TelephoneNumber CreateTelephoneNumber(TelephoneDetail telephoneDetail)
        {
            if (telephoneDetail == null)
                return null;

            TelephoneNumber telephoneNumber = new TelephoneNumber();

            telephoneNumber.CountryCode = telephoneDetail.CountryCode;
            telephoneNumber.AreaCode = telephoneDetail.AreaCode;
            telephoneNumber.Number = telephoneDetail.Number;
            telephoneNumber.Extension = telephoneDetail.Extension;
            telephoneNumber.ValidRange.From = telephoneDetail.ValidRangeFrom;
            telephoneNumber.ValidRange.Until = telephoneDetail.ValidRangeUntil;

            SimplifiedPhoneTypeAssembler simplePhoneTypeAssembler = new SimplifiedPhoneTypeAssembler();
            simplePhoneTypeAssembler.UpdatePhoneNumber(telephoneDetail.Type, telephoneNumber);

            return telephoneNumber;
        }
    }
}
