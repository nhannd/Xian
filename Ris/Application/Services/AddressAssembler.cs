#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
    public class AddressAssembler
    {
        public AddressDetail CreateAddressDetail(Address address, IPersistenceContext context)
        {
            if (address == null)
                return null;

            AddressDetail addressDetail = new AddressDetail();

            addressDetail.Street = address.Street;
            addressDetail.Unit = address.Unit;
            addressDetail.City = address.City;
            addressDetail.Province = address.Province;
            addressDetail.PostalCode = address.PostalCode;
            addressDetail.Country = address.Country;

            addressDetail.Type = EnumUtils.GetEnumValueInfo(address.Type, context);

            addressDetail.ValidRangeFrom = address.ValidRange.From;
            addressDetail.ValidRangeUntil = address.ValidRange.Until;

            return addressDetail;
        }

        public Address CreateAddress(AddressDetail addressDetail)
        {
            if (addressDetail == null)
                return null;

            Address newAddress = new Address();

            newAddress.Street = addressDetail.Street;
            newAddress.Unit = addressDetail.Unit;
            newAddress.City = addressDetail.City;
            newAddress.Province = addressDetail.Province;
            newAddress.PostalCode = addressDetail.PostalCode;
            newAddress.Country = addressDetail.Country;
            newAddress.Type = EnumUtils.GetEnumValue<AddressType>(addressDetail.Type);
            newAddress.ValidRange.From = addressDetail.ValidRangeFrom;
            newAddress.ValidRange.Until = addressDetail.ValidRangeUntil;

            return newAddress;
        }
    }
}
