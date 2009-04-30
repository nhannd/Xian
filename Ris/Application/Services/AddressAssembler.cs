#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
